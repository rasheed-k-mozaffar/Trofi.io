using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trofi.io.Shared;

namespace Trofi.io.Server;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("/api/[controller]")]
public class CartsController : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly AppDbContext _context;

    public CartsController(ICartRepository cartRepository, AppDbContext context)
    {
        _cartRepository = cartRepository;
        _context = context;
    }

    /// <summary>
    /// Gets cart items for the requesting user
    /// </summary>
    /// <returns></returns>
    [HttpGet("get-items")]
    public async Task<IActionResult> GetItems()
    {
        var items = await _cartRepository.GetCartItemsAsync();
        var itemsAsDtos = items.Select(i => i.ToCartItemDto()); // map the items to DTOs

        return Ok(new ApiResponse<IEnumerable<CartItemDto>>
        {
            Message = "Cart items retrieved successfully",
            Body = itemsAsDtos,
            IsSuccess = true
        });
    }

    /// <summary>
    /// Adds an item to the user's cart
    /// </summary>
    /// <param name="request">The request contains the product ID and the quantity. (Quantity defaults to one)</param>
    /// <returns></returns>
    [HttpPost("add-to-cart")]
    public async Task<IActionResult> AddToCart(AddToCartRequest request)
    {
        if (ModelState.IsValid)
        {
            bool result;
            if (request.Quantity is 0)
            {
                result = await _cartRepository.AddItemToCartAsync(request.ProductId);
            }
            else
            {
                result = await _cartRepository.AddItemToCartAsync(request.ProductId, request.Quantity);
            }

            if (result)
            {
                return Ok(new ApiResponse
                {
                    Message = "The item has been added to your cart",
                    IsSuccess = true
                });
            }
            else
            {
                return BadRequest(new ApiErrorResponse
                {
                    ErrorMessage = "Something went wrong while adding the item to your cart"
                });
            }
        }
        else
        {
            return BadRequest(ModelState);
        }
    }

    /// <summary>
    /// Empty the cart for a specific user
    /// </summary>
    /// <returns></returns>
    [HttpPost("flush-cart")]
    public async Task<IActionResult> ClearCart()
    {
        await _cartRepository.ClearCartAsync();

        return NoContent();
    }


    /// <summary>
    /// Remove item from cart
    /// </summary>
    /// <param name="id">The ID of the item to remove</param>
    /// <returns></returns>
    [HttpDelete("remove-item/{id}")]
    public async Task<IActionResult> RemoveItem(Guid id)
    {
        try
        {
            await _cartRepository.RemoveItemFromCartAsync(id);

            return Ok(new ApiResponse
            {
                Message = "The item has removed from your cart",
                IsSuccess = true
            });
        }
        catch (NotFoundException ex)
        {
            // item doesn't exist in cart
            return BadRequest(new ApiErrorResponse
            {
                ErrorMessage = ex.Message
            });
        }
    }

    /// <summary>
    /// Update a cart item's quantity to something else
    /// </summary>
    /// <param name="id">The id of the cart item to alter its quantity</param>
    /// <param name="request">The request including the updated quantity value</param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateCartItemQuantity(Guid id, UpdateCartItemQuantityRequest request)
    {
        var item = await _context.CartItems.FindAsync(id);

        if (item is not null)
        {
            item.Quantity = request.UpdatedQuantity;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        else
        {
            // The item the user is trying to update its quantity doesn't exist
            return BadRequest(new ApiErrorResponse
            {
                ErrorMessage = "The item you're trying to access doesn't exist"
            });
        }
    }
}

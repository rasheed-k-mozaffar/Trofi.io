using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Trofi.io.Shared;

namespace Trofi.io.Server;

[ApiController]
[Route("/api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuRepository _menuRepository;
    private readonly ILogger<MenuController> _logger;
    private readonly AppDbContext _context;

    public MenuController
    (
        IMenuRepository menuRepository,
        ILogger<MenuController> logger,
        AppDbContext context)
    {
        _menuRepository = menuRepository;
        _logger = logger;
        _context = context;
    }


    /// <summary>
    /// Gets all the menu items available in the database, maps them to their equivalent dto type and returns them
    /// along side a success message
    /// </summary>
    /// <returns></returns>
    [HttpGet("all")]
    public async Task<IActionResult> GetMenu()
    {
        var menu = await _menuRepository.GetMenuAsync();

        if (!menu.Any())
        {
            // the db query returned no results
            return Ok(new ApiResponse
            {
                Message = "There are currently no items on the menu. Please check back later!",
                IsSuccess = true
            });
        }
        else
        {
            var menuAsDto = menu.Select(mi => mi.ToMenuItemDto());
            return Ok(new ApiResponse<IEnumerable<MenuItemDto>>
            {
                Message = "Menu retrieved successfully",
                Body = menuAsDto,
                IsSuccess = true
            });
        }
    }

    [HttpGet("item/{id}")]
    public async Task<IActionResult> GetItem(Guid id)
    {
        try
        {
            var item = await _menuRepository.GetDishByIdAsync(id);
            var itemAsDto = item.ToMenuItemDto();

            return Ok(new ApiResponse<MenuItemDto>
            {
                Message = "Item retrieved successfully",
                Body = itemAsDto,
                IsSuccess = true
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiErrorResponse
            {
                ErrorMessage = ex.Message
            });
        }
    }


    /// <summary>
    /// Adds a new dish item to the menu if the given model is valid, and the insertion was a success
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    [HttpPost("add")]
    public async Task<IActionResult> AddItem(DishCreateDto item)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var itemToAdd = item.ToMenuItemCreate();
                var addedItemId = await _menuRepository.AddDishAsync(itemToAdd); // map the item to a useable data model

                return Ok(new ApiResponse<Guid>
                {
                    Message = $"{item.Name} was successfully added to the menu",
                    Body = addedItemId,
                    IsSuccess = true
                });
            }
            catch (ResourceCreationFailedException ex)
            {
                // the item failed the creation process
                _logger.LogError("Failed attempt to add a new item to the menu");
                return BadRequest(new ApiErrorResponse
                {
                    ErrorMessage = ex.Message
                });
            }
        }
        else
        {
            return BadRequest(ModelState);
        }
    }


    /// <summary>
    /// Deletes an item from the menu items table using its id
    /// </summary>
    /// <param name="id">The id of the item to delete</param>
    /// <returns></returns>
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteItem(Guid id)
    {
        try
        {
            await _menuRepository.RemoveDishAsync(id);

            return Ok(new ApiResponse
            {
                Message = "The item was successfully removed from the menu",
                IsSuccess = true
            });
        }
        catch (NotFoundException ex)
        {
            return BadRequest(new ApiErrorResponse
            {
                ErrorMessage = ex.Message
            });
        }
    }


    /// <summary>
    /// Updates the details of a menu item by first checking wether the item exists or not
    /// and then maps the updated details to the retrieved object from the db then saves the changes
    /// </summary>
    /// <param name="id">The id of the item to update</param>
    /// <param name="updatedItem">The updated object received in the request body</param>
    /// <returns></returns>
    [HttpPut("edit-dish/{id}")]
    public async Task<IActionResult> EditItem(Guid id, DishEditDto updatedItem)
    {
        if (ModelState.IsValid)
        {
            var itemToUpdate = await _context.MenuItems.FindAsync(id);

            if (itemToUpdate is null)
            {
                // the id given doesn't exist in the db
                return NotFound(new ApiErrorResponse
                {
                    ErrorMessage = "The item you are tying to edit doesn't exist"
                });
            }

            // map the updated values
            MapUpdatedItemToOldItem(itemToUpdate, updatedItem);

            await _context.SaveChangesAsync();
            return Ok(new ApiResponse
            {
                Message = "The item has been updated successfully",
                IsSuccess = true
            });
        }
        else
        {
            ModelState.AddModelError(key: "Invalid data format", errorMessage: "The information you have provided is invalid");
            return BadRequest(ModelState);
        }
    }

    private void MapUpdatedItemToOldItem(MenuItem oldItem, DishEditDto updatedItem)
    {
        oldItem.Name = updatedItem.Name;
        oldItem.Description = updatedItem.Description;
        oldItem.Price = updatedItem.Price;
        oldItem.UpdatedPrice = updatedItem.UpdatedPrice;
        oldItem.IsAvailable = updatedItem.IsAvailable;
        oldItem.IsSpecial = updatedItem.IsSpecial;
    }
}

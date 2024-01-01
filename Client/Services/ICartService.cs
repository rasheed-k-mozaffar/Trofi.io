namespace Trofi.io.Client.Services;

public interface ICartService
{
    Task<ApiResponse<IEnumerable<CartItemDto>>> GetCartItemsAsync();
    Task<ApiResponse> AddProductToCartAsync(AddToCartRequest request);
    Task<ApiResponse> RemoveProductFromCartAsync(Guid itemId);
    Task<ApiResponse> UpdateCartProductQuantityAsync(Guid itemId, byte newQunatity);
    Task<string> ClearCartAsync();
}

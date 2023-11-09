namespace Trofi.io.Server.Repositories;

public interface ICartRepository
{
    Task<IEnumerable<CartItem>> GetCartItemsAsync();
    Task<bool> AddItemToCartAsync(Guid productId, byte quantity = 1);
    Task RemoveItemFromCartAsync(Guid cartItemId);
    Task ClearCartAsync();
    Task<bool> CheckoutAsync();
}

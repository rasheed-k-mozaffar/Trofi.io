namespace Trofi.io.Client.Services;

public interface IMenuService
{
    Task<ApiResponse<IEnumerable<MenuItemDto>>> GetMenuAsync();
    Task<ApiResponse<MenuItemDto>> GetMenuItemByIdAsync(Guid itemId);
    Task<ApiResponse<Guid>> AddItemAsync(DishCreateDto item);
    Task<ApiResponse> RemoveItemAsync(Guid itemId);
    Task<ApiResponse> UpdateItemAsync(Guid itemId, DishEditDto editedItem);
}

using System.Net.Http.Json;

namespace Trofi.io.Client.Services;

public class MenuService : IMenuService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "/api/menu";

    public MenuService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<Guid>> AddItemAsync(DishCreateDto item)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/add", item);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new ResourceCreationFailedException(message: error!.ErrorMessage!);
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
        return result!;
    }

    public async Task<ApiResponse<IEnumerable<MenuItemDto>>> GetMenuAsync()
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/all");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new DataRetrievalException(message: error!.ErrorMessage!);
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<MenuItemDto>>>();
        return result!;
    }

    public async Task<ApiResponse<MenuItemDto>> GetMenuItemByIdAsync(Guid itemId)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/item/{itemId}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new DataRetrievalException(message: error!.ErrorMessage!);
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<MenuItemDto>>();
        return result!;
    }

    public async Task<ApiResponse> RemoveItemAsync(Guid itemId)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/delete/{itemId}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new OperationFailureException(message: error!.ErrorMessage!);
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        return result!;
    }

    public async Task<ApiResponse> UpdateItemAsync(Guid itemId, DishEditDto editedItem)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/edit-dish/{itemId}", editedItem);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new OperationFailureException(message: error!.ErrorMessage!);
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        return result!;
    }
}

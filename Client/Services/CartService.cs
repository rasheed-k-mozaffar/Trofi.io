
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Trofi.io.Client.States;

namespace Trofi.io.Client.Services;

public class CartService : ICartService
{
    private const string BaseUrl = "/api/carts";
    #region Injected Dependencies
    private readonly HttpClient _httpClient;

    public CartService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    #endregion

    #region Methods

    public async Task<ApiResponse> AddProductToCartAsync(AddToCartRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/add-to-cart", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new OperationFailureException(message: error!.ErrorMessage!);
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        return result!;
    }

    public async Task<string> ClearCartAsync()
    {
        var response = await _httpClient.PostAsync($"{BaseUrl}/flush-cart", null);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new OperationFailureException(message: error!.ErrorMessage!);
        }

        return "Your cart has been emptied!";
    }

    public async Task<ApiResponse<IEnumerable<CartItemDto>>> GetCartItemsAsync()
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/get-items");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new DataRetrievalException(message: error!.ErrorMessage!);
        }

        var result = await response.
                            Content.ReadFromJsonAsync<ApiResponse<IEnumerable<CartItemDto>>>();
        return result!;
    }

    public async Task<ApiResponse> RemoveProductFromCartAsync(Guid itemId)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/remove-item/{itemId}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new OperationFailureException(message: error!.ErrorMessage!);
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        return result!;
    }

    public async Task<ApiResponse> UpdateCartProductQuantityAsync(Guid itemId, byte newQunatity)
    {
        var response = await _httpClient
                            .PatchAsJsonAsync($"{BaseUrl}/{itemId}", new UpdateCartItemQuantityRequest
                            {
                                UpdatedQuantity = newQunatity
                            });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new OperationFailureException(message: error!.ErrorMessage!);
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        return result!;
    }

    #endregion
}

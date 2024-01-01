using System.Net.Http.Json;

namespace Trofi.io.Client.States;

public class CartState
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "/api/carts";

    public static bool IsLoadingCartData = true;
    public static List<CartItemDto> CartItems = new();
    public int CartItemsCount { get; set; } = 0;

    public event Action? OnChange;

    public CartState(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("Trofi.io.ServerAPI");
    }

    public async Task FetchItemsAsync()
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/get-items");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<CartItemDto>>>();

            if (result?.Body is not null && result.Body.Any())
            {
                CartItems = result.Body.ToList();
                CartItemsCount = CartItems.Sum(i => i.Quantity);
            }
            IsLoadingCartData = false;
        }
    }

    public void NotifyStateChanged() => OnChange?.Invoke();
}

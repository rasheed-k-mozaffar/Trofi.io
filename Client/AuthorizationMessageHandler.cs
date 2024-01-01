using Blazored.LocalStorage;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Trofi.io.Client;

public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthorizationMessageHandler(ILocalStorageService localStorage, IHttpClientFactory httpClientFactory)
    {
        _localStorage = localStorage;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (await _localStorage.ContainKeyAsync("access_token"))
        {
            // get the token and attach it to the request authorization header
            string token = await _localStorage.GetItemAsStringAsync("access_token");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await base.SendAsync(request, cancellationToken);

            // the current user's JWT is expired, get them a fresh one
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(token))
            {
                var refreshResult = await RefreshTokenAsync();
                Console.WriteLine($"JWT refreshed: {refreshResult.IsSuccess}");

                await SetAccessTokenAsync(refreshResult.Body!);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                response = await base.SendAsync(request, cancellationToken);
            }

            return response;
        }
        else
        {
            return await base.SendAsync(request, cancellationToken);
        }
    }

    private async Task SetAccessTokenAsync(string token)
    {
        await _localStorage.SetItemAsStringAsync("access_token", token);
    }

    private async Task<ApiResponse<string>> RefreshTokenAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("Trofi.io.ServerAPI");
        var response = await httpClient.GetAsync("/api/auth/refresh-token");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new AuthFailureException(message: error!.ErrorMessage!);
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        return result!;
    }
}

using System.Net.Http.Json;

namespace Trofi.io.Client.Services;

public class AuthService : IAuthService
{
    # region Variables
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    #endregion

    #region Methods
    public async Task<ApiResponse> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/change-password", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new AuthFailureException(message: error!.ErrorMessage!);
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        return result!;
    }

    public async Task<ApiResponse<string>> LoginUserAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new AuthFailureException(message: error!.ErrorMessage!);
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        return result!;
    }

    public async Task<ApiResponse<string>> RefreshTokenAsync()
    {
        var response = await _httpClient.GetAsync("/api/auth/refresh-token");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new AuthFailureException(message: error!.ErrorMessage!);
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        return result!;
    }

    public async Task RegisterUserAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new AuthFailureException(message: error!.ErrorMessage!);
        }
    }

    public async Task RevokeTokenAsync(RevokeTokenRequest? request = null)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/revoke-token", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new AuthFailureException(message: error!.ErrorMessage!);
        }
    }
    #endregion
}

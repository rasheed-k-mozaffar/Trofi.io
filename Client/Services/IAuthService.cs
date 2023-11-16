namespace Trofi.io.Client.Services;

public interface IAuthService
{
    Task RegisterUserAsync(RegisterRequest request);
    Task<ApiResponse<string>> LoginUserAsync(LoginRequest request);
    Task<ApiResponse> ChangePasswordAsync(ChangePasswordRequest request);
    Task<ApiResponse<string>> RefreshTokenAsync();
    Task RevokeTokenAsync(RevokeTokenRequest? request = null);
}

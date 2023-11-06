using Microsoft.AspNetCore.Identity;
using Trofi.io.Server.Models;
using Trofi.io.Shared.Auth;
using Trofi.io.Shared.Auth;

namespace Trofi.io.Server.Repositories
{
    public interface IAuthRepository
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterRequest request);
        Task<UserManagerResponse> LoginUserAsync(LoginRequest request);
        Task<UserManagerResponse> UpdatePasswordAsync(ChangePasswordRequest request);

    }
}

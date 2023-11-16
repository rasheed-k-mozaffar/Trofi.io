using Blazored.LocalStorage;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace Trofi.io.Client;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;

    public JwtAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (await _localStorage.ContainKeyAsync("access_token"))
        {
            var token = await _localStorage.GetItemAsStringAsync("access_token");
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            ClaimsIdentity identity = new ClaimsIdentity(jwt.Claims, "Bearer");
            ClaimsPrincipal user = new ClaimsPrincipal(identity);

            AuthenticationState authState = new AuthenticationState(user);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));

            return authState;
        }

        // Give the user an anonymous identity
        return new AuthenticationState(new ClaimsPrincipal());
    }
}

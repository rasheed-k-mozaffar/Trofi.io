using System.Runtime.CompilerServices;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Trofi.io.Client.Pages.Users;

public partial class Login : ComponentBase
{
    #region Injected Services

    [Inject]
    public IAuthService AuthService { get; set; } = default!;

    [Inject]
    public NavigationManager Nav { get; set; } = default!;

    [Inject]
    public ILocalStorageService LocalStorage { get; set; } = default!;

    [Inject]
    public AuthenticationStateProvider AuthState { get; set; } = default!;
    #endregion

    LoginRequest request = new();
    bool isMakingRequest = false;
    string errorMessage = string.Empty;

    private async Task HandleUserLoginAsync()
    {
        isMakingRequest = true;
        errorMessage = string.Empty;

        try
        {
            var result = await AuthService.LoginUserAsync(request);
            if (result.IsSuccess) // the login succeeded
            {
                // set the token in the local storage and call auth state changes
                await LocalStorage.SetItemAsStringAsync("access_token", result.Body);
                await AuthState.GetAuthenticationStateAsync();

                Nav.NavigateTo("/");
            }
        }
        catch (AuthFailureException ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isMakingRequest = false;
        }
    }
}

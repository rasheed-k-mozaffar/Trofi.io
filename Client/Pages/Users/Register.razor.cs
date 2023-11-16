using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Trofi.io.Client.Pages.Users;

public partial class Register : ComponentBase
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

    RegisterRequest request = new();
    bool isMakingRequest = false;
    string errorMessage = string.Empty;

    private async Task HandleUserRegistrationAsync()
    {
        isMakingRequest = true;
        errorMessage = string.Empty;

        try
        {
            await AuthService.RegisterUserAsync(request);

            // if the registration went all good, then use the request to also sign up the user
            LoginRequest loginRequest = new()
            {
                Email = request.Email,
                Password = request.Password
            };

            var loginResult = await AuthService.LoginUserAsync(loginRequest);

            if (loginResult.IsSuccess)
            {
                await LocalStorage.SetItemAsStringAsync("access_token", loginResult.Body);
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

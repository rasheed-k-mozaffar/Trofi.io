using System.ComponentModel.DataAnnotations;

namespace Trofi.io.Shared.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "Your email is required")]
    [EmailAddress(ErrorMessage = "The email you entered is an invalid email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Your password is required")]
    public string? Password { get; set; }
}


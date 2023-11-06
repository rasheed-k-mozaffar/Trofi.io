using System.ComponentModel.DataAnnotations;

namespace Trofi.io.Shared.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "Your email is required")]
    [EmailAddress(ErrorMessage = "The email you entered is an invalid email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Please enter a user name")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Your phone number is required")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Your first name is required")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Your last name is required")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Your location is required")]
    public string? Location { get; set; }

    [Required(ErrorMessage = "You need to create a password")]
    public string? Password { get; set; }
}


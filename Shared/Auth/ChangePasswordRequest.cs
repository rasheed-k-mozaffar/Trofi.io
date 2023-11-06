using System.ComponentModel.DataAnnotations;

namespace Trofi.io.Shared.Auth
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Your email is required")]
        [EmailAddress(ErrorMessage = "The email you entered is an invalid email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Your old password is required")]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "Your new password is required")]
        public string? NewPassword { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Trofi.io.Shared;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "The category name is required")]
    [MaxLength(50, ErrorMessage = "The category name cannot be longer than 50 characters")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "The category description is required")]
    [MaxLength(250, ErrorMessage = "The category description cannot be longer than 250 characters")]
    public string? Description { get; set; }

    public string? LogoUrl { get; set; }
}

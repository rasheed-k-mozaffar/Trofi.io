using System.ComponentModel.DataAnnotations;
using System.Runtime.ConstrainedExecution;

namespace Trofi.io.Shared;

public class DishCreateDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "The dish name is required")]
    [MaxLength(100, ErrorMessage = "The dish name should be less than 100 characters long")]
    [MinLength(3, ErrorMessage = "The dish name should be greater than 3 characters")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "The dish description is required")]
    [MaxLength(100, ErrorMessage = "The dish description should be less than 2000 characters long")]
    [MinLength(3, ErrorMessage = "The dish description should be greater than 10 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "The dish price is required")]
    [Range(1, 9999, ErrorMessage = "The dish price should be $1 minimum and $9999.9 maximum")]
    public double Price { get; set; }
    public bool IsSpecial { get; set; }
    public bool IsAvailable { get; set; }
}

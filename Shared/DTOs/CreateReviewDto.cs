using System.ComponentModel.DataAnnotations;

namespace Trofi.io.Shared;

public class CreateReviewDto
{
    [Required]
    [MaxLength(200, ErrorMessage = "The title of the review cannot be longer than 200 characters")]
    public string? Title { get; set; }

    [Required]
    [MaxLength(2500, ErrorMessage = "The review cannot be longer than 2,500 characters")]
    public string? Review { get; set; }

    [Range(1, 5, ErrorMessage = "The rating must be between 1 and 5")]
    public float Rating { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Trofi.io.Server.Models;

public class DishImage
{
    [Key]
    public Guid Id { get; set; }

    public Guid MenuItemId { get; set; }

    [Required]
    public string? URL { get; set; }

    [Required]
    public string? Path { get; set; }
}
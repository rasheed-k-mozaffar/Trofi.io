using System.ComponentModel.DataAnnotations;

namespace Trofi.io.Server.Models;

public class MenuItem
{
    [Key]
    public Guid Id { get; set; }

    public Guid? CartId { get; set; }

    [Required]
    [StringLength(maximumLength: 200)]
    public string? Name { get; set; }

    [Required]
    [StringLength(maximumLength: 7500)]
    public string? Description { get; set; }

    [Required]
    public double Price { get; set; }

    public double? UpdatedPrice { get; set; }
    public virtual ICollection<DishImage>? DishImages { get; set; } = new List<DishImage>();
    public bool IsSpecial { get; set; }
    public bool IsAvailable { get; set; }
}

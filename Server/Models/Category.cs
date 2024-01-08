using System.ComponentModel.DataAnnotations;

namespace Trofi.io.Server.Models;

public class Category
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(maximumLength: 50)]
    public string? Name { get; set; }

    [StringLength(maximumLength: 250)]
    public string? Description { get; set; }

    public string? LogoUrl { get; set; } // this property will be used to store the logo or image of a category

    public virtual ICollection<MenuItem>? Items { get; set; } = new List<MenuItem>();
}

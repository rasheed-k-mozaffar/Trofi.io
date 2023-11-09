using System.ComponentModel.DataAnnotations;

namespace Trofi.io.Server.Models;

public class Cart
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(450)]
    public string? AppUserId { get; set; }
    public virtual AppUser? CartOwner { get; set; }

    public virtual ICollection<CartItem>? Items { get; set; } = new List<CartItem>();
    public double Total => Items is not null ? Items.Sum(i => i.Price * i.Quantity) : 0;
}

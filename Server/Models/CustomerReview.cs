using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Trofi.io.Server.Models;

public class CustomerReview
{
    [Key]
    public Guid Id { get; set; }

    public Guid MenuItemID { get; set; }

    public string? AppUserId { get; set; }

    public string? ReviwerName { get; set; }

    [StringLength(maximumLength: 200)]
    public string? Title { get; set; }

    [Required]
    [StringLength(maximumLength: 2500)]
    public string? Review { get; set; }

    [Required]
    [Range(1, 5)]
    public float Rating { get; set; }

    public int UpVotes { get; set; }

    public DateTime WrittenOn { get; set; }

    public DateTime? EditedOn { get; set; }

    [JsonIgnore]
    public virtual MenuItem? ReviewedItem { get; set; }
}

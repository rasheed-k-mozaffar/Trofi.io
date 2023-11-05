using System.Runtime.ConstrainedExecution;

namespace Trofi.io.Shared;

public class DishCreateDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public bool IsSpecial { get; set; }
    public bool IsAvailable { get; set; }
}

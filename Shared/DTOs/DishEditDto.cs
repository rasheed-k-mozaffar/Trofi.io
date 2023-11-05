namespace Trofi.io.Shared;

public class DishEditDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public double? UpdatedPrice { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsSpecial { get; set; }
}

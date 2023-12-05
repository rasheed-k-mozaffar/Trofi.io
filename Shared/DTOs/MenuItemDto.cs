namespace Trofi.io.Shared;

public class MenuItemDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public double? UpdatedPrice { get; set; }
    public IEnumerable<ImageDto>? Images { get; set; }
    public bool IsSpecial { get; set; }
    public bool IsAvailable { get; set; }
}

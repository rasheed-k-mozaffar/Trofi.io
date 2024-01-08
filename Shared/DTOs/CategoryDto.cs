namespace Trofi.io.Shared;

public class CategoryDto
{
    public Guid CategoryId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public IEnumerable<MenuItemDto>? Items { get; set; }
}

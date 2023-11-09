namespace Trofi.io.Shared;

public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public byte Quantity { get; set; }
    public double Price { get; set; }
    public double? UpdatedPrice { get; set; }
    public string? CoverImageURL { get; set; }
}

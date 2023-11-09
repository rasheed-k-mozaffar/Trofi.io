namespace Trofi.io.Server.Models;

public class CartItem
{
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? CoverImageURL { get; set; }
    public byte Quantity { get; set; }
    public double Price { get; set; }
    public double? UpdatedPrice { get; set; }
}

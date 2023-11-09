namespace Trofi.io.Shared;

public class AddToCartRequest
{
    public Guid ProductId { get; set; }
    public byte Quantity { get; set; }
}

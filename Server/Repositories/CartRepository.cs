
using Trofi.io.Server.Options;

namespace Trofi.io.Server.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly UserInfo _userInfo;

    public CartRepository(AppDbContext context, UserManager<AppUser> userManager, UserInfo userInfo)
    {
        _context = context;
        _userManager = userManager;
        _userInfo = userInfo;
    }

    public async Task<IEnumerable<CartItem>> GetCartItemsAsync()
    {
        var items = await _context.CartItems
        .Where(i => i.CartId == _userInfo.CartId).ToListAsync();

        return items.Any() ? items : Enumerable.Empty<CartItem>();
    }

    public async Task<bool> AddItemToCartAsync(Guid productId, byte quantity = 1)
    {
        // get the item from the db
        var itemToAdd = await _context.MenuItems
        .FirstOrDefaultAsync(i => i.Id == productId);

        if (itemToAdd is null)
        {
            throw new NotFoundException(message: "The item you are trying to add doesn't exist");
        }

        // check if the user's cart already contain the item, if so, just increment its quantity
        var userCart = await _context.Carts.FindAsync(_userInfo.CartId);

        if (userCart is null)
        {
            return false;
        }

        if (userCart.Items!.Any(i => i.ProductName == itemToAdd.Name))
        {
            var item = await _context.CartItems.FirstAsync(i => i.ProductName == itemToAdd.Name);
            item.Quantity++;
            await _context.SaveChangesAsync();

            return true;
        }

        // the item is new, and needs to be added
        var cartItem = new CartItem()
        {
            Id = Guid.NewGuid(),
            CartId = userCart.Id,
            ProductId = itemToAdd.Id,
            ProductName = itemToAdd.Name,
            Quantity = quantity,
            Price = itemToAdd.Price,
            UpdatedPrice = itemToAdd.UpdatedPrice,
            CoverImageURL = itemToAdd.DishImages?.LastOrDefault()?.URL ?? "images/no-images.png"
        };

        var result = await _context.CartItems.AddAsync(cartItem);
        if (result.State == EntityState.Added)
        {
            await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }

    public Task<bool> CheckoutAsync()
    {
        throw new NotImplementedException();
    }

    public async Task ClearCartAsync()
    {
        var items = await _context.CartItems
        .Where(ci => ci.CartId == _userInfo.CartId).ToListAsync();

        if (items is not null && items.Any())
        {
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveItemFromCartAsync(Guid cartItemId)
    {
        var item = await _context.CartItems.FindAsync(cartItemId);

        if (item is null)
        {
            throw new NotFoundException(message: "The item you are tying to remove doesn't exist");
        }

        var result = _context.CartItems.Remove(item);
        if (result.State == EntityState.Deleted)
        {
            await _context.SaveChangesAsync();
        }
    }
}

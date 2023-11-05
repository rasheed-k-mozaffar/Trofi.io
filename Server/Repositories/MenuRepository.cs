
namespace Trofi.io.Server;

public class MenuRepository : IMenuRepository
{
    private readonly AppDbContext _context;
    private readonly IFilesRepository _filesRepository;

    public MenuRepository(AppDbContext context, IFilesRepository filesRepository)
    {
        _context = context;
        _filesRepository = filesRepository;
    }

    /// <summary>
    /// Adds a new item without adding the images, the images get added through the files controller
    /// </summary>
    /// <param name="item">The new dish to add</param>
    /// <returns></returns>
    /// <exception cref="ResourceCreationFailedException"></exception>
    public async Task<Guid> AddDishAsync(MenuItem item)
    {
        var result = await _context.MenuItems.AddAsync(item);

        if (result.State == EntityState.Added)
        {
            await _context.SaveChangesAsync();
            return item.Id; // get back the item ID to use it later in the dish image gallery
        }
        else
        {
            throw new ResourceCreationFailedException(message: "Something went wrong while attempting to add the new item");
        }
    }

    /// <summary>
    /// Returns an item using its id
    /// </summary>
    /// <param name="id"> the id of the item to return</param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    public async Task<MenuItem> GetDishByIdAsync(Guid id)
    {
        var item = await _context.MenuItems
                                .FirstOrDefaultAsync(i => i.Id == id);

        if (item is not null)
        {
            return item;
        }
        else
        {
            throw new NotFoundException(message: "The item you are looking for doesn't exist");
        }
    }

    /// <summary>
    /// Gets all the items that are available in the menu
    /// </summary>
    /// <returns>A list of menu items if there are any, if not, an empty collection is returned</returns>
    public async Task<IEnumerable<MenuItem>> GetMenuAsync()
    {
        var items = await _context.MenuItems.ToListAsync();

        return items.Any() ? items : Enumerable.Empty<MenuItem>();
    }

    /// <summary>
    /// Removes a dish from the menu, if the dish has images belonging to it, they are also removed from 
    /// the file system and the database through the IFilesRepository
    /// </summary>
    /// <param name="id">The id of the dish to remove</param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    public async Task RemoveDishAsync(Guid id)
    {
        var item = await _context.MenuItems.FindAsync(id);

        if (item is not null)
        {
            // before completing the delete, we have to delete the associated images
            if (item.DishImages is not null && item.DishImages.Any())
            {
                for (int i = 0; i < item.DishImages.Count; i++)
                {
                    await _filesRepository.DeleteDishImageAsync(item.DishImages.ElementAt(i).Id);
                }
            }

            // finally delete the item itself
            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new NotFoundException(message: "The item you are trying to remove doesn't exist");
        }
    }

    //INFO: This doesn't have to be implemented as EF Core doesn't have update,
    // The update will happen in the controller directly
    public Task UpdateDishAsync(Guid id, MenuItem item)
    {
        throw new NotImplementedException();
    }
}

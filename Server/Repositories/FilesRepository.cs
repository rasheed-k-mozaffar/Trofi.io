namespace Trofi.io.Server.Repositories;

public class FilesRepository : IFilesRepository
{
    private readonly AppDbContext _context;

    public FilesRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Writes a new image to the database for a particular menu item
    /// </summary>
    /// <param name="imageUrl">The image URL in the file system</param>
    /// <param name="path">The root path of the image so that the image can be removed from the file system</param>
    /// <param name="dishId">The menu item which the image belongs to</param>
    /// <returns></returns>
    /// <exception cref="ResourceCreationFailedException">Thrown if the database failes to save the new image</exception>
    public async Task<DishImage> AddDishImageAsync(string imageUrl, string path, Guid dishId)
    {
        DishImage dishImage = new() { Id = Guid.NewGuid(), URL = imageUrl, Path = path, MenuItemId = dishId };

        var result = await _context.Images.AddAsync(dishImage);

        if (result.State == EntityState.Added)
        {
            await _context.SaveChangesAsync();
            return dishImage;
        }
        else
        {
            throw new ResourceCreationFailedException(message: "Something went wrong while trying to save the new image");
        }
    }

    /// <summary>
    /// Deletes a dish image record from the database
    /// </summary>
    /// <param name="imageId">The id of the image to delete</param>
    /// <returns></returns>
    /// <exception cref="NotFoundException">If the given id has no corresponding record in the database</exception>
    public async Task DeleteDishImageAsync(Guid imageId)
    {
        var imageToDelete = await _context.Images.FindAsync(imageId);

        if (imageToDelete is not null)
        {
            File.Delete(imageToDelete.Path!); // we know for a fact, the Path cannot be null

            var result = _context.Images.Remove(imageToDelete);

            if (result.State == EntityState.Deleted)
            {
                await _context.SaveChangesAsync();
            }
        }
        else
        {
            throw new NotFoundException(message: "The image doesn't exist in the system");
        }
    }

    /// <summary>
    /// Retrieves a dish image from the database using its id
    /// </summary>
    /// <param name="imageId">The id of the image to retrieve</param>
    /// <returns></returns>
    /// <exception cref="NotFoundException">If the given id has no corresponding record in the database</exception>
    public async Task<DishImage> GetDishImageAsync(Guid imageId)
    {
        var dishImage = await _context.Images.FirstOrDefaultAsync(i => i.Id == imageId);

        if (dishImage is not null)
        {
            return dishImage;
        }
        else
        {
            throw new NotFoundException(message: "The image you're looking for doesn't exist");
        }
    }
}

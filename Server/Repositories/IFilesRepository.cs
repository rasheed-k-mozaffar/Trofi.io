namespace Trofi.io.Server.Repositories;

public interface IFilesRepository
{
    Task<DishImage> AddDishImageAsync(string imageUrl, string path, Guid dishId);
    Task<DishImage> GetDishImageAsync(Guid imageId);
    Task DeleteDishImageAsync(Guid imageId);
}

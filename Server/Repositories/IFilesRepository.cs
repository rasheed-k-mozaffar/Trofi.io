namespace Trofi.io.Server.Repositories;

public interface IFilesRepository
{
    Task AddDishImageAsync(string imageUrl, Guid dishId);
    Task<DishImage> GetDishImageAsync(Guid imageId);
    Task DeleteDishImageAsync(Guid imageId);
}

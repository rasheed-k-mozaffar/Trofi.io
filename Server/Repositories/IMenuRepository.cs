namespace Trofi.io.Server.Repositories;

public interface IMenuRepository
{
    Task<Guid> AddDishAsync(MenuItem item);
    Task<MenuItem> GetDishByIdAsync(Guid id);
    Task<IEnumerable<MenuItem>> GetMenuAsync();
    Task RemoveDishAsync(Guid id);
    Task UpdateDishAsync(Guid id, MenuItem item);
}

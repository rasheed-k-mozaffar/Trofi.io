namespace Trofi.io.Server.Repositories;

public interface ICategoriesRepository
{
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<IEnumerable<MenuItem>> GetCategoryItemsAsync(Guid categoryId);
    Task<Category> GetCategoryAsync(Guid categoryId);
    Task CreateCategoryAsync(Category category);
    Task DeleteCategoryAsync(Guid categoryId);
    Task UpdateCategoryAsync(Guid categoryId, Category updatedCategory);
}

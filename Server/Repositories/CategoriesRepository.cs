
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Trofi.io.Server.Repositories;

public class CategoriesRepository : ICategoriesRepository
{
    private readonly AppDbContext _context;

    public CategoriesRepository(AppDbContext context)
    {
        _context = context;
    }


    public async Task CreateCategoryAsync(Category category)
    {
        var existingCategory = await _context.Categories
                                       .FirstOrDefaultAsync(c => c.Name == category.Name);

        // check wether a category with the name exists
        if (existingCategory is not null)
        {
            throw new ResourceCreationFailedException(message: $"There's already an existing category with the name {category.Name}");
        }

        var entityEntry = await _context.Categories.AddAsync(category);

        if (entityEntry.State == EntityState.Added)
        {
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new ResourceCreationFailedException(message: "Something went wrong while creating the new category");
        }
    }

    public async Task DeleteCategoryAsync(Guid categoryId)
    {
        var category = await _context.Categories.FindAsync(categoryId);

        if (category is null)
        {
            throw new NotFoundException(message: "The category you are trying to delete doesn't exist");
        }

        // before deleting the category, detach the menu items connected to it
        var relatedItems = await _context.MenuItems
                                         .Where(i => i.CategoryId == categoryId)
                                         .ToListAsync();

        EntityEntry deletionResult;

        // if there're items categorized under this category, 
        // perform a bulk update on the categoryId property, then delete the category
        if (relatedItems is not null && relatedItems.Any())
        {
            await _context.Database
            .ExecuteSqlRawAsync("UPDATE MenuItems SET CategoryId = NULL WHERE CategoryId = {0}", categoryId);
        }

        deletionResult = _context.Categories.Remove(category);

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        var categories = await _context.Categories.ToListAsync();

        return categories.Any() ? categories : Enumerable.Empty<Category>();
    }

    public async Task<Category> GetCategoryAsync(Guid categoryId)
    {
        var category = await _context.Categories
                                    .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category is null)
        {
            throw new NotFoundException(message: "The category you're looking for was not found");
        }

        return category;
    }

    public async Task<IEnumerable<MenuItem>> GetCategoryItemsAsync(Guid categoryId)
    {
        var category = await _context.Categories
                                    .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category is null)
        {
            throw new NotFoundException(message: "The category you're looking for was not found");
        }

        return category.Items is not null && category.Items.Any() ? category.Items : Enumerable.Empty<MenuItem>();
    }

    public Task UpdateCategoryAsync(Guid categoryId, Category updatedCategory)
    {
        throw new NotImplementedException();
    }
}

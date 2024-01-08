using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trofi.io.Shared;

namespace Trofi.io.Server.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly ILogger<CategoriesController> _logger;
    private readonly AppDbContext _context;

    public CategoriesController(ICategoriesRepository categoriesRepository, AppDbContext context, ILogger<CategoriesController> logger)
    {
        _categoriesRepository = categoriesRepository;
        _context = context;
        _logger = logger;
    }

    [HttpGet("get-categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _categoriesRepository.GetCategoriesAsync();
        var categoriesAsDtos = categories.Select(c => c.ToCategorySummaryDto());

        return Ok(new ApiResponse<IEnumerable<CategorySummaryDto>>
        {
            Message = "Categories retrieved successfully",
            Body = categoriesAsDtos,
            IsSuccess = true
        });
    }

    [HttpGet("get-category/{id}")]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        try
        {
            var category = await _categoriesRepository.GetCategoryAsync(id);
            var categoryAsDto = category.ToCategoryDto();

            return Ok(new ApiResponse<CategoryDto>
            {
                Message = "Category retrieved successfully",
                Body = categoryAsDto,
                IsSuccess = true
            });

        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiErrorResponse
            {
                ErrorMessage = ex.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _categoriesRepository.CreateCategoryAsync(model.ToCategoryCreate());
                return Ok(new ApiResponse
                {
                    Message = $"{model.Name} category was created successfully",
                    IsSuccess = true
                });
            }
            catch (ResourceCreationFailedException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    ErrorMessage = ex.Message
                });
            }
        }
        else
        {
            return BadRequest(ModelState);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        try
        {
            await _categoriesRepository.DeleteCategoryAsync(id);

            return Ok(new ApiResponse
            {
                Message = "The category was deleted successfully",
                IsSuccess = true
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiErrorResponse
            {
                ErrorMessage = ex.Message
            });
        }
    }

    [HttpPut("update-category/{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryDto model)
    {
        if (ModelState.IsValid)
        {
            var categoryToUpdate = await _context.Categories.FindAsync(id);

            if (categoryToUpdate is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    ErrorMessage = "The category you're trying to update was not found"
                });
            }

            categoryToUpdate.Name = model.Name;
            categoryToUpdate.Description = model.Description;
            categoryToUpdate.LogoUrl = model.LogoUrl;

            await _context.SaveChangesAsync();
            _context.Dispose();

            return Ok(new ApiResponse
            {
                Message = "The category was updated successfully",
                IsSuccess = true
            });
        }
        else
        {
            return BadRequest(ModelState);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Trofi.io.Shared;

namespace Trofi.io.Server.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly ILogger<FilesController> _logger;
    private readonly IWebHostEnvironment _webHostEnv;
    private readonly IFilesRepository _filesRepository;
    public FilesController
    (
        ILogger<FilesController> logger,
        IWebHostEnvironment webHostEnv,
        IFilesRepository filesRepository)
    {
        _logger = logger;
        _webHostEnv = webHostEnv;
        _filesRepository = filesRepository;
    }

    private static IEnumerable<string> allowedFileExtensions = new List<string>(3)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    /// <summary>
    /// This endpoint allows mods to upload an image to the gallery of a specific dish
    /// providing it's id, and the chosen file.
    /// The file must have a compatible extension, otherwise, it'll be rejected
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost("upload-dish-image/{dishId}")]
    public async Task<IActionResult> UploadDishImage([FromForm] IFormFile file, Guid dishId)
    {
        if (file is null)
        {
            return BadRequest("The file is required");
        }

        var fileExtension = Path.GetExtension(file.FileName);

        // check if the file extension is not allowed
        if (!allowedFileExtensions.Contains(fileExtension))
        {
            return BadRequest($"The file extension ({fileExtension}) is not allowed");
        }

        try
        {
            var newFileName = $"{Guid.NewGuid()}{fileExtension}";
            var imageFilePath = Path.Combine(_webHostEnv.WebRootPath, "images", newFileName);

            using (var fileStream = new FileStream(imageFilePath, FileMode.Create))
            {
                // Save the new file to the file system inside of wwwroot/images
                file.CopyTo(fileStream);

                var url = Url.Content($"{imageFilePath}"); // the ~/file is the relative path

                await _filesRepository.AddDishImageAsync(imageUrl: url, dishId);
                _logger.LogInformation($"A new file was successfully uploaded: File URL => {newFileName}");

                return Ok(new { Url = url });
            }
        }
        catch (Exception ex)
        {
            // return Internal Server Error (500) with the exception message
            _logger.LogError("An attempt to upload a file failed");
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// This method gets an image based on its ID so the user can view it in full screen
    /// </summary>
    /// <param name="imageId">The id of the image to get</param>
    /// <returns></returns>
    [HttpGet("image/{id}")]
    public async Task<IActionResult> GetDishImageById(Guid imageId)
    {
        try
        {
            var dishImage = await _filesRepository.GetDishImageAsync(imageId);

            var imageAsDto = dishImage.ToDishImageDto();

            return Ok(new ApiResponse<ImageDto>
            {
                Message = "Image retrieved successfully",
                Body = imageAsDto,
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

    /// <summary>
    /// This endpoint deletes a message from the database and the file system
    /// </summary>
    /// <param name="imageId">the id of the image to delete</param>
    /// <returns></returns>
    [HttpDelete("delete-image/{id}")]
    public async Task<IActionResult> DeleteImage(Guid imageId)
    {
        try
        {
            await _filesRepository.DeleteDishImageAsync(imageId);

            return Ok(new ApiResponse
            {
                Message = "Image deleted successfully",
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
}

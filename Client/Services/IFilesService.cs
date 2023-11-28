using Microsoft.AspNetCore.Http;

namespace Trofi.io.Client.Services;

public interface IFilesService
{
    Task<ApiResponse<ImageDto>> UploadFileAsync(IFormFile file, Guid dishId);
    Task<ApiResponse<ImageDto>> GetImageAsync(Guid imageId);
    Task<ApiResponse> RemoveImageAsync(Guid imageId);
}

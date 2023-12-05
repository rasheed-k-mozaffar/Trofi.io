using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Trofi.io.Client.Services;

public class FilesService : IFilesService
{
    private const string BaseUrl = "/api/files";

    #region  Injected Dependencies
    private readonly HttpClient _httpClient;

    public FilesService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    #endregion

    public async Task<ApiResponse<ImageDto>> GetImageAsync(Guid imageId)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/image/{imageId}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new DataRetrievalException(message: error!.ErrorMessage!);
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ImageDto>>();
        return result!;
    }

    public async Task<ApiResponse> RemoveImageAsync(Guid imageId)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/delete-image/{imageId}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new OperationFailureException(message: error!.ErrorMessage!);
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        return result!;
    }

    public async Task<ApiResponse<ImageDto>> UploadFileAsync(IFormFile file, Guid dishId)
    {
        var formData = new MultipartFormDataContent();
        var streamContent = new StreamContent(file.OpenReadStream());
        formData.Add(streamContent, "file", file.FileName);

        var response = await _httpClient.PostAsync($"{BaseUrl}/upload-dish-image/{dishId}", formData);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new OperationFailureException(message: error!.ErrorMessage!);
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ImageDto>>();
        return result!;
    }

    public async Task<ApiResponse<IEnumerable<ImageDto>>> GetDishImagesAsync(Guid dishId)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/dish-images/{dishId}");

        if (!response.IsSuccessStatusCode)
        {
            throw new DataRetrievalException(message: "Something went wrong while attempting to load the images");
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<ImageDto>>>();
        return result!;
    }
}
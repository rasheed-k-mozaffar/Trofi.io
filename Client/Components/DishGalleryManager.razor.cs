
using Microsoft.AspNetCore.Components.Forms;
using Trofi.io.Client.Extensions;

namespace Trofi.io.Client.Components;

public partial class DishGalleryManager : ComponentBase
{
    #region File Controls
    private const int MaxAllowedFileSize = 1024 * 1024 * 5;
    private readonly string[] _allowedFileExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    #endregion

    #region Injected Dependecies

    [Inject]
    public NavigationManager Nav { get; set; } = default!;

    [Inject] public IFilesService FilesService { get; set; } = default!;

    #endregion

    [Parameter] public Guid DishId { get; set; }
    [Parameter] public EventCallback OnCloseClicked { get; set; }

    #region Variables
    ApiResponse<IEnumerable<ImageDto>> response = new();
    List<ImageDto> images = new();

    bool isLoadingData = true;
    bool isMakingRequest = false;
    string errorMessage = string.Empty;
    #endregion

    #region Methods
    protected override async Task OnParametersSetAsync()
    {
        errorMessage = string.Empty;

        try
        {
            response = await FilesService.GetDishImagesAsync(DishId);

            if (response.IsSuccess)
            {
                images = response.Body!.ToList();
                // reverse them such that they appear in the order in which they were uploaded in
                images.Reverse();
            }
        }
        catch (DataRetrievalException ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isLoadingData = false;
        }
    }

    private void CloseButtonClicked()
    {
        OnCloseClicked.InvokeAsync();
    }
    #endregion

    #region Image Methods 
    private async Task HandleImageRemovalAsync(Guid imageId)
    {
        isMakingRequest = true;
        errorMessage = string.Empty;

        try
        {
            var result = await FilesService.RemoveImageAsync(imageId);

            if (result.IsSuccess)
            {
                // look up the image in the in memory collection to remove it from the client without refreshing
                var imageToRemove = images.First(i => i.Id == imageId);
                images.Remove(imageToRemove);
            }
        }
        catch (OperationFailureException ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isMakingRequest = false;
        }
    }

    private async Task AddNewDishImageAsync(InputFileChangeEventArgs eventArgs)
    {
        isMakingRequest = true;
        errorMessage = string.Empty;

        try
        {
            // check the uploaded file for safety reasons
            var fileExtension = Path.GetExtension(eventArgs.File.Name);

            // checking the file size
            if (eventArgs.File.Size > MaxAllowedFileSize)
            {
                errorMessage = "The selected image's size exceeded the maximum allowed size";
                isMakingRequest = false;
                return;
            }

            // checking the file extension
            if (!_allowedFileExtensions.Contains(fileExtension))
            {
                errorMessage = "The selected image has an unsupported file format";
                isMakingRequest = false;
                return;
            }

            var formFile = await eventArgs.File.CovertToIFormFileAsync(MaxAllowedFileSize);

            var newGalleryImage = await FilesService
                                        .UploadFileAsync(formFile, DishId);

            images.Add(newGalleryImage.Body!);
            StateHasChanged();
        }
        catch (OperationFailureException ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isMakingRequest = false;
        }
    }
    #endregion
}

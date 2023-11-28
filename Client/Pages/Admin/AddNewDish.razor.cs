using System.Net.Security;
using Microsoft.AspNetCore.Components.Forms;
using Trofi.io.Client.Extensions;

namespace Trofi.io.Client.Pages.Admin;

public partial class AddNewDish : ComponentBase
{
    #region File Controls
    private const int MaxAllowedFileSize = 1024 * 1024 * 5;
    private readonly string[] _allowedFileExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    #endregion

    #region  Injected Dependencies
    [Inject]
    public NavigationManager Nav { get; set; } = default!;

    [Inject]
    public IMenuService MenuService { get; set; } = default!;

    [Inject]
    public IFilesService FilesService { get; set; } = default!;

    #endregion

    #region Variables
    DishCreateDto createDishRequest = new() { Id = Guid.NewGuid() };
    List<ImageDto> dishGallery = new();
    MenuItemDto previewObject = new();

    bool imageUploadedLimitExceeded => dishGallery.Count > 10;

    bool isMakingRequest = false;
    string errorMessage = string.Empty;
    int currentStep = 1;
    float barWidth;
    #endregion

    #region Methods
    private async Task HandleDishCreationAsync()
    {
        errorMessage = string.Empty;
        isMakingRequest = true;

        try
        {
            var result = await MenuService.AddItemAsync(createDishRequest);

            if (result.IsSuccess)
            {
                currentStep++;
                ModifyBarWidth();
            }
        }
        catch (ResourceCreationFailedException ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            // reset the state variables
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
                                        .UploadFileAsync(formFile, createDishRequest.Id);

            dishGallery.Add(newGalleryImage.Body!);
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

    private async Task HandleImageRemovalAsync(Guid imageToDeleteId)
    {
        isMakingRequest = true;
        errorMessage = string.Empty;

        try
        {
            var result = await FilesService.RemoveImageAsync(imageToDeleteId);

            if (result.IsSuccess)
            {
                // look up the image in the in memory collection to remove it from the client without refreshing
                var imageToRemove = dishGallery.First(i => i.Id == imageToDeleteId);
                dishGallery.Remove(imageToRemove);
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

    private async Task MoveToPreview()
    {
        errorMessage = string.Empty;
        isMakingRequest = true;


        // force the user to upload 2 images
        if (dishGallery.Count < 2)
        {
            errorMessage = "You need to upload at least two images";
            isMakingRequest = false;
            return;
        }

        // reset the state variables
        errorMessage = string.Empty;
        isMakingRequest = false;

        await LoadPreviewItemAsync();
        currentStep++;
        ModifyBarWidth();
    }

    private async Task LoadPreviewItemAsync()
    {
        isMakingRequest = true;
        errorMessage = string.Empty;

        try
        {
            var result = await MenuService.GetMenuItemByIdAsync(createDishRequest.Id);

            previewObject = result.Body!;
        }
        catch (DataRetrievalException)
        {
            errorMessage = "Failed to load the preview of the newly added dish!";
        }
        finally
        {
            isMakingRequest = false;
        }
    }

    void ModifyBarWidth()
    {
        // check the progress, and convert into a percentage.
        switch (currentStep)
        {
            case 1:
                barWidth = 2.5f;
                break;
            case 2:
                barWidth = 50f;
                break;
            case 3:
                barWidth = 100f;
                break;
            default:
                barWidth = 0.0f;
                break;
        }
    }

    #endregion
}

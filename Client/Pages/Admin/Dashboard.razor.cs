
using Blazored.LocalStorage;

namespace Trofi.io.Client.Pages.Admin;

public partial class Dashboard : ComponentBase
{
    #region Injected Dependencies
    [Inject]
    public NavigationManager Nav { get; set; } = default!;

    [Inject]
    public IMenuService MenuService { get; set; } = default!;

    [Inject]
    public ILocalStorageService LocalStorage { get; set; } = default!;
    #endregion

    #region Variables
    ApiResponse<IEnumerable<MenuItemDto>> apiResponse = new();
    List<MenuItemDto>? menuItems = new();

    Guid selectedDishId;
    bool wantsToDeleteDish = false;
    bool wantsToViewDishGallery = false;
    bool isMakingRequest = false;
    bool isLoadingData = true;
    string errorMessage = string.Empty;
    #endregion

    #region Data Loading Methods
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        try
        {
            apiResponse = await MenuService.GetMenuAsync();

            if (apiResponse.IsSuccess)
            {
                menuItems = apiResponse.Body?.ToList() ?? null;
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
    #endregion

    #region Delete Dishes Methods
    private void OpenDeleteDishConfirmationDialog(Guid dishId)
    {
        wantsToDeleteDish = true;
        selectedDishId = dishId;
    }

    private void CloseDeleteDishConfirmationDialog()
    {
        wantsToDeleteDish = false;
        selectedDishId = default;
    }

    private async Task HandleDishDeletionAsync()
    {
        isMakingRequest = true;
        errorMessage = string.Empty;

        try
        {
            var response = await MenuService.RemoveItemAsync(selectedDishId);

            if (response.IsSuccess)
            {
                // delete the item from the in memory collection
                var itemToDelete = menuItems!.First(i => i.Id == selectedDishId);
                menuItems!.Remove(itemToDelete);
                StateHasChanged();

                // reset the delete dish related variables
                wantsToDeleteDish = false;
                selectedDishId = default;
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
    #endregion

    #region Dish Gallery Methods
    private void OpenDishGallery(Guid dishId)
    {
        // resetting it just to not have conflicts occuring
        wantsToDeleteDish = false;
        selectedDishId = dishId;
        wantsToViewDishGallery = true;
    }

    private void CloseDishGallery()
    {
        wantsToViewDishGallery = false;
        selectedDishId = default;
    }

    private async Task GoToEditDishAsync(Guid dishId, MenuItemDto item)
    {
        await LocalStorage.SetItemAsync<MenuItemDto>("item_to_edit", item);
        Nav.NavigateTo($"/admin/edit-dish/{dishId}");
    }
    #endregion
}

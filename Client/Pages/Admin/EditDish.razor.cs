
using System.Drawing;
using Blazored.LocalStorage;
using Microsoft.VisualBasic;

namespace Trofi.io.Client.Pages.Admin;

public partial class EditDish : ComponentBase
{
    #region Injected Dependencies
    [Inject]
    public IMenuService MenuService { get; set; } = default!;

    [Inject]
    public NavigationManager Nav { get; set; } = default!;

    [Inject]
    public ILocalStorageService LocalStorage { get; set; } = default!;
    #endregion

    #region Variables
    [Parameter] public Guid DishId { get; set; }

    bool wantsToDiscard = false;


    MenuItemDto? itemToEdit;
    DishEditDto editedItem = new();
    bool isLoadingDishData = true;
    bool isMakingRequest = false;
    string errorMessage = string.Empty;
    #endregion


    #region Methods
    /// <summary>
    /// When the page gets initialized, the item can be loaded from the local storage
    /// instead of making an HTTP request to the server to get the item
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        if (await LocalStorage.ContainKeyAsync("item_to_edit"))
        {
            itemToEdit = await LocalStorage.GetItemAsync<MenuItemDto>("item_to_edit");
            editedItem = MapItemDetails(itemToEdit);
            isLoadingDishData = false;
        }
    }

    private async Task HandleItemUpdateAsync()
    {
        isMakingRequest = true;
        errorMessage = string.Empty;
        try
        {
            var response = await MenuService.UpdateItemAsync(DishId, editedItem);

            if (response.IsSuccess)
            {
                Nav.NavigateTo("/admin/dashboard");
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

    private async Task DiscardChangesAsync()
    {
        await LocalStorage.RemoveItemAsync("item_to_edit");
        Nav.NavigateTo("/admin/dashboard");
    }
    #endregion

    #region Helper methods
    private DishEditDto MapItemDetails(MenuItemDto item)
    {
        return new DishEditDto()
        {
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            UpdatedPrice = item.UpdatedPrice,
            IsAvailable = item.IsAvailable,
            IsSpecial = item.IsAvailable,
        };
    }
    #endregion
}


using Trofi.io.Client.States;

namespace Trofi.io.Client.Pages;

public partial class Index : ComponentBase
{
    #region Injected Dependencies

    [Inject]
    public IMenuService MenuService { get; set; } = default!;

    [Inject]
    public ICartService CartService { get; set; } = default!;

    [Inject]
    public CartState CartState { get; set; } = default!;

    #endregion

    #region Variables

    ApiResponse<IEnumerable<MenuItemDto>> apiResponse = new();
    IEnumerable<MenuItemDto> menu = new List<MenuItemDto>();

    bool isLoadingMenuData = true;
    string errorMessage = string.Empty;

    #endregion

    protected override async Task OnInitializedAsync()
    {
        try
        {
            apiResponse = await MenuService.GetMenuAsync();

            if (apiResponse.IsSuccess)
            {
                menu = apiResponse.Body!;
            }
        }
        catch (DataRetrievalException ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isLoadingMenuData = false;
        }
    }

    #region  Cart Methods
    private async Task AddItemToCartAsync(Guid itemId)
    {
        errorMessage = string.Empty;

        try
        {
            var response = await CartService
                                .AddProductToCartAsync(new AddToCartRequest()
                                {
                                    ProductId = itemId
                                });
        }
        catch (OperationFailureException ex)
        {
            errorMessage = ex.Message;
        }
    }
    #endregion
}

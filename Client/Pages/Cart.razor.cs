using Trofi.io.Client.Components;
using Trofi.io.Client.States;

namespace Trofi.io.Client.Pages;

public partial class Cart : ComponentBase
{
    #region Injected Dependencies

    [Inject]
    public NavigationManager Nav { get; set; } = default!;

    [Inject]
    public CartState CartState { get; set; } = default!;

    [Inject]
    public ICartService CartService { get; set; } = default!;

    #endregion

    #region Variables
    bool isLoadingCartData = true;
    bool isPerformingNetworkRequest = false; // used to manage state while an operation is undergoing 
    List<CartItemDto> items = new();

    string errorMessage = string.Empty;
    #endregion

    #region Methods

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var response = await CartService.GetCartItemsAsync();

            if (response.IsSuccess)
            {
                items = response.Body!.ToList();
            }
        }
        catch (DataRetrievalException ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isLoadingCartData = false;
        }
    }

    private async Task ClearCartAsync()
    {
        isPerformingNetworkRequest = true;
        errorMessage = string.Empty;

        try
        {
            var response = await CartService.ClearCartAsync();

            items.Clear();
            CartState.CartItemsCount = 0;
            CartState.NotifyStateChanged();
        }
        catch (OperationFailureException ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isPerformingNetworkRequest = false;
        }
    }

    #endregion

    #region Cart Item Controls

    private async Task IncrementItemQuantityAsync(CartItemDto item)
    {
        isPerformingNetworkRequest = true;
        errorMessage = string.Empty;

        try
        {
            var response = await CartService.UpdateCartProductQuantityAsync(item.Id, ++item.Quantity);

            item.Quantity++; // increment the quantity on the client
        }
        catch (OperationFailureException ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isPerformingNetworkRequest = false;
            StateHasChanged();
        }
    }

    private async Task DecrementItemQuantityAsync(CartItemDto item)
    {
        isPerformingNetworkRequest = true;
        errorMessage = string.Empty;

        try
        {
            // if the new quantity is 0, just remove the whole item from the cart
            if (item.Quantity - 1 == 0)
            {
                await RemoveItemFromCartAsync(item);
            }
            else
            {
                var response = await CartService.UpdateCartProductQuantityAsync(item.Id, --item.Quantity);
                item.Quantity--; // decrement the quantity on the client
            }
        }
        catch (OperationFailureException ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isPerformingNetworkRequest = false;
            StateHasChanged();
        }
    }

    private async Task ChangeItemQuantityAsync(CartItemDto item)
    {
        isPerformingNetworkRequest = true;
        errorMessage = string.Empty;

        try
        {
            // if the new quantity is 0, just remove the whole item from the cart
            if (item.Quantity <= 0)
            {
                await RemoveItemFromCartAsync(item);
            }
            else
            {
                var response = await CartService.UpdateCartProductQuantityAsync(item.Id, item.Quantity);
            }
        }
        catch (OperationFailureException ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isPerformingNetworkRequest = false;
            StateHasChanged();
        }
    }

    private async Task RemoveItemFromCartAsync(CartItemDto item)
    {
        isPerformingNetworkRequest = true;
        errorMessage = string.Empty;

        try
        {
            var response = await CartService.RemoveProductFromCartAsync(item.Id);

            // remove the item from the client in-memory collection
            items.Remove(item);
        }
        catch (OperationFailureException ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isPerformingNetworkRequest = false;
        }
    }


    #endregion
}

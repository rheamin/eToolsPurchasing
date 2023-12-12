using Microsoft.AspNetCore.Components;

namespace eToolsWebApp.Components;

public partial class SalesNav
{
    [Inject] 
    protected NavigationManager NavManager { get; set; }

    private void ToShopping()
    {
        NavManager.NavigateTo("/sales/shopping");
    }

    private void ToCart()
    {
        NavManager.NavigateTo("/sales/cart");
    }

    private void ToCheckout()
    {
        NavManager.NavigateTo("/sales/checkout");
    }
}
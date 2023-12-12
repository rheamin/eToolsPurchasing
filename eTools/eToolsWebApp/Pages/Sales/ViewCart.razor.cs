using eToolsWebApp.Data;
using Microsoft.AspNetCore.Components;
using SalesSystem.Models;

namespace eToolsWebApp.Pages.Sales;

public partial class ViewCart
{
    [Inject] protected AppState State { get; set; }

    private decimal PriceTotal;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (State.Cart == null)
        {
            State.Cart = new List<ShoppingCartView>();
        }
        
        UpdateTotal();
        
    }
    
    private void UpdateTotal()
    {
        // reset price
        decimal total = 0;
        // add up items
        foreach (var item in State.Cart)
        {
            total += item.SellingPrice * item.Quantity;
        }

        PriceTotal = total;
    }

    private void RemoveItem(ShoppingCartView item)
    {
        State.Cart.Remove(item);
    }
}
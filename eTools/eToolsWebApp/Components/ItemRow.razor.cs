using Microsoft.AspNetCore.Components;
using SalesSystem.Models;

namespace eToolsWebApp.Components;

public partial class ItemRow
{
    [Parameter] public StockItemView? StockItem { get; set; }
    [Parameter] public EventCallback<ShoppingCartView> OnAdd { get; set; }

    public int OrderQuantity;


    private void OnAddClicked()
    {
        if (OrderQuantity > 0)
        {
            // create shopping cart view
            var cartItem = new ShoppingCartView
            {
                StockItemID = StockItem.StockItemID,
                Description = StockItem.Description,
                Quantity = OrderQuantity,
                SellingPrice = StockItem.SellingPrice
            };
            
            OnAdd.InvokeAsync(cartItem);
        }
        
    }
}
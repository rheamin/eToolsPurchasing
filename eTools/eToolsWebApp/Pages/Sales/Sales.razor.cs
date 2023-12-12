using eToolsWebApp.Data;
using Microsoft.AspNetCore.Components;
using SalesSystem.BLL;
using SalesSystem.Models;
using MudBlazor;

namespace eToolsWebApp.Pages.Sales;

public partial class Sales
{

    // private fields
    private int _categoryid;
    private bool _invalidOrderQuantity;
    
    [Inject]
    protected SalesService _salesService { get; set; }
    
    [Inject]
    protected AppState State { get; set; }
    
    private List<CategoryView> Categories { get; set; }
    
    private List<StockItemView> StockItems { get; set; }
    
    // selected category
    private int CategoryID
    {
        get
        {
            return _categoryid;
        }
        set
        {
            _categoryid = value;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        try
        {
            // initialize cart 
            State.Cart = new List<ShoppingCartView>();
            _invalidOrderQuantity = false;
            Categories = _salesService.GetCategories();
            StockItems = _salesService.GetItemsByCategoryID(CategoryID);
        }
        catch (ArgumentException)
        {
            
        }
        
    }

    private async void OnCatChanged(int catid)
    {
        // change category id
        CategoryID = catid;
        StockItems = LoadItems(CategoryID);
    }

    private List<StockItemView> LoadItems(int cat)
    {
        return _salesService.GetItemsByCategoryID(CategoryID);
    }
    private void OnAddItem(ShoppingCartView cartItem)
    {
        _invalidOrderQuantity = false;
        // check item quantity 
        if (cartItem.Quantity > StockItems
                .First(si => si.StockItemID == cartItem.StockItemID).QuantityOnHand)
        {
            _invalidOrderQuantity = true;
        }
        else
        {
            
            var item = State.Cart.SingleOrDefault(ci => ci.StockItemID == cartItem.StockItemID);
                
            // add 
            if (item == null)
            {
                State.Cart.Add(cartItem);
            }
            // update 
            else
            {
                item.Quantity += cartItem.Quantity;
            }
        }
    }
    
}
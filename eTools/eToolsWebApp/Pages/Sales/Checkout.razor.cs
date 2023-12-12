using eToolsWebApp.Data;
using Microsoft.AspNetCore.Components;
using SalesSystem.BLL;
using SalesSystem.Models;

namespace eToolsWebApp.Pages.Sales;

public partial class Checkout
{
    private string _coupon;
    private int _discount;
    private decimal _discountAmmount;
    
    [Inject] protected AppState State { get; set; }
    [Inject] protected SalesService Service { get; set; }

    private String Coupon
    {
        get
        {
            return _coupon;
        }
        set
        {
            // get coupon id
            var id = Service.GetCouponID(value);
            if (id != 0) // set coupon id
            {
                Sale.CouponID = id;
                
                // get coupon discount
                _discount = Service.GetCouponValue(id);
                _discountAmmount = Sale.SubTotal * (_discount / 100.0m);
            }
            
            _coupon = value;
        }
    }

    private SaleView Sale { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        try
        {
            if (State.Cart == null)
            {
                State.Cart = new List<ShoppingCartView>();
            }

            _discount = 0;
            _discountAmmount = 0.0m;
            
            // initialize sale
            Sale = new SaleView();
            Sale.EmployeeID = 1;
            Sale.Items = State.Cart;
            Sale.SubTotal = State.Cart.Sum(i => i.SellingPrice * i.Quantity);
            Sale.TaxAmount = Sale.SubTotal * 0.05m;
            Sale.PaymentType = "C";


        }
        catch (Exception err)
        {
            
        }
        
    }

    private void ProcessSale()
    {
        Service.SaveSale(Sale);
    }
}
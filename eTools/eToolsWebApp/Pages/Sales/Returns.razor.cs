using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SalesSystem.BLL;
using SalesSystem.Models;

namespace eToolsWebApp.Pages.Sales;

public partial class Returns
{
    [Inject] protected SalesService Service { get; set; }

    private EditContext editContext;
    private ValidationMessageStore messageStore;
    
    private int invoiceNumber;
    private ReturnSaleView sale;

    private int? refundID;

    private int discount;
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        try
        {
            sale = new ReturnSaleView();
            sale.ReturnSaleDetails = new List<ReturnSaleDetailCartView>();
            
            // edit context
            editContext = new EditContext(sale);
            editContext.OnValidationRequested += HandleValidationRequest;
            messageStore = new ValidationMessageStore(editContext);
            editContext.OnFieldChanged += OnFieldChanged;
            
        }
        catch (Exception err)
        {
            
        }
    }
    
    #region form methods

    private void OnFieldChanged(object sender, FieldChangedEventArgs e)
    {
        if (editContext.Validate())
        {
            UpdateTotal();
        }
        
    }

    private void HandleValidationRequest(object sender, ValidationRequestedEventArgs e)
    {
        // verify return qty
        foreach (var item in sale.ReturnSaleDetails)
        {
            if (item.QtyReturnNow > (item.OriginalQty - item.PreviouReturnQty))
            {
                messageStore.Add(new FieldIdentifier(item, nameof(item.QtyReturnNow)),"Return quantity invalid");
            }
        }
        
        editContext.NotifyValidationStateChanged();
    }
    
    #endregion

    private void LookupInvoice()
    {
        try
        {
            sale = Service.GetSaleByID(invoiceNumber);
            discount = Service.GetCouponValue(sale.CouponID ?? 0);
        }
        catch (KeyNotFoundException err)
        {
            
        }
    }

    private void Clear()
    {
        invoiceNumber = 0;
        sale = new ReturnSaleView();
        sale.ReturnSaleDetails = new List<ReturnSaleDetailCartView>();
    }

    private void Refund()
    {
        try
        {
            Service.Refund(sale);
        }
        catch (AggregateException err)
        {
            
        }
    }
    
    private void UpdateTotal()
    {
        sale.SubTotal = sale.ReturnSaleDetails.Sum(rsd => rsd.QtyReturnNow * rsd.SellingPrice);
        sale.TaxAmount = sale.SubTotal * (decimal)0.05;
    }
 
}
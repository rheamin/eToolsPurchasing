#nullable disable
using Microsoft.AspNetCore.Components;
using PurchasingSystem.BLL;
using PurchasingSystem.Entities;
using PurchasingSystem.ViewModels;
using MudBlazor;
using ReceivingSystem.Entities;
using SalesSystem.Entities;

namespace eToolsWebApp.Pages.Purchasing
{
    public partial class Purchasing
    {
        #region Fields and Properties
        [Inject]
        protected VendorService VendorService { get; set; }

        [Inject]
        protected PurchasingService PurchasingService { get; set; }

        [Inject]
        protected EmployeeService EmployeeService { get; set; }

        private string errorMessage { get; set; }

        private List<string> errorDetails { get; set; } = new();

        private string feedBack { get; set; }

        private string errorDetailsDisplay { get; set; }
        private List<VendorView> VendorList { get; set; } = new List<VendorView>();

        private EmployeeView currentUser { get; set; }
        private decimal subTotal { get; set; }
        private decimal GST { get; set; }
        private decimal total { get; set; }
        private ItemView item { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            VendorList = VendorService.GetVendors();
            currentUser = EmployeeService.GetEmployee(1);
            
        }

        private int vendorID { get; set; }

        private List<PurchaseOrderEditView> purchaseOrders { get; set; } = new List<PurchaseOrderEditView>();

        private List<ItemView> itemsOnOrder { get; set;  } = new List<ItemView>();
        private List<ItemView> itemsNotOnOrder { get; set; } = new List<ItemView>();
        private List<ItemView> vendorItems { get; set; } = new List<ItemView>();

        private PurchaseOrderEditView purchaseOrderEditView { get; set; }
        private ItemDetailView itemDetailView { get; set; } = new ItemDetailView();

        private void Recalculate(ItemView item)
        {
            itemsOnOrder.Remove(item);
            itemsOnOrder.Add(item);
        }

        private void refreshCost()
        {
            subTotal = decimal.Round(itemsOnOrder.Sum(x => x.Price * x.QTO), 2);
            GST = decimal.Round(itemsOnOrder.Sum(x => x.Price * x.QTO) * 0.05m, 2);
            total = decimal.Round(itemsOnOrder.Sum(x => x.Price * x.QTO) * 1.05m, 2);
        }

        private void Clear()
        {
            vendorID = 0;
            itemsOnOrder = new List<ItemView>();  
            itemsNotOnOrder = new List<ItemView>();
            subTotal = 0;
            GST = 0;
            total = 0;
        }

        private void PurchaseOrderLogic(int vendorID)
        {
            try
            {
                errorMessage = "";
                errorDetails.Clear();
                PurchaseOrderView order = PurchasingService.GetVendorPurchaseOrder(vendorID);
                vendorItems = PurchasingService.FetchInventoryBy(vendorID);

                if (order == null)
                {
                    purchaseOrderEditView = new PurchaseOrderEditView();
                    purchaseOrderEditView.EmployeeID = currentUser.EmployeeID;
                    purchaseOrderEditView.ItemDetails = new List<ItemDetailView>();

                    foreach (var item in vendorItems)
                    {
                        if (item.QTO > 0)
                        {
                            itemsOnOrder.Add(item);
                            itemDetailView.StockItemID = item.StockItemID;
                            itemDetailView.QTO = item.QTO;
                            itemDetailView.Price = item.Price;
                            purchaseOrderEditView.ItemDetails.Add(itemDetailView);
                            purchaseOrderEditView.VendorID = vendorID;
                        }
                        else
                        {
                            itemsNotOnOrder.Add(item);
                        }
                    }
                    PurchasingService.UpdatePurchaseOrder(purchaseOrderEditView, false);
                    refreshCost();
                }
                else
                {
                    purchaseOrderEditView = PurchasingService.generateEditView(order);
                    purchaseOrderEditView.EmployeeID = currentUser.EmployeeID;

                    foreach (var item in vendorItems)
                    {
                        if (item.QTO > 0)
                        {
                            if (!itemsOnOrder.Contains(item))
                            {
                                itemsOnOrder.Add(item);
                                itemDetailView.StockItemID = item.StockItemID;
                                itemDetailView.QTO = item.QTO;
                                itemDetailView.Price = item.Price;
                                purchaseOrderEditView.ItemDetails.Add(itemDetailView);
                            }
                        }
                        else
                        {
                            if (!itemsNotOnOrder.Contains(item))
                            {
                                itemsNotOnOrder.Add(item);
                            }
                        }
                    }
                    PurchasingService.UpdatePurchaseOrder(purchaseOrderEditView, false);
                    refreshCost();
                }

            }
            catch (ArgumentNullException ex)
            {
                errorMessage = "Unable to update purchase order:";
                errorMessage += GetInnerException(ex).Message;
            }
            catch (ArgumentException ex)
            {
                errorMessage = GetInnerException(ex).Message;
            }
            catch (AggregateException ex)
            {
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }

        private void placeOrderLogic()
        {
            feedBack = "";
            try
            {
                PurchasingService.UpdatePurchaseOrder(purchaseOrderEditView, true);
                feedBack = "Order placed sucessfully";
            }
            catch (ArgumentNullException ex)
            {
                errorMessage = "Unable to update purchase order:";
                errorMessage += GetInnerException(ex).Message;
            }
            catch (ArgumentException ex)
            {
                errorMessage = GetInnerException(ex).Message;
            }
            catch (AggregateException ex)
            {
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }

        public void AddItemToPurchaseOrderLine(ItemView item)
        {
            PurchaseOrderView purchaseOrderView = PurchasingService.GetVendorPurchaseOrder(vendorID);
            purchaseOrderView.Items.Add(item);
            itemsOnOrder.Add(item);
            itemsNotOnOrder.Remove(item);
        }

        // see the README for method change justification
        public void RemovePurchaseOrderLine(ItemView item)
        {
            PurchaseOrderView purchaseOrderView = PurchasingService.GetVendorPurchaseOrder(vendorID);
            purchaseOrderView.Items.Remove(item);
            itemsOnOrder.Remove(item);
            itemsNotOnOrder.Add(item);
        }

        public static Exception GetInnerException(System.Exception ex)
        {
            while (ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }
    }
}

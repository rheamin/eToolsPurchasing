#nullable disable
namespace PurchasingSystem.BLL;
using PurchasingSystem.DAL;
using PurchasingSystem.ViewModels;
using PurchasingSystem.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public class PurchasingService
{
    #region Fields
    private readonly PurchasingContext _purchasingContext;
    #endregion

    internal PurchasingService(PurchasingContext purchasingContext)
    {
        _purchasingContext = purchasingContext;
    }
    public PurchaseOrderView UpdatePurchaseOrder(PurchaseOrderEditView order, bool placeOrder)
    {
        // when you have a new order create a new PO, see if there is any items below the reorder level and add it to the PO
        #region Business Rules
        List<Exception> errorList = new List<Exception>();
        // rule: employee ID must be valid
        if (order.EmployeeID <= 0)
        {
            errorList.Add(new Exception("Please provide a valid employee ID"));
        }
        // rule: vendor ID must be valid
        if (order.VendorID <= 0)
        {
            errorList.Add(new Exception("Please provide a valid vendor ID"));
        }
        #endregion
        #region Main Method Code
        PurchaseOrder purchaseOrder = _purchasingContext.PurchaseOrders
                            .Where(x => x.PurchaseOrderId == order.PurchaseOrderID)
                            .FirstOrDefault();

        if (purchaseOrder == null)
        {
            purchaseOrder = new PurchaseOrder();
        }

        purchaseOrder.EmployeeId = order.EmployeeID;
        purchaseOrder.PurchaseOrderId = order.PurchaseOrderID;
        purchaseOrder.VendorId = order.VendorID;
        purchaseOrder.SubTotal = order.SubTotal;
        purchaseOrder.TaxAmount = order.GST;

        foreach (var item in order.ItemDetails)
        {
            PurchaseOrderDetail purchaseOrderDetails = _purchasingContext.PurchaseOrderDetails
                                                        .Where(x => x.PurchaseOrderDetailId == item.PurchaseOrderDetailID)
                                                        .FirstOrDefault();

            if (purchaseOrderDetails == null)
            {
                purchaseOrderDetails = new PurchaseOrderDetail();
            }

            if (!_purchasingContext.StockItems.Any(x => x.StockItemId == item.StockItemID))
            {
                errorList.Add(new Exception($"The stock item with id ${item.StockItemID} doesnt exist"));
            }

            if (_purchasingContext.StockItems.Where(x => x.StockItemId == item.StockItemID).Any(x => x.QuantityOnHand == 0))
            {
                errorList.Add(new Exception($"The stock item with id ${item.StockItemID} is out of stock"));
            }

            if (_purchasingContext.StockItems.Where(x => x.StockItemId == item.StockItemID).Any(x => x.PurchasePrice == 0))
            {
                errorList.Add(new Exception($"The stock item with id ${item.StockItemID} isn't currently for sale"));
            }

            purchaseOrderDetails.PurchaseOrderId = order.PurchaseOrderID;
            purchaseOrderDetails.StockItemId = item.StockItemID;
            purchaseOrderDetails.Quantity = item.QTO;
            purchaseOrderDetails.PurchasePrice = item.Price;
            purchaseOrderDetails.RemoveFromViewFlag = item.RemoveFromViewFlag;


            if (purchaseOrderDetails.PurchaseOrderDetailId == 0)
            {
                purchaseOrder.PurchaseOrderDetails.Add(purchaseOrderDetails);
            }
            else
            {
                _purchasingContext.PurchaseOrderDetails.Update(purchaseOrderDetails);
            }

        }

        if (purchaseOrder.PurchaseOrderId == 0)
        {
            _purchasingContext.PurchaseOrders.Add(purchaseOrder);
        }

        if (placeOrder)
        {
            purchaseOrder.OrderDate = DateTime.Today;
            foreach (var orderedItem in purchaseOrder.PurchaseOrderDetails)
            {
                StockItem stockItemToUpdate = _purchasingContext.StockItems.Where(x => x.StockItemId == orderedItem.StockItemId).FirstOrDefault();
                stockItemToUpdate.QuantityOnOrder = stockItemToUpdate.QuantityOnOrder + orderedItem.Quantity;
            }
        }

        #region Check for errors and saving of data

        // --- Error handling and saving
        if (errorList.Count > 0)
        {
            _purchasingContext.ChangeTracker.Clear();
            string errorMsg = "Unable to add or edit purchase order.";
            errorMsg += " Please check error message(s)";
            throw new AggregateException(errorMsg, errorList);
        }
        else
        {
            _purchasingContext.SaveChanges();
        }
        return GetVendorPurchaseOrder(purchaseOrder.VendorId);
        #endregion
        #endregion
    }

    public List<ItemView> FetchInventoryBy(int vendorID)
    {
        // fetch inventory including inventory on order
        // parameter validation
        if (vendorID <= 0)
        {
            throw new ArgumentNullException("Please provide a valid vendor ID.");
        }

        return _purchasingContext.StockItems
        .Where(x => x.VendorId == vendorID && !x.RemoveFromViewFlag)
        .Select(x => new ItemView
        {
            PurchaseOrderDetailID = 0,
            StockItemID = x.StockItemId,
            Description = x.Description,
            QOH = x.QuantityOnHand,
            ROL = x.ReOrderLevel,
            QOO = x.QuantityOnOrder,
            QTO = x.ReOrderLevel - (x.QuantityOnHand + x.QuantityOnOrder),
            Price = x.PurchasePrice
        }).ToList();
    }

    public PurchaseOrderEditView generateEditView(PurchaseOrderView purchaseOrderView)
    {
        PurchaseOrderEditView purchaseOrderEditView = new PurchaseOrderEditView();
        purchaseOrderEditView.VendorID = purchaseOrderView.Vendor.VendorID;
        purchaseOrderEditView.PurchaseOrderID = purchaseOrderView.PurchaseOrderID;
        purchaseOrderEditView.SubTotal = purchaseOrderView.SubTotal;
        purchaseOrderEditView.GST = purchaseOrderView.GST;
        purchaseOrderEditView.ItemDetails = new List<ItemDetailView>();

        foreach (var item in purchaseOrderView.Items)
        {
            ItemDetailView itemDetailViewToAdd = new ItemDetailView();
            itemDetailViewToAdd.QTO = item.QTO;
            itemDetailViewToAdd.Price = item.Price;
            itemDetailViewToAdd.PurchaseOrderDetailID = item.PurchaseOrderDetailID;
            itemDetailViewToAdd.StockItemID = item.StockItemID;
            purchaseOrderEditView.ItemDetails.Add(itemDetailViewToAdd);
        }

        return purchaseOrderEditView;
    }

    public PurchaseOrderView GetVendorPurchaseOrder(int vendorID)
    {
        // get all purchase orders for a vendor.
        // doesn't include placed ones

        // parameter validation
        if (vendorID <= 0)
        {
            throw new ArgumentNullException("Please provide a valid vendor ID.");
        }

        return _purchasingContext.PurchaseOrders
        .Where(x => x.VendorId == vendorID && !x.RemoveFromViewFlag && x.OrderDate == null)
        .Select(x => new PurchaseOrderView
        {
            Vendor = _purchasingContext.Vendors
            .Where(x => x.VendorId == vendorID)
            .Select(x => new VendorView
            {
                VendorID = x.VendorId,
                VendorName = x.VendorName,
                Phone = x.Phone,
                Address = x.Address,
                City = x.City,
                Province = x.Province.Description,
                PostalCode = x.PostalCode
            }).FirstOrDefault(),
            PurchaseOrderID = x.PurchaseOrderId,
            SubTotal = x.PurchaseOrderDetails.Sum(x => x.Quantity * x.PurchasePrice),
            GST = x.PurchaseOrderDetails.Sum(x => x.Quantity * x.PurchasePrice) * 0.05m,
            Items = x.PurchaseOrderDetails
            .Select(x => new ItemView
            {
                PurchaseOrderDetailID = x.PurchaseOrderDetailId,
                StockItemID = x.StockItemId,
                Description = x.StockItem.Description,
                QOH = x.StockItem.QuantityOnHand,
                ROL = x.StockItem.ReOrderLevel,
                QOO = x.StockItem.QuantityOnOrder,
                QTO = x.StockItem.ReOrderLevel - (x.StockItem.QuantityOnHand + x.StockItem.QuantityOnOrder),
                Price = x.StockItem.SellingPrice
            }).ToList()
        }).FirstOrDefault();
    }

    public void DeletePurchaseOrder(int purchaseOrderID)
    {
        // delete a purchase order (removefromviewflag)
        List<Exception> errorList = new List<Exception>();

        // parameter validation
        if (purchaseOrderID <= 0)
        {
            throw new ArgumentNullException("Please provide a valid purchase order ID");
        }

        PurchaseOrder purchaseOrderToDelete = _purchasingContext.PurchaseOrders
                                                .Where(x => x.PurchaseOrderId == purchaseOrderID && x.OrderDate == null)
                                                .FirstOrDefault();
        if (purchaseOrderToDelete == null)
        {
            errorList.Add(new Exception("The purchase order doesn't exist or has already been placed"));
        }

        purchaseOrderToDelete.RemoveFromViewFlag = true;

        if (errorList.Count > 0)
        {
            _purchasingContext.ChangeTracker.Clear();
            string errorMsg = "The purchase order couldnt be deleted. ";
            errorMsg += "Please check error message";
            throw new AggregateException(errorMsg, errorList);
        }
        else
        {
            _purchasingContext.SaveChanges();
        }
    }
}

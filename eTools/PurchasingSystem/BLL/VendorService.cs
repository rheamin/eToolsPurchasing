#nullable disable
using PurchasingSystem.DAL;
using PurchasingSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchasingSystem.BLL
{
    public class VendorService
    {
        #region Fields
        private readonly PurchasingContext _purchasingContext;
        #endregion

        internal VendorService(PurchasingContext purchasingContext)
        {
            _purchasingContext = purchasingContext;
        }

        public List<VendorView> GetVendors()
        {
            // get all vendors
            return _purchasingContext.Vendors
            .Where(x => !x.RemoveFromViewFlag)
            .Select(x => new VendorView
            {
                VendorID = x.VendorId,
                VendorName = x.VendorName,
                Phone = x.Phone,
                Address = x.Address,
                City = x.City,
                Province = x.Province.Description,
                PostalCode = x.PostalCode
            }).ToList();
        }

        public List<ItemView> FetchInventoryBy(int PurchaseOrderID, int vendorID)
        {
            // fetches vendor inventory excluding current items on order
            // parameter validation
            if (vendorID <= 0)
            {
                throw new ArgumentNullException("Please provide a valid vendor ID.");
            }
            if (PurchaseOrderID <= 0)
            {
                throw new ArgumentNullException("Please provide a valid purchase order ID.");
            }


            return _purchasingContext.StockItems
                    .Where(x => x.VendorId == vendorID && !x.RemoveFromViewFlag && !x.PurchaseOrderDetails.Any(x => x.PurchaseOrderId == PurchaseOrderID))
                    .Select(x => new ItemView
                    {
                        PurchaseOrderDetailID = 0,
                        StockItemID = x.StockItemId,
                        Description = x.Description,
                        QOH = x.QuantityOnHand,
                        ROL = x.ReOrderLevel,
                        QOO = x.QuantityOnOrder,
                        QTO = x.QuantityOnHand - x.ReOrderLevel,
                        Price = x.PurchasePrice
                    }).ToList();
        }
    }
}

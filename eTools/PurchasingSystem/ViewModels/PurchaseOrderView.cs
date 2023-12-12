using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchasingSystem.ViewModels
{
    public class PurchaseOrderView
    {
        public VendorView Vendor { get; set; }
        public int PurchaseOrderID { get; set; }
        public List<ItemView> Items { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal GST { get; set; }
    }
}

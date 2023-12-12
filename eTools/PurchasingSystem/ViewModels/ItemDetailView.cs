using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchasingSystem.ViewModels
{
    public class ItemDetailView
    {
        public int PurchaseOrderDetailID { get; set; }
        public int StockItemID { get; set; }
        public int QTO { get; set; }
        public decimal Price { get; set; }
        public bool RemoveFromViewFlag { get; set; }
    }
}

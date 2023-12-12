namespace SalesSystem.Models;

public class StockItemView
{
    public int StockItemID { get; set; }
    public decimal SellingPrice { get; set; }
    public string Description { get; set; }
    public int QuantityOnHand { get; set; }
}
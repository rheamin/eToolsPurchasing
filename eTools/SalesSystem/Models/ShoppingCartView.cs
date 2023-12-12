namespace SalesSystem.Models;

public class ShoppingCartView
{
    public int StockItemID { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public decimal SellingPrice { get; set; }
}
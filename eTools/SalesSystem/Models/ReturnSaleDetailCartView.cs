namespace SalesSystem.Models;

public class ReturnSaleDetailCartView
{
    public int StockItemID { get; set; }
    public string Description { get; set; }
    public int OriginalQty { get; set; }
    public decimal SellingPrice { get; set; }
    public int PreviouReturnQty { get; set; }
    public int QtyReturnNow { get; set; }
}
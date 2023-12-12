namespace SalesSystem.Models;

public class ReturnSaleView
{
    public int SaleID { get; set; }
    public int EmployeeID { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal SubTotal { get; set; }
    public int? CouponID { get; set; }
    public List<ReturnSaleDetailCartView> ReturnSaleDetails { get; set; }
}
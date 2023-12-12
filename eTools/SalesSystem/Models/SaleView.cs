namespace SalesSystem.Models;

public class SaleView
{
    public int SaleID { get; set; }
    public int EmployeeID { get; set; }
    public string PaymentType { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal SubTotal { get; set; }
    public int? CouponID { get; set; }
    public List<ShoppingCartView> Items { get; set; }
}
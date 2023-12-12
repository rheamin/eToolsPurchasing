using SalesSystem.DAL;
using SalesSystem.Entities;
using SalesSystem.Models;

namespace SalesSystem.BLL;

public class SalesService
{
    private SalesContext _context;

    public SalesService(SalesContext context)
    {
        _context = context;
    }

    public List<CategoryView> GetCategories()
    {
        return _context
            .Categories
            .Where(c => !c.RemoveFromViewFlag)
            .Select(c => new CategoryView
            {
                CategoryID = c.CategoryId,
                Description = c.Description,
                StockItemCount = c.StockItems.Count
            })
            .ToList();
    }

    public List<StockItemView> GetItemsByCategoryID(int categoryid)
    {
        List<StockItem> items;
        
        if (categoryid == 0)
        {
            items = _context
                .StockItems
                .Where(si => !si.RemoveFromViewFlag)
                .ToList();
        }
        else
        {
            items = _context
                .StockItems
                .Where(si => !si.RemoveFromViewFlag && si.CategoryId == categoryid)
                .ToList();
        }

        return items
            .Select(i => new StockItemView
            {
                StockItemID = i.StockItemId,
                SellingPrice = i.SellingPrice,
                Description = i.Description,
                QuantityOnHand = i.QuantityOnHand
            })
            .ToList();
    }

    public int GetCouponID(string couponTxt)
    {
        var coupon = _context.Coupons
            .SingleOrDefault(c => c.CouponIdvalue == couponTxt &&
                                  c.EndDate >= DateTime.Today);
        int id;
        if (coupon == null)
        {
            id = 0;
        }
        else
        {
            id = coupon.CouponId;
        }
        return id;
    }

    public int GetCouponValue(int id)
    {
        var coupon = _context.Coupons
            .SingleOrDefault(c => c.CouponId == id &&
                                  c.EndDate >= DateTime.Today);
        int value = 0;
        if (coupon != null)
        {
            value = coupon.CouponDiscount;
        }

        return value;
    }

    public void SaveSale(SaleView sale)
    {
        #region rule check

        var errorlist = new List<Exception>();
        
        // rule : must be new sale
        if (sale.SaleID != 0)
        {
            errorlist.Add(new Exception("Cannot edit existing sales"));
        }
        
        // rule : payment type valid
        if ( !(new List<string> { "M", "C", "D" }.Contains(sale.PaymentType)) ) 
        {
            errorlist.Add(new Exception("Payment type must be 'M', 'C', or 'D'"));
        }

        {
            // rule : coupon applied to amount if present
            var itemsTotal = sale.Items.Sum(i => i.Quantity * i.SellingPrice);
            // apply coupon
            itemsTotal -= sale.CouponID == null ? 0.0m : itemsTotal * (GetCouponValue(sale.CouponID.Value)/ 100.00m);
            if (sale.SubTotal != itemsTotal)
            {
                errorlist.Add(new Exception("Subtotal incorrect: did you forget to apply a coupon?"));
            }
        }
        
        // rule : no duplicate items
        if (sale.Items.GroupBy(i => i.StockItemID).Where(i => i.Count() > 1).Any()) 
        {
            errorlist.Add(new Exception("Cannot have duplicate items in cart"));
        }
        
        
        // check all items
        foreach (var item in sale.Items)
        {
            // rule : sales quantity must be less than quantity on hand
            if (item.Quantity > _context.StockItems.Single(si => si.StockItemId == item.StockItemID).QuantityOnHand)
            {
                errorlist.Add(new Exception("Item: '" + item.Description + "' Cannot order more than is on hand"));
            }
		
            // rule : item cannot be deiscontinued
            if (_context.StockItems.Any(si => si.StockItemId == item.StockItemID && si.RemoveFromViewFlag)) 
            {
                errorlist.Add(new Exception("Cannot add discontinued products to cart"));
            }
		
            // rule : quantity must be > 0
            if ( item.Quantity < 1 )
            {
                errorlist.Add(new Exception("Quantity for item: '" + item.Description + "' must be greater than 0"));
            }
        }
        #endregion

        #region method logic

        // add sale
        var addSale = new Sale
        {
            SaleDate = DateTime.Now,
            PaymentType = sale.PaymentType,
            EmployeeId = sale.EmployeeID,
            TaxAmount = sale.TaxAmount,
            SubTotal = sale.SubTotal,
            CouponId = sale.CouponID,
            PaymentToken = Guid.NewGuid()
        };
        // sale details
        foreach (var item in sale.Items)
        {
            var addItem = new SaleDetail
            {
                StockItemId = item.StockItemID,
                SellingPrice = item.SellingPrice,
                Quantity = item.Quantity
            };
            addSale.SaleDetails.Add(addItem);
            
            // update stock quantity
            _context.StockItems
                .Single(si => si.StockItemId == item.StockItemID)
                .QuantityOnHand -= item.Quantity;
        }
        
        _context.Sales.Add(addSale);
        #endregion

        #region save

        if (errorlist.Count > 0)
        {
            _context.ChangeTracker.Clear();
            throw new AggregateException("Could not save sale", errorlist);
        }
        
        // save sale
        _context.SaveChanges();

        #endregion
        
    }

    public ReturnSaleView GetSaleByID(int id)
    {
        ReturnSaleView? sale = _context.Sales.Where(s => s.SaleId == id && !s.RemoveFromViewFlag)
            .Select(s => new ReturnSaleView
            {
                SaleID = s.SaleId,
                EmployeeID = s.EmployeeId,
                CouponID = s.CouponId,
                ReturnSaleDetails = s.SaleDetails
                    .Select(sd => new ReturnSaleDetailCartView
                    {
                        StockItemID = sd.StockItemId,
                        Description = sd.StockItem.Description,
                        OriginalQty = sd.Quantity,
                        SellingPrice = sd.SellingPrice,
                        // query refund details
                        PreviouReturnQty = _context
                            .SaleRefundDetails
                            .Where(srd => srd.StockItemId == sd.StockItemId)
                            .Sum(srd => srd.Quantity)
                    }).ToList()
            })
            .FirstOrDefault();

        if (sale == null)
        {
            throw new KeyNotFoundException("Could not find sale");
        }
        return sale;
    }

    public int Refund(ReturnSaleView refund)
    {

        #region rule check

        var errorList = new List<Exception>();
        
        // rule : sale must exist
        if (!_context.Sales.Any(s => s.SaleId == refund.SaleID))
        {
            errorList.Add(new Exception("Refund must be for existing sales"));
        }

        // rule : refund must have at least 1 item
        if (refund.ReturnSaleDetails.Count == 0)
        {
            errorList.Add(new Exception("Refund must have at least 1 item"));
        }
        
        foreach (var item in refund.ReturnSaleDetails)
        {
            // rule : cannot refund more than originally purchased
            if (item.QtyReturnNow > (item.OriginalQty - item.PreviouReturnQty))
            {
                errorList.Add(new Exception($"{item.Description}: Cannot refund more than was purchased"));
            }
            
        }

        #endregion

        #region method logic

        var refundSale = new SaleRefund();
        refundSale.SaleRefundDate = DateTime.Today;
        refundSale.SaleId = refund.SaleID;
        refundSale.EmployeeId = refund.EmployeeID;
        refundSale.TaxAmount = refund.TaxAmount;
        refundSale.SubTotal = refund.SubTotal;

        _context.SaleRefunds.Add(refundSale);
        #endregion

        if (errorList.Count > 0)
        {
            _context.ChangeTracker.Clear();
            throw new AggregateException("Could not process refund", errorList);
        }

        _context.SaveChanges();
        
        return refundSale.SaleRefundId;
    }
}
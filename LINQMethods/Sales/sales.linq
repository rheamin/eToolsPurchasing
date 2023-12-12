<Query Kind="Program">
  <Connection>
    <ID>5e2d25e3-aa3b-42b0-a465-e8de80a96af2</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Database>eTools2023</Database>
    <Server>localhost</Server>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	try 
	{
		// list categories
		var categories = GetCategories();
		categories.Dump("Categories");
		
		// list items in random category
		var catid = RandCategory();
		var category = GetItemsByCategoryID(catid);
		category.Dump("Items in category: " + catid);
		
		// create new sale
		var cart = GenShoppingCart(5);
		cart.Dump("Shopping Cart");
		
		var sale = new SaleView();
		sale.EmployeeID = 1;
		sale.PaymentType = "C";
		// calc tax and subtotal - tax is 5%
		sale.SubTotal = cart.Sum(c => c.SellingPrice * c.Quantity);
		sale.TaxAmount = sale.SubTotal * 0.05m;
		sale.Items = cart;
		sale.Dump("Sale");
		
		// save sale
		var saleid = SaveSales(sale);
		saleid.Dump("new Sale id");
		
		// refund
		var refund = GetSaleRefund(saleid);
		refund.EmployeeID = 1;
		
		// return 1 of last item on order
		var returnItem = refund.ReturnSaleDetails.First();
		returnItem.QtyReturnNow = 1;
		
		// subtotal
		refund.SubTotal = returnItem.QtyReturnNow * returnItem.SellingPrice;
		refund.TaxAmount = refund.SubTotal * 0.05m;
		
		
		refund.Dump("Before process refund");
		// save refund
		var refundid = SaveRefund(refund);
		
		// dump refund
		SaleRefunds.FirstOrDefault(sr => sr.SaleRefundID == refundid).Dump("After processing refund");
		
		// re add stock quantities
		RestoreStock(cart);
	}
	#region Catch all exceptions
	catch (AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump("Aggregate Error");
		}
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump("Arg null Error");
	}
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump("Error");
	}
	#endregion
	

}
private Exception GetInnerException(Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException;
	return ex;
}
// You can define other methods, fields, classes and namespaces here


#region methods
// from specs
public List<CategoryView> GetCategories() 
{
	return Categories
			.Where(c => !c.RemoveFromViewFlag)
			.Select(c => new CategoryView {
				CategoryID = c.CategoryID,
				Description = c.Description,
				StockItemCount = c.StockItems.Count
			}).ToList();
				
}

public List<StockItemView> GetItemsByCategoryID(int categoryid)
{
	return StockItems
			.Where(si => !si.RemoveFromViewFlag && si.CategoryID == categoryid)
			.Select(si => new StockItemView {
				StockItemID = si.StockItemID,
				SellingPrice = si.SellingPrice,
				Description = si.Description,
				QuantityOnHand = si.QuantityOnHand
			}).ToList();
}

public int SaveSales(SaleView sale) 
{

	#region rule check

	var errorlist = new List<Exception>();
	
	// rule : must be new sale
	if (sale.SaleID != 0)
	{
		errorlist.Add(new Exception("Cannot edit existing sales"));
	}

	// rule : sales type must be 'M', 'C', or 'D'
	if ( !(new List<string> { "M", "C", "D" }.Contains(sale.PaymentType)) ) 
	{
		errorlist.Add(new Exception("Payment type must be 'M', 'C', or 'D'"));
	}
	
	// rule : no duplicate items
	if (sale.Items.GroupBy(i => i.StockItemID).Where(i => i.Count() > 1).Any())
	{
		errorlist.Add(new Exception("Cannot have duplicate items in cart"));
	}
	
	// rule : coupon applied to amount if present
	var itemsTotal = sale.Items.Sum(i => i.Quantity * i.SellingPrice);
	// apply coupon
	itemsTotal -= sale.CouponID == null ? 0.0m : itemsTotal * (Coupons.FirstOrDefault(c => c.CouponID == sale.CouponID).CouponDiscount / 100);
	if (sale.SubTotal != itemsTotal)
	{
		errorlist.Add(new Exception("Subtotal incorrect: did you forget to apply a coupon?"));
	}
	
	// check all items
	foreach (var item in sale.Items)
	{
		// rule : sales quantity must be less than quantity on hand
		if (item.Quantity > StockItems.SingleOrDefault(si => si.StockItemID == item.StockItemID).QuantityOnHand)
		{
			errorlist.Add(new Exception("Item: '" + item.Description + "' Cannot order more than is on hand"));
		}
		
		// rule : item cannot be deiscontinued
		if (StockItems.Any(si => si.StockItemID == item.StockItemID && si.RemoveFromViewFlag)) 
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
	var newSale = new Sales();
	newSale.SaleDate = DateTime.Today;
	newSale.PaymentType = sale.PaymentType;
	newSale.EmployeeID = sale.EmployeeID;
	newSale.TaxAmount = sale.TaxAmount;
	newSale.SubTotal = sale.SubTotal;
	newSale.CouponID = sale.CouponID;
	newSale.SaleDetails = new List<SaleDetails>();
	
	foreach (var item in sale.Items)
	{
		var saleItem = new SaleDetails();
		saleItem.StockItemID = item.StockItemID;
		saleItem.SellingPrice = item.SellingPrice;
		saleItem.Quantity = item.Quantity;
		
		// add item to sale
		newSale.SaleDetails.Add(saleItem);
	}
	
	// guid for debit and credit sale
	// create dummy guid
	if (sale.PaymentType == "C" || sale.PaymentType == "D")
	{
		newSale.PaymentToken = Guid.NewGuid();
	}
	
	Sales.Add(newSale);
	
	// update stock quantities
	foreach (var item in sale.Items)
	{
		// get item
		var updateItem = StockItems.SingleOrDefault(si => si.StockItemID == item.StockItemID);
		// update quantity on hand
		updateItem.QuantityOnHand -= item.Quantity;
		// update record
		StockItems.Update(updateItem);
	}
	
	#endregion
	
	#region save changes
	
	if (errorlist.Count > 0)
	{
		ChangeTracker.Clear();
		throw new AggregateException("Could not process sale", errorlist);
	}
	
	SaveChanges();
	
	#endregion
	
	return newSale.SaleID;
}

public ReturnSaleView GetSaleRefund(int saleID)
{
	var sale = Sales
			.Where(s => !s.RemoveFromViewFlag && s.SaleID == saleID)
			.Select(s => new ReturnSaleView {
				SaleID = s.SaleID,
				EmployeeID = s.EmployeeID,
				CouponID = s.CouponID,
				ReturnSaleDetails = s.SaleDetails.Where(sd => !sd.RemoveFromViewFlag)
									.Select(sd => new ReturnSaleDetailCartView {
										StockItemID = sd.StockItemID,
										Description = sd.StockItem.Description,
										OriginalQty = sd.Quantity,
										SellingPrice = sd.SellingPrice,
										// query refund details
										PreviouReturnQty = SaleRefundDetails.Where(srd => srd.StockItemID == sd.StockItemID)
																.Sum(srd => srd.Quantity)
									})
									.ToList()
			})
			.FirstOrDefault();
	return sale;
}

public int SaveRefund(ReturnSaleView refundSale)
{
	
	#region rule check
	var errorlist = new List<Exception>();
	
	// rule : sale must exist
	if (refundSale.SaleID == 0) 
	{
		errorlist.Add(new Exception("Refunds must be for existing sales"));
	}

	// rule : subtotal must be correct
	// correct subtotal
	var subtotal = refundSale.ReturnSaleDetails.Sum(rsd => rsd.QtyReturnNow * rsd.SellingPrice);
	// apply discount
	subtotal -= refundSale.CouponID == null ? 0 : subtotal * (Coupons.FirstOrDefault(c => c.CouponID == refundSale.CouponID).CouponDiscount / 100);
	if (subtotal != refundSale.SubTotal)
	{
		errorlist.Add(new Exception("refund amount must be correct. Did you forget to apply a coupon"));
	}
	
	foreach (var item in refundSale.ReturnSaleDetails)
	{
		// rule : refund price must be same as original price
		// original item selling price 
		var sellingPrice = SaleDetails.SingleOrDefault(sd => sd.SaleID == refundSale.SaleID && sd.StockItemID == item.StockItemID).SellingPrice;
		if (item.SellingPrice != sellingPrice) 
		{
			errorlist.Add(new Exception("Selling price must be the same as on the original reciept"));	
		}
		
	}
	
	#endregion
	
	#region method logic
	
	// new refund
	var newRefund = new SaleRefunds();
	newRefund.SaleRefundDate = DateTime.Now;
	newRefund.SaleID = refundSale.SaleID;
	newRefund.EmployeeID = refundSale.EmployeeID;
	newRefund.TaxAmount = refundSale.TaxAmount;
	newRefund.SubTotal = refundSale.SubTotal;
	newRefund.SaleRefundDetails = new List<SaleRefundDetails>();
	
	// refunded items
	foreach (var item in refundSale.ReturnSaleDetails)
	{
		if (item.QtyReturnNow > 0) 
		{
			var refundItem = new SaleRefundDetails();
			refundItem.StockItemID = item.StockItemID;
			refundItem.SellingPrice = item.SellingPrice;
			refundItem.Quantity = item.QtyReturnNow;
			
			// add 
			newRefund.SaleRefundDetails.Add(refundItem);
			
			// update item quantities
			var stockItem = StockItems.SingleOrDefault(si => si.StockItemID == item.StockItemID);
			stockItem.QuantityOnHand += item.QtyReturnNow;
			StockItems.Update(stockItem);
		}
	}
	
	SaleRefunds.Add(newRefund);
	
	#endregion
	
	#region save
	
	if (errorlist.Count > 0)
	{
		ChangeTracker.Clear();
		throw new AggregateException("Could not save Refund", errorlist);
	}
	
	SaveChanges();
	
	#endregion
	
	return newRefund.SaleRefundID;
}

#endregion

#region utility methods

public int RandCategory() 
{
	int catCount = Categories.Where(c => !c.RemoveFromViewFlag).ToList().Count;
	
	// random number
	bool isValid;
	int cat;
	do 
	{
		var rand = new Random();
		cat = rand.Next(1, catCount);
		
		// check if cat is valid
		if (Categories.Any(c => c.CategoryID == cat && !c.RemoveFromViewFlag))
		{
			isValid = true;
		}
		else {
			isValid = false;
		}
		
	} while (!isValid);
	
	return cat;
}

public ShoppingCartView GetRandItem() 
{
	var rand = new Random();
	
	
	// check if item is discontinued
	bool isValid;
	int itemid;
	do 
	{
		itemid = rand.Next(1, StockItems.Count());
		if (StockItems.Any(si => si.StockItemID == itemid && !si.RemoveFromViewFlag))
		{
			isValid = true;
		}
		else 
		{
			isValid = false;
		}
		
	} while (!isValid);
	
	// return item
	return StockItems
			.Where(si => si.StockItemID == itemid)
			.Select(si => new ShoppingCartView {
				StockItemID = si.StockItemID,
				Description = si.Description,
				SellingPrice = si.SellingPrice,
				Quantity = 1 
			})
			.FirstOrDefault();
}

public List<ShoppingCartView> GenShoppingCart(int cartLen)
{
	
	var cart = new List<ShoppingCartView>();
	
	for (int i = 0; i < cartLen; i++)
	{
		// get random item
		var item = GetRandItem();
		// item already in cart
		if (cart.Any(c => c.StockItemID == item.StockItemID))
		{
			// get duplicate item
			var dup = cart.Where(i => i.StockItemID == item.StockItemID).FirstOrDefault();
			dup.Quantity += item.Quantity;
		}
		else {
			cart.Add(item);
		}
		
	}
	
	return cart;
}

public void RestoreStock(List<ShoppingCartView> cart)
{
	
	foreach (var item in cart)
	{
		var stockItem = StockItems.SingleOrDefault(si => si.StockItemID == item.StockItemID);
		if (stockItem == null)
		{
			continue;
		}
		
		// update on hand quantity
		stockItem.QuantityOnHand += item.Quantity;
		StockItems.Update(stockItem);
	}
	
	// save changes
	SaveChanges();
}
#endregion

#region view models

public class CategoryView
{
	public int CategoryID { get; set; }
	public string Description { get; set; }
	public int StockItemCount { get; set; }
}

public class StockItemView
{
	public int StockItemID { get; set; }
	public decimal SellingPrice { get; set; }
	public string Description { get; set; }
	public int QuantityOnHand { get; set; }
}

public class ShoppingCartView
{
	public int StockItemID { get; set; }
	public string Description { get; set; }
	public int Quantity { get; set; }
	public decimal SellingPrice { get; set; }
}

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
public class ReturnSaleView
{
	public int SaleID { get; set; }
	public int EmployeeID { get; set; }
	public decimal TaxAmount { get; set; }
	public decimal SubTotal { get; set; }
	public int? CouponID { get; set; }
	public List<ReturnSaleDetailCartView> ReturnSaleDetails { get; set; }
}

public class ReturnSaleDetailCartView
{
	public int StockItemID { get; set; }
	public string Description { get; set; }
	public int OriginalQty { get; set; }
	public decimal SellingPrice { get; set; }
	public int PreviouReturnQty { get; set; }
	public int QtyReturnNow { get; set; }
}



#endregion
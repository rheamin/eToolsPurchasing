<Query Kind="Program">
  <Connection>
    <ID>2e0435b5-1dec-436a-b03a-2bcdf3960d15</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Server>.</Server>
    <Database>eTools2023</Database>
    <DisplayName>eTools2023-Entity</DisplayName>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	// Driver
	try
	{
		// query methods
		GetVendors();
		GetEmployee(1);
		GetVendorPurchaseOrder(1);
		FetchInventoryBy(1);
		FetchInventoryBy(358, 1).Dump();
		bool deleted = DeletePurchaseOrder(359);
		Console.WriteLine(deleted);
		PurchaseOrderEditView testOrder = new PurchaseOrderEditView();
		testOrder.EmployeeID = 1;
		testOrder.VendorID = 4;
		UpdatePurchaseOrder(testOrder, false);
		// check results
		GetVendorPurchaseOrder(4);
	}
	#region catch all exceptions 
	catch (AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	#endregion
}

private Exception GetInnerException(Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException;
	return ex;
}

#region Methods
public List<VendorView> GetVendors()
{
	// get all vendors
	return Vendors
	.Where(x => !x.RemoveFromViewFlag)
	.Select(x => new VendorView
	{
		VendorID = x.VendorID,
		VendorName = x.VendorName,
		Phone = x.Phone,
		Address = x.Address,
		City = x.City,
		Province = x.Province.Description,
		PostalCode = x.PostalCode
	}).ToList().Dump();
}

public EmployeeView GetEmployee (int employeeID)
{
	// get an employee's details
	// parameter validation
	if (employeeID <= 0)
	{
		throw new ArgumentNullException("Please provide a valid employee ID.");	
	}
	
	return Employees
	.Where(x => x.EmployeeID == employeeID && !x.RemoveFromViewFlag)
	.Select(x => new EmployeeView
	{
		EmployeeID = x.EmployeeID,
		FullName = x.FirstName + ' ' + x.LastName
	}).FirstOrDefault().Dump();
}

public PurchaseOrderView GetVendorPurchaseOrder(int vendorID)
{
	// get all purchase orders for a vendor.
	// includes closed ones
	
	// parameter validation
	if (vendorID <= 0)
	{
		throw new ArgumentNullException("Please provide a valid vendor ID.");
	}

	return PurchaseOrders
	.Where(x => x.VendorID == vendorID && !x.RemoveFromViewFlag && x.Closed == false)
	.Select(x => new PurchaseOrderView
	{
		Vendor = Vendors
		.Where(x => x.VendorID == vendorID)
		.Select(x => new VendorView
		{
			VendorID = x.VendorID,
			VendorName = x.VendorName,
			Phone = x.Phone,
			Address = x.Address,
			City = x.City,
			Province = x.Province.Description,
			PostalCode = x.PostalCode
		}).FirstOrDefault(),
		PurchaseOrderID = x.PurchaseOrderID,
		SubTotal = x.Vendor.StockItems.Sum(x => (x.ReOrderLevel - x.QuantityOnHand) * x.SellingPrice),
		GST = x.TaxAmount,
		Items = x.PurchaseOrderDetails
		.Select(x => new ItemView
		{
			PurchaseOrderDetailID = x.PurchaseOrderDetailID,
			StockItemID = x.StockItemID,
			Description = x.StockItem.Description,
			QOH = x.StockItem.QuantityOnHand,
			ROL = x.StockItem.ReOrderLevel,
			QOO = x.StockItem.QuantityOnOrder,
			QTO = x.StockItem.ReOrderLevel - (x.StockItem.QuantityOnHand + x.StockItem.QuantityOnOrder),
			Price = x.StockItem.SellingPrice
		}).ToList()
	}).FirstOrDefault().Dump();
}

public List<ItemView> FetchInventoryBy(int vendorID)
{	
	// fetch inventory including inventory on order
	// parameter validation
	if (vendorID <= 0)
	{
		throw new ArgumentNullException("Please provide a valid vendor ID.");
	}

	return StockItems
	.Where(x => x.VendorID == vendorID && !x.RemoveFromViewFlag)
	.Select(x => new ItemView
	{
		PurchaseOrderDetailID = 0,
		StockItemID = x.StockItemID,
		Description = x.Description,
		QOH = x.QuantityOnHand,
		ROL = x.ReOrderLevel,
		QOO = x.QuantityOnOrder,
		QTO = x.ReOrderLevel - (x.QuantityOnHand + x.QuantityOnOrder),
		Price = x.PurchasePrice
	}).ToList()
	.Dump();
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
	

	return StockItems
			.Where(x => x.VendorID == vendorID && !x.RemoveFromViewFlag && !x.PurchaseOrderDetails.Any(x => x.PurchaseOrderID == PurchaseOrderID))
			.Select(x => new ItemView
			{
				PurchaseOrderDetailID = 0,
				StockItemID = x.StockItemID,
				Description = x.Description,
				QOH = x.QuantityOnHand,
				ROL = x.ReOrderLevel,
				QOO = x.QuantityOnOrder,
				QTO = x.QuantityOnHand - x.ReOrderLevel,
				Price = x.PurchasePrice
			}).ToList();
}
#region Deliverable 2
public void AddItemToPurchaseOrderLine(ItemView item)
{
	
}

public void RemovePurchaseOrderLine(int purchaseOrderLineID)
{
	
}
#endregion
public PurchaseOrderView UpdatePurchaseOrder(PurchaseOrderEditView order, bool placeOrder)
{
	// when you have a new order create a new PO, see if there is any items below the reorder level and add it to the PO
	#region Business Rules
	List<Exception> errorList = new List<Exception>();
	// rule: employee ID must be valid
	if (order.EmployeeID <= 0)
	{
		errorList.Add(new Exception("Please provide a valid employee ID"));
	}
	// rule: vendor ID must be valid
	if (order.VendorID <= 0)
	{
		errorList.Add(new Exception("Please provide a valid vendor ID"));
	}
	#endregion
	#region Main Method Code
	PurchaseOrders purchaseOrder = PurchaseOrders
						.Where(x => x.PurchaseOrderID == order.PurchaseOrderID)
						.FirstOrDefault();

	PurchaseOrderDetails purchaseOrderDetails = PurchaseOrderDetails
							.Where(x => x.PurchaseOrderID == order.PurchaseOrderID)
							.FirstOrDefault();
	// do they want to place the order
	if (placeOrder)
	{
		// rule: purchase order ID must be valid
		if (order.PurchaseOrderID <= 0)
		{
			errorList.Add(new Exception("Please provide a valid purchase order ID"));
		}
		else
		{
			purchaseOrder.OrderDate = DateTime.Today;	
		}
	}
	else
	{
		var isOpenOrder = PurchaseOrders
							.Where(x => order.VendorID == x.VendorID)
							.Any(x => x.Closed == false);

		if (!isOpenOrder)
		{
			// no current open purchase order
			purchaseOrder = new PurchaseOrders();
			purchaseOrderDetails = new PurchaseOrderDetails();

			purchaseOrder.EmployeeID = order.EmployeeID;
			purchaseOrder.VendorID = order.VendorID;
			purchaseOrder.SubTotal = order.SubTotal;
			purchaseOrder.TaxAmount = order.GST;
			purchaseOrder.Closed = false;
			purchaseOrder.RemoveFromViewFlag = false;
			
			order.ItemDetails = new List<ItemDetailView>();
			purchaseOrderDetails.PurchaseOrderID = order.PurchaseOrderID;
			

			foreach (var item in StockItems)
			{
				if (item.VendorID == purchaseOrder.VendorID)
				{
					if (item.ReOrderLevel - (item.QuantityOnHand + item.QuantityOnOrder) < 0)
					{
						ItemDetailView itemDetail = new ItemDetailView();
						itemDetail.Price = item.PurchasePrice;
						itemDetail.QTO = item.QuantityOnHand + item.QuantityOnOrder;
						itemDetail.StockItemID = item.StockItemID;
						purchaseOrderDetails.PurchasePrice = itemDetail.Price;
						purchaseOrderDetails.Quantity = itemDetail.QTO;
						purchaseOrderDetails.StockItemID = itemDetail.StockItemID;
						purchaseOrderDetails.RemoveFromViewFlag = false;
						PurchaseOrderDetails.Add(purchaseOrderDetails);
					}
				}
			}
			
			PurchaseOrders.Add(purchaseOrder);

			#region errorchecking and data saving
			if (errorList.Count > 0)
			{
				ChangeTracker.Clear();
				string errorMsg = "Unable to add or edit purchase order.";
				errorMsg += " Please check error message(s)";
				throw new AggregateException(errorMsg, errorList);
			}
			else
			{
				SaveChanges();
			}
			#endregion
		}
		else
		{
			// update suggested purchase order
			purchaseOrderDetails = new PurchaseOrderDetails();

			foreach (var item in StockItems)
			{
				if (!order.ItemDetails.Any(x => x.StockItemID == item.StockItemID))
				{
					if (item.ReOrderLevel - (item.QuantityOnHand + item.QuantityOnOrder) < 0)
					{
						ItemDetailView itemDetail = new ItemDetailView();
						itemDetail.Price = item.PurchasePrice;
						itemDetail.QTO = item.QuantityOnHand + item.QuantityOnOrder;
						itemDetail.StockItemID = item.StockItemID;
						purchaseOrderDetails.PurchasePrice = itemDetail.Price;
						purchaseOrderDetails.Quantity = itemDetail.QTO;
						purchaseOrderDetails.StockItemID = itemDetail.StockItemID;
						purchaseOrderDetails.RemoveFromViewFlag = false;
						PurchaseOrderDetails.Add(purchaseOrderDetails);
					}
				}
			}
		}
	}
	return GetVendorPurchaseOrder(purchaseOrder.PurchaseOrderID);
}

public bool DeletePurchaseOrder(int purchaseOrderID)
{
	// delete a purchase order (removefromviewflag)
	bool deleted = false;
	List<Exception> errorList = new List<Exception>();

	// parameter validation
	if (purchaseOrderID <= 0)
	{
		throw new ArgumentNullException("Please provide a valid purchase order ID");
	}

	PurchaseOrders purchaseOrderToDelete = PurchaseOrders
											.Where(x => x.PurchaseOrderID == purchaseOrderID && x.OrderDate == null)
											.FirstOrDefault();
	if (purchaseOrderToDelete == null)
	{
		errorList.Add(new Exception("The purchase order doesn't exist or has already been placed"));
	}

	purchaseOrderToDelete.RemoveFromViewFlag = true;

	if (errorList.Count > 0)
	{
		ChangeTracker.Clear();
		string errorMsg = "The purchase order couldnt be deleted. ";
		errorMsg += "Please check error message";
		throw new AggregateException(errorMsg, errorList);
	}
	else
	{
		SaveChanges();
		deleted = true;
	}
	return deleted;
}
#endregion
#endregion
#region View Models
public class VendorView
{
	public int VendorID { get; set; }
	public string VendorName { get; set; }
	public string Phone { get; set; }
	public string Address { get; set; }
	public string City { get; set; }
	public string Province { get; set; }
	public string PostalCode { get; set; }
}

public class EmployeeView
{
	public int EmployeeID { get; set; }
	public string FullName { get; set; }
}

public class PurchaseOrderView //(Web Page View)
{
public VendorView Vendor { get; set; }
public int PurchaseOrderID { get; set; }
public List<ItemView> Items { get; set; }
public decimal SubTotal { get; set; }
public decimal GST { get; set; }
}

// This view can be used as the vendor inventory with a PurchaseOrderDetailID == 0 as we are not using it.
// The PurchaseOrderDetailID will be set when you are either "Update" or "Place" using the save method in your project library.
// Nov 8
// Added PurchaseOrderDetailID
// Updated ID to StockItemID for better clarification.
public class ItemView
{
	public int PurchaseOrderDetailID { get; set; } // used when working with purchaseorderdetails
	public int StockItemID { get; set; }
	public string Description { get; set; }
	public int QOH { get; set; } // Quanity on Hand
	public int ROL { get; set; } // Reoreder Level
	public int QOO { get; set; } // Quantity on Order
	public int QTO { get; set; } // Quantity to Order
	public decimal Price { get; set; }
}

public class PurchaseOrderEditView
{
	public int PurchaseOrderID { get; set; }
	public int VendorID { get; set; }
	public int EmployeeID { get; set; }
	public List<ItemDetailView> ItemDetails { get; set; }
	public decimal SubTotal { get; set; }
	public decimal GST { get; set; }
}

public class ItemDetailView
{
	public int PurchaseOrderDetailID { get; set; }
	public int StockItemID { get; set; }
	public int QTO { get; set; }
	public decimal Price { get; set; }
	public bool RemoveFromViewFlag { get; set; }
}
#endregion
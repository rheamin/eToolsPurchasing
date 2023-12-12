<Query Kind="Program">
  <Connection>
    <ID>e218e9b2-cc6c-4b2a-8874-82f709c24124</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Server>DESKTOP-59U0FTC\SQLEXPRESS</Server>
    <Database>eTools2023</Database>
    <DisplayName>eTools2023-Entity</DisplayName>
    <DriverData>
      <EncryptSqlTraffic>False</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
      <TrustServerCertificate>True</TrustServerCertificate>
      <NoMARS>True</NoMARS>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	//   Travis Simmons - Receiving  

	#region Driver 
	try
	{

		List<OutstandingOrderView> outstandingOrders = PurchaseOrders_FetchOutstandingOrders();
		foreach (var order in outstandingOrders)
		{

			order.Dump("Outstanding Orders");
		}


		int purchaseOrderID = 358;
		var receivingView = PurchaseOrders_FetchOrderDetails(purchaseOrderID);
	
		receivingView.Dump("Before Processing/Receiving");

		List<UnorderedReturnItemView> unorderedItems = UnOrderedItems_FetchUnOrderedItems();
		if (unorderedItems != null && unorderedItems.Any())
		{
			foreach (var item in unorderedItems)
			{
				
				item.Dump("Unordered Return Items");
			}
		}
		receivingView = ReceiveOrders_ProcessReceivedPurchaseOrder(receivingView);

		foreach (var itemDetail in receivingView.ItemDetails)
		{
			int testReceivedQuantity = 5;  
			itemDetail.QtyReceive = testReceivedQuantity;

			int testQuantityOnOrder = 22; 
			itemDetail.QtyOnOrder = testQuantityOnOrder;
			
			itemDetail.QtyOutstanding = itemDetail.QtyOnOrder - itemDetail.QtyReceive;

			if (testReceivedQuantity > testQuantityOnOrder)
			{
				throw new InvalidOperationException("Received quantity cannot be greater than quantity on order.");
			}


			var purchaseOrderDetail = PurchaseOrderDetails.FirstOrDefault(pod => pod.PurchaseOrderDetailID == itemDetail.PurchaseOrderDetailId);
			var receiveOrderDetail = ReceiveOrderDetails.FirstOrDefault(ro => ro.PurchaseOrderDetailID == itemDetail.PurchaseOrderDetailId);


			if (purchaseOrderDetail != null && receiveOrderDetail != null)
			{
				receiveOrderDetail.QuantityReceived += itemDetail.QtyReceive;
				itemDetail.QtyOutstanding = purchaseOrderDetail.Quantity - receiveOrderDetail.QuantityReceived;
			}
		}

		receivingView.Dump("After Processing/Receiving");

		
		
		SaveChanges();
		
		
		
		string reason = "Item has been discontinued by the manufacturer.";
		PurchaseOrders_ForceCloseOrder(reason, receivingView);

		receivingView.Dump("After Force Close");

		SaveChanges();

	}

	#endregion

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


public List<OutstandingOrderView> PurchaseOrders_FetchOutstandingOrders()
{
	return PurchaseOrders
	   .Where(x => !x.Closed)
	   .Select(x => new OutstandingOrderView
	   {
		   PurchaseOrderID = x.PurchaseOrderID,
		   OrderDate = x.OrderDate,
		   VendorName = x.Vendor.VendorName,
		   VendorPhone = x.Vendor.Phone,
	   }).ToList();
}

public List<UnorderedReturnItemView> UnOrderedItems_FetchUnOrderedItems()
{
	return UnOrderedItems
		.Select(x => new UnorderedReturnItemView
		{
			UnorderedItemItem = x.UnOrderedItemID,
			PurchaseOrderID = x.ReceiveOrder.PurchaseOrderID,
			ItemID = x.ItemID,
			Description = x.ItemName,
			VSN = x.VendorProductID,
			Quantity = x.Quantity,
			RemoveFromViewFlag = x.RemoveFromViewFlag

		}).ToList();
}

public ReceivingView ReceiveOrders_ProcessReceivedPurchaseOrder(ReceivingView receiving)
{
	if (receiving == null)
	{
		throw new ArgumentNullException(nameof(receiving), "Received order data is null.");
	}

	foreach (var itemDetail in receiving.ItemDetails)
	{
		
		var purchaseOrderDetail = PurchaseOrderDetails.FirstOrDefault(pod => pod.PurchaseOrderDetailID == itemDetail.PurchaseOrderDetailId);

		
		var receiveOrderDetail = ReceiveOrderDetails.FirstOrDefault(ro => ro.PurchaseOrderDetailID == itemDetail.PurchaseOrderDetailId);

		if (purchaseOrderDetail != null && receiveOrderDetail != null)
		{	
			receiveOrderDetail.QuantityReceived += itemDetail.QtyReceive;
	
			itemDetail.QtyOutstanding = purchaseOrderDetail.Quantity - receiveOrderDetail.QuantityReceived;
		}
	}

	SaveChanges();

	return receiving; 
}

public void PurchaseOrders_ForceCloseOrder(string reason, ReceivingView receiving)
{
	if (receiving == null || string.IsNullOrEmpty(reason))
	{
		throw new ArgumentException("A reason is required to close this order.");
	}

	var purchaseOrder = PurchaseOrders.FirstOrDefault(po => po.PurchaseOrderID == receiving.PurchaseOrderID);
	if (purchaseOrder == null)
	{
		throw new InvalidOperationException("Purchase order does not exist in the database.");
	}

	purchaseOrder.Notes = reason;
	purchaseOrder.Closed = true; 


	foreach (var itemDetail in receiving.ItemDetails)
	{
		itemDetail.QtyOnOrder = 0;
		itemDetail.QtyOutstanding = 0; 
		itemDetail.Reason = reason;
	}

	receiving.CanBeClosed = false;

	SaveChanges();
}


public ReceivingView PurchaseOrders_FetchOrderDetails(int purchaseOrder)
{
	var order = PurchaseOrders
		.Include(p => p.Vendor)
		.FirstOrDefault(x => x.PurchaseOrderID == purchaseOrder);

	if (order == null)
	{

		throw new InvalidOperationException("Uh Oh. Order not found.");

	}

	var receivingView = new ReceivingView
	{
		PurchaseOrderID = order.PurchaseOrderID,
		OrderDate = order.OrderDate,
		Vendor = new VendorView
		{
			VendorName = order.Vendor.VendorName,
			VendorPhone = order.Vendor.Phone,
			Address = order.Vendor.Address,
			City = order.Vendor.City,
			Province = order.Vendor.Province.Description,
			PostalCode = order.Vendor.PostalCode,

		},
		CanBeClosed = !order.Closed,

		ItemDetails = order.PurchaseOrderDetails
		 .Select(od =>
		{
			var receiveOrderDetail = ReceiveOrderDetails.FirstOrDefault(ro => ro.PurchaseOrderDetailID == od.PurchaseOrderDetailID);

			return new ItemDetailView
			{
				PurchaseOrderDetailId = od.PurchaseOrderDetailID,
				StockItemId = od.StockItemID,
				StockItemDescription = od.StockItem.Description,
				QtyOnOrder = od.Quantity,
				QtyOutstanding = od.Quantity-(receiveOrderDetail?.QuantityReceived ?? 0)
			};
		}).ToList()
	};


	return receivingView;
}


#endregion

#region Class/View Model   

public class OutstandingOrderView
{
	public int PurchaseOrderID { get; set; }
	public DateTime? OrderDate { get; set; }
	public string VendorName { get; set; }
	public string VendorPhone { get; set; }
}

public class VendorView
{
	public string VendorName { get; set; }
	public string Address { get; set; }
	public string City { get; set; }
	public string Province { get; set; }
	public string PostalCode { get; set; }
	public string VendorPhone { get; set; }
}

public class ReturnedOrderDetailView
{
	public int ReturnedOrderDetailID { get; set; }
	public int ReceiveOrderID { get; set; }
	public int PurchaseOrderDetailID { get; set; }
	public int UnOrderedItemID { get; set; }
	public string ItemDescription { get; set; }
	public int Quantity { get; set; }
	public string Reason { get; set; }
	public string VendorStockNumber { get; set; }
	public bool RemoveFromViewFlag { get; set; }
}


public class UnorderedReturnItemView
{
	public int UnorderedItemItem { get; set; }
	public int PurchaseOrderID { get; set; }
	public int ItemID { get; set; }
	public string Description { get; set; }
	public string VSN { get; set; }
	public int Quantity { get; set; }
	public bool RemoveFromViewFlag { get; set; }
}
public class ReceivingView
{
	public int PurchaseOrderID { get; set; }
	public DateTime? OrderDate { get; set; }
	public VendorView Vendor { get; set; }
	public bool CanBeClosed { get; set; }
	public List<ItemDetailView> ItemDetails { get; set; }
	public List<UnorderedReturnItemView> UnorderedReturnItems { get; set; }
}

public class ItemDetailView
{
	public int PurchaseOrderDetailId { get; set; }
	public int StockItemId { get; set; }
	public string StockItemDescription { get; set; }
	public int QtyOnOrder { get; set; }
	public int QtyOutstanding { get; set; }
	public int QtyReceive { get; set; }
	public int QtyReturn { get; set; }
	public string Reason { get; set; }
}


#endregion

#region Supporting Methods

public void UnOrderedItems_AddUnOrderedItem(int? itemId)
{

}
public void UnOrderedItems_RemoveUnOrderedItem(int? itemId)
{

}

#endregion

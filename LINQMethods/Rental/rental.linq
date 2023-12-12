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
	
}

// You can define other methods, fields, classes and namespaces here

#region methods

public CustomerView GetCustomerByPhone(string phone)
{
	
	var customer = Customers
					.Where(c => c.ContactPhone == phone)
					.Select(c => new CustomerView {
						CustomerID = c.CustomerID,
						LastName = c.LastName,
						FirstName = c.FirstName,
						Address = c.Address,
					})
					.FirstOrDefault();
	
	if (customer == null)
	{
		throw new KeyNotFoundException("Could not find customer");
	}
	
	return customer;
}

public List<AvailableEquipmentView> GetEquipments() 
{
	var availableEqp = RentalEquipments
						.Where(re => re.Available && !re.Retired && !re.RemoveFromViewFlag)
						.Select( re => new AvailableEquipmentView {
							RentalEquipmentID = re.RentalEquipmentID,
							Description = re.Description,
							SerialNumber = re.SerialNumber,
							DailyRate = re.DailyRate,
							Condition = re.Condition
						})
						.ToList();
	
	return availableEqp;
}

public decimal GetCoupon(string coupon)
{
	var couponItem = Coupons
					.SingleOrDefault(c => c.CouponIDValue == coupon && !c.RemoveFromViewFlag);
	if (couponItem == null)
	{
		throw new KeyNotFoundException("Coupon not found");
	}
	
	return couponItem.CouponDiscount;
}


#endregion // methods


#region utility methods



#endregion // utility methods

#region view models

// Updated Nov 8
// added RentalID
public class CustomerView
{
	public int CustomerID { get; set; }
	public string LastName { get; set; }
	public string FirstName { get; set; }
	public string Address { get; set; }
	public int RentalID { get; set; }
}

public class AvailableEquipmentView
{
	public int RentalEquipmentID { get; set; }
	public string Description { get; set; }
	public string SerialNumber { get; set; }
	public decimal DailyRate { get; set; }
	public string Condition { get; set; }
}

public class RentalsView
{
	public int RentalID { get; set; }
	public int CustomerID { get; set; }
	public int EmployeeID { get; set; }
	public int CouponID { get; set; }
	public decimal SubTotal { get; set; }
	public decimal TaxAmount { get; set; }
	public DateTime RentalDateOut { get; set; }
	public DateTime RentalDateIn { get; set; }
	public string PaymentType { get; set; }
	public List<RentalDetailView> RentalDetails { get; set; }
}

public class RentalDetailView
{
	public int RentalDetailID { get; set; }
	public int RentalID { get; set; }
	public int RentalEquipmentID { get; set; }
	public decimal RentalDays { get; set; }
	public decimal RentalRate { get; set; }
	public string OutCondition { get; set; }
	public string InCondition { get; set; }
	public decimal DamageRepairCost { get; set; }
	public string Comments { get; set; }
}



#endregion // view models
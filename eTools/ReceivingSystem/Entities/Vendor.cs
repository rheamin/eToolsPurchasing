using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ReceivingSystem.Entities;

public partial class Vendor
{
    [Key]
    [Column("VendorID")]
    public int VendorId { get; set; }

    [StringLength(100)]
    public string VendorName { get; set; } = null!;

    [StringLength(12)]
    public string Phone { get; set; } = null!;

    [StringLength(30)]
    public string Address { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string City { get; set; } = null!;

    [Column("ProvinceID")]
    [StringLength(2)]
    public string ProvinceId { get; set; } = null!;

    [StringLength(6)]
    public string PostalCode { get; set; } = null!;

    public bool RemoveFromViewFlag { get; set; }

    [InverseProperty("Vendor")]
    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();

    [InverseProperty("Vendor")]
    public virtual ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
}

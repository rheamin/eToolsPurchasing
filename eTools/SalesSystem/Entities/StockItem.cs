using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SalesSystem.Entities;

public partial class StockItem
{
    [Key]
    [Column("StockItemID")]
    public int StockItemId { get; set; }

    [StringLength(50)]
    public string Description { get; set; } = null!;

    [Column(TypeName = "smallmoney")]
    public decimal SellingPrice { get; set; }

    [Column(TypeName = "smallmoney")]
    public decimal PurchasePrice { get; set; }

    public int QuantityOnHand { get; set; }

    public int QuantityOnOrder { get; set; }

    public int ReOrderLevel { get; set; }

    public bool Discontinued { get; set; }

    [Column("VendorID")]
    public int VendorId { get; set; }

    [StringLength(25)]
    public string? VendorStockNumber { get; set; }

    [Column("CategoryID")]
    public int CategoryId { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("StockItems")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("StockItem")]
    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();

    [InverseProperty("StockItem")]
    public virtual ICollection<SaleRefundDetail> SaleRefundDetails { get; set; } = new List<SaleRefundDetail>();
}

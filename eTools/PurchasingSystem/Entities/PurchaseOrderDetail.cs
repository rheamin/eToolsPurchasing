using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PurchasingSystem.Entities;

internal partial class PurchaseOrderDetail
{
    [Key]
    [Column("PurchaseOrderDetailID")]
    public int PurchaseOrderDetailId { get; set; }

    [Column("PurchaseOrderID")]
    public int PurchaseOrderId { get; set; }

    [Column("StockItemID")]
    public int StockItemId { get; set; }

    [Column(TypeName = "money")]
    public decimal PurchasePrice { get; set; }

    public int Quantity { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    [ForeignKey("PurchaseOrderId")]
    [InverseProperty("PurchaseOrderDetails")]
    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;

    [ForeignKey("StockItemId")]
    [InverseProperty("PurchaseOrderDetails")]
    public virtual StockItem StockItem { get; set; } = null!;
}

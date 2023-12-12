using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ReceivingSystem.Entities;

public partial class ReturnedOrderDetail
{
    [Key]
    [Column("ReturnedOrderDetailID")]
    public int ReturnedOrderDetailId { get; set; }

    [Column("ReceiveOrderID")]
    public int ReceiveOrderId { get; set; }

    [Column("PurchaseOrderDetailID")]
    public int? PurchaseOrderDetailId { get; set; }

    [Column("UnOrderedItemID")]
    public int? UnOrderedItemId { get; set; }

    [StringLength(50)]
    public string? ItemDescription { get; set; }

    public int Quantity { get; set; }

    [StringLength(50)]
    public string Reason { get; set; } = null!;

    [StringLength(15)]
    public string? VendorStockNumber { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    [ForeignKey("PurchaseOrderDetailId")]
    [InverseProperty("ReturnedOrderDetails")]
    public virtual PurchaseOrderDetail? PurchaseOrderDetail { get; set; }

    [ForeignKey("ReceiveOrderId")]
    [InverseProperty("ReturnedOrderDetails")]
    public virtual ReceiveOrder ReceiveOrder { get; set; } = null!;

    [ForeignKey("UnOrderedItemId")]
    [InverseProperty("ReturnedOrderDetails")]
    public virtual UnOrderedItem? UnOrderedItem { get; set; }
}

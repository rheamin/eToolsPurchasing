using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ReceivingSystem.Entities;

public partial class ReceiveOrderDetail
{
    [Key]
    [Column("ReceiveOrderDetailID")]
    public int ReceiveOrderDetailId { get; set; }

    [Column("ReceiveOrderID")]
    public int ReceiveOrderId { get; set; }

    [Column("PurchaseOrderDetailID")]
    public int PurchaseOrderDetailId { get; set; }

    public int QuantityReceived { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    [ForeignKey("PurchaseOrderDetailId")]
    [InverseProperty("ReceiveOrderDetails")]
    public virtual PurchaseOrderDetail PurchaseOrderDetail { get; set; } = null!;

    [ForeignKey("ReceiveOrderId")]
    [InverseProperty("ReceiveOrderDetails")]
    public virtual ReceiveOrder ReceiveOrder { get; set; } = null!;
}

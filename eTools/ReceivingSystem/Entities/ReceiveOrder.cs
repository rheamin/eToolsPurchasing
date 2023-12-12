using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ReceivingSystem.Entities;

public partial class ReceiveOrder
{
    [Key]
    [Column("ReceiveOrderID")]
    public int ReceiveOrderId { get; set; }

    [Column("PurchaseOrderID")]
    public int PurchaseOrderId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReceiveDate { get; set; }

    [Column("EmployeeID")]
    public int? EmployeeId { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("ReceiveOrders")]
    public virtual Employee? Employee { get; set; }

    [ForeignKey("PurchaseOrderId")]
    [InverseProperty("ReceiveOrders")]
    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;

    [InverseProperty("ReceiveOrder")]
    public virtual ICollection<ReceiveOrderDetail> ReceiveOrderDetails { get; set; } = new List<ReceiveOrderDetail>();

    [InverseProperty("ReceiveOrder")]
    public virtual ICollection<ReturnedOrderDetail> ReturnedOrderDetails { get; set; } = new List<ReturnedOrderDetail>();

    [InverseProperty("ReceiveOrder")]
    public virtual ICollection<UnOrderedItem> UnOrderedItems { get; set; } = new List<UnOrderedItem>();
}

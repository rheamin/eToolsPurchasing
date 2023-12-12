using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ReceivingSystem.Entities;

public partial class UnOrderedItem
{
    [Key]
    [Column("UnOrderedItemID")]
    public int UnOrderedItemId { get; set; }

    [Column("ReceiveOrderID")]
    public int ReceiveOrderId { get; set; }

    [Column("ItemID")]
    public int ItemId { get; set; }

    [StringLength(50)]
    public string ItemName { get; set; } = null!;

    [Column("VendorProductID")]
    [StringLength(25)]
    public string VendorProductId { get; set; } = null!;

    public int Quantity { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    [ForeignKey("ReceiveOrderId")]
    [InverseProperty("UnOrderedItems")]
    public virtual ReceiveOrder ReceiveOrder { get; set; } = null!;

    [InverseProperty("UnOrderedItem")]
    public virtual ICollection<ReturnedOrderDetail> ReturnedOrderDetails { get; set; } = new List<ReturnedOrderDetail>();
}

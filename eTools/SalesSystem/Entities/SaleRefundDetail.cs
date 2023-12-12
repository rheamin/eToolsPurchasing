using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SalesSystem.Entities;

public partial class SaleRefundDetail
{
    [Key]
    [Column("SaleRefundDetailID")]
    public int SaleRefundDetailId { get; set; }

    [Column("SaleRefundID")]
    public int SaleRefundId { get; set; }

    [Column("StockItemID")]
    public int StockItemId { get; set; }

    [Column(TypeName = "money")]
    public decimal SellingPrice { get; set; }

    public int Quantity { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    [ForeignKey("SaleRefundId")]
    [InverseProperty("SaleRefundDetails")]
    public virtual SaleRefund SaleRefund { get; set; } = null!;

    [ForeignKey("StockItemId")]
    [InverseProperty("SaleRefundDetails")]
    public virtual StockItem StockItem { get; set; } = null!;
}

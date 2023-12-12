using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SalesSystem.Entities;

public partial class SaleDetail
{
    [Key]
    [Column("SaleDetailID")]
    public int SaleDetailId { get; set; }

    [Column("SaleID")]
    public int SaleId { get; set; }

    [Column("StockItemID")]
    public int StockItemId { get; set; }

    [Column(TypeName = "smallmoney")]
    public decimal SellingPrice { get; set; }

    public int Quantity { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    [ForeignKey("SaleId")]
    [InverseProperty("SaleDetails")]
    public virtual Sale Sale { get; set; } = null!;

    [ForeignKey("StockItemId")]
    [InverseProperty("SaleDetails")]
    public virtual StockItem StockItem { get; set; } = null!;
}

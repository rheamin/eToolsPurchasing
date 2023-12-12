using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SalesSystem.Entities;

public partial class SaleRefund
{
    [Key]
    [Column("SaleRefundID")]
    public int SaleRefundId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime SaleRefundDate { get; set; }

    [Column("SaleID")]
    public int SaleId { get; set; }

    [Column("EmployeeID")]
    public int EmployeeId { get; set; }

    [Column(TypeName = "money")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "money")]
    public decimal SubTotal { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("SaleRefunds")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("SaleId")]
    [InverseProperty("SaleRefunds")]
    public virtual Sale Sale { get; set; } = null!;

    [InverseProperty("SaleRefund")]
    public virtual ICollection<SaleRefundDetail> SaleRefundDetails { get; set; } = new List<SaleRefundDetail>();
}

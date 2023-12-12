using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SalesSystem.Entities;

public partial class Sale
{
    [Key]
    [Column("SaleID")]
    public int SaleId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime SaleDate { get; set; }

    [StringLength(1)]
    public string PaymentType { get; set; } = null!;

    [Column("EmployeeID")]
    public int EmployeeId { get; set; }

    [Column(TypeName = "smallmoney")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "money")]
    public decimal SubTotal { get; set; }

    [Column("CouponID")]
    public int? CouponId { get; set; }

    public Guid? PaymentToken { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    [ForeignKey("CouponId")]
    [InverseProperty("Sales")]
    public virtual Coupon? Coupon { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Sales")]
    public virtual Employee Employee { get; set; } = null!;

    [InverseProperty("Sale")]
    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();

    [InverseProperty("Sale")]
    public virtual ICollection<SaleRefund> SaleRefunds { get; set; } = new List<SaleRefund>();
}

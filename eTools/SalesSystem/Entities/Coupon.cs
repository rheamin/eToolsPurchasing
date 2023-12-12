using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SalesSystem.Entities;

[Index("CouponIdvalue", Name = "UQ_Coupons_CouponIDValue", IsUnique = true)]
public partial class Coupon
{
    [Key]
    [Column("CouponID")]
    public int CouponId { get; set; }

    [Column("CouponIDValue")]
    [StringLength(10)]
    public string CouponIdvalue { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EndDate { get; set; }

    public int CouponDiscount { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    [InverseProperty("Coupon")]
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}

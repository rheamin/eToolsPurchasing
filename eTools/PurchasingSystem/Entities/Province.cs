using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PurchasingSystem.Entities;

internal partial class Province
{
    [Key]
    [Column("ProvinceID")]
    [StringLength(2)]
    public string ProvinceId { get; set; } = null!;

    [StringLength(50)]
    public string Description { get; set; } = null!;

    public bool RemoveFromViewFlag { get; set; }

    [InverseProperty("Province")]
    public virtual ICollection<Vendor> Vendors { get; set; } = new List<Vendor>();
}

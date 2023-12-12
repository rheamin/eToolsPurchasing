using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ReceivingSystem.Entities;

public partial class Employee
{
    [Key]
    [Column("EmployeeID")]
    public int EmployeeId { get; set; }

    [StringLength(25)]
    public string FirstName { get; set; } = null!;

    [StringLength(25)]
    public string LastName { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime DateHired { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateReleased { get; set; }

    [Column("PositionID")]
    public int PositionId { get; set; }

    [Column("LoginID")]
    [StringLength(30)]
    public string? LoginId { get; set; }

    [StringLength(75)]
    public string Address { get; set; } = null!;

    [StringLength(30)]
    public string City { get; set; } = null!;

    [StringLength(12)]
    public string ContactPhone { get; set; } = null!;

    [StringLength(6)]
    public string PostalCode { get; set; } = null!;

    public bool RemoveFromViewFlag { get; set; }

    [ForeignKey("PositionId")]
    [InverseProperty("Employees")]
    public virtual Position Position { get; set; } = null!;

    [InverseProperty("Employee")]
    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();

    [InverseProperty("Employee")]
    public virtual ICollection<ReceiveOrder> ReceiveOrders { get; set; } = new List<ReceiveOrder>();
}

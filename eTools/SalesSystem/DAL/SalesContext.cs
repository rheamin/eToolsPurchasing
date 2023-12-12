using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SalesSystem.Entities;

namespace SalesSystem.DAL;

public partial class SalesContext : DbContext
{
    public SalesContext()
    {
    }

    public SalesContext(DbContextOptions<SalesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SaleDetail> SaleDetails { get; set; }

    public virtual DbSet<SaleRefund> SaleRefunds { get; set; }

    public virtual DbSet<SaleRefundDetail> SaleRefundDetails { get; set; }

    public virtual DbSet<StockItem> StockItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK_Category_CategoryID");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.CouponId).HasName("PK_Coupons_CouponID");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK_Employee_EmployeeID");

            entity.Property(e => e.ContactPhone).IsFixedLength();
            entity.Property(e => e.PostalCode).IsFixedLength();

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeesPositions_PositionID");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK_Position_PositionID");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.SaleId).HasName("PK_Sales_SaleNumber");

            entity.Property(e => e.PaymentType).IsFixedLength();
            entity.Property(e => e.SaleDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Coupon).WithMany(p => p.Sales).HasConstraintName("FK_SalesCoupons_CouponID");

            entity.HasOne(d => d.Employee).WithMany(p => p.Sales)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesEmployees_EmployeeID");
        });

        modelBuilder.Entity<SaleDetail>(entity =>
        {
            entity.HasKey(e => e.SaleDetailId).HasName("PK_SaleDetails_SaleDetailsID");

            entity.HasOne(d => d.Sale).WithMany(p => p.SaleDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SaleDetailsSales_SaleID");

            entity.HasOne(d => d.StockItem).WithMany(p => p.SaleDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SaleDetailsStockItems_StockItemID");
        });

        modelBuilder.Entity<SaleRefund>(entity =>
        {
            entity.HasKey(e => e.SaleRefundId).HasName("PK_SaleRefunds_SaleRefundID");

            entity.Property(e => e.SaleRefundDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Employee).WithMany(p => p.SaleRefunds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SaleRefundsEmployees_EmployeeID");

            entity.HasOne(d => d.Sale).WithMany(p => p.SaleRefunds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SaleRefundsSales_SaleID");
        });

        modelBuilder.Entity<SaleRefundDetail>(entity =>
        {
            entity.HasKey(e => e.SaleRefundDetailId).HasName("PK_SaleRefundDetails_SaleRefundDetailID");

            entity.HasOne(d => d.SaleRefund).WithMany(p => p.SaleRefundDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SaleRefundDetailsSaleRefunds_SaleRefundID");

            entity.HasOne(d => d.StockItem).WithMany(p => p.SaleRefundDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SaleRefundDetailsStockItems_StockItemID");
        });

        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.HasKey(e => e.StockItemId).HasName("PK_StockItems_StockItemID");

            entity.HasOne(d => d.Category).WithMany(p => p.StockItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockItemsCategories_CategoryID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

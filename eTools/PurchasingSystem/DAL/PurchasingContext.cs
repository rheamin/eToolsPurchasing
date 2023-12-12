using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PurchasingSystem.Entities;

namespace PurchasingSystem.DAL;

internal partial class PurchasingContext : DbContext
{
    public PurchasingContext()
    {
    }

    public PurchasingContext(DbContextOptions<PurchasingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

    public virtual DbSet<StockItem> StockItems { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK_Category_CategoryID");
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

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.ProvinceId).HasName("PK_Provinces_ProvinceCode");

            entity.Property(e => e.ProvinceId).IsFixedLength();
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.PurchaseOrderId).HasName("PK_PurchaseOrders_PurchaseOrderID");

            entity.HasOne(d => d.Employee).WithMany(p => p.PurchaseOrders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseOrdersEmployees_EmployeeID");

            entity.HasOne(d => d.Vendor).WithMany(p => p.PurchaseOrders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseOrdersVendors_VendorID");
        });

        modelBuilder.Entity<PurchaseOrderDetail>(entity =>
        {
            entity.HasKey(e => e.PurchaseOrderDetailId).HasName("PK_PurchaseOrderDetails_OrderDetailID");

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseOrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseOrderDetailsPurchaseOrders_OrderID");

            entity.HasOne(d => d.StockItem).WithMany(p => p.PurchaseOrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseOrderDetailsStockItems_StockItemID");
        });

        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.HasKey(e => e.StockItemId).HasName("PK_StockItems_StockItemID");

            entity.HasOne(d => d.Category).WithMany(p => p.StockItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockItemsCategories_CategoryID");

            entity.HasOne(d => d.Vendor).WithMany(p => p.StockItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockItemsVendors_VendorID");
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.VendorId).HasName("PK_Vendors_VendorID");

            entity.Property(e => e.PostalCode).IsFixedLength();
            entity.Property(e => e.ProvinceId).IsFixedLength();

            entity.HasOne(d => d.Province).WithMany(p => p.Vendors)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CK_VendorsProvinces_ProvinceID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
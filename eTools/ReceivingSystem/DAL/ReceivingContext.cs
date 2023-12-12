using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ReceivingSystem.Entities;

namespace ReceivingSystem.DAL;

public partial class ReceivingContext : DbContext
{
    public ReceivingContext()
    {
    }

    public ReceivingContext(DbContextOptions<ReceivingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

    public virtual DbSet<ReceiveOrder> ReceiveOrders { get; set; }

    public virtual DbSet<ReceiveOrderDetail> ReceiveOrderDetails { get; set; }

    public virtual DbSet<ReturnedOrderDetail> ReturnedOrderDetails { get; set; }

    public virtual DbSet<StockItem> StockItems { get; set; }

    public virtual DbSet<UnOrderedItem> UnOrderedItems { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433; Database=eTools2023; User Id=SA; Password=Nintendo77aa; TrustServerCertificate=true;");

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

        modelBuilder.Entity<ReceiveOrder>(entity =>
        {
            entity.HasKey(e => e.ReceiveOrderId).HasName("PK_ReceiveOrders_ReceiveOrderID");

            entity.HasOne(d => d.Employee).WithMany(p => p.ReceiveOrders).HasConstraintName("FK_ReceiveOrdersEmployees_EmployeeID");

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.ReceiveOrders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReceiveOrdersPurchaseOrders_OrderID");
        });

        modelBuilder.Entity<ReceiveOrderDetail>(entity =>
        {
            entity.HasOne(d => d.PurchaseOrderDetail).WithMany(p => p.ReceiveOrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReceiveOrderDetailsPurchaseOrderDetails_OrderDetailID");

            entity.HasOne(d => d.ReceiveOrder).WithMany(p => p.ReceiveOrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReceiveOrderDetailsReceiveOrders_ReceiveOrderID");
        });

        modelBuilder.Entity<ReturnedOrderDetail>(entity =>
        {
            entity.HasOne(d => d.PurchaseOrderDetail).WithMany(p => p.ReturnedOrderDetails).HasConstraintName("FK_ReturnedOrderDetailsPurchaseOrderDetails_OrderDetailID");

            entity.HasOne(d => d.ReceiveOrder).WithMany(p => p.ReturnedOrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReturnedOrderDetailsReceiveOrders_ReceiveOrder");

            entity.HasOne(d => d.UnOrderedItem).WithMany(p => p.ReturnedOrderDetails).HasConstraintName("FK_ReturnedOrderDetailsUnOrderedItems_UnOrderedItemID");
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

        modelBuilder.Entity<UnOrderedItem>(entity =>
        {
            entity.HasKey(e => e.UnOrderedItemId).HasName("PK_UnOrderedItems_ItemID");

            entity.HasOne(d => d.ReceiveOrder).WithMany(p => p.UnOrderedItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UnOrderedItems_ReceiveOrderID");
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.VendorId).HasName("PK_Vendors_VendorID");

            entity.Property(e => e.PostalCode).IsFixedLength();
            entity.Property(e => e.ProvinceId).IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

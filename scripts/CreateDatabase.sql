--------------------------------------------------------------------------------
-- CreateDatabase.sql
-- Unified SQL Server script generated from all DbContext entity definitions:
--   PurchasingSystem, ReceivingSystem, SalesSystem
-- Database: eTools2023
--------------------------------------------------------------------------------

USE [master]
GO

IF DB_ID('eTools2023') IS NOT NULL
BEGIN
    ALTER DATABASE [eTools2023] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [eTools2023];
END
GO

CREATE DATABASE [eTools2023]
GO

USE [eTools2023]
GO

--------------------------------------------------------------------------------
-- 1. Tables with no foreign keys
--------------------------------------------------------------------------------

CREATE TABLE [dbo].[Positions] (
    [PositionID]       INT            NOT NULL,
    [Description]      NVARCHAR(50)   NOT NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_Positions_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_Positions_PositionID] PRIMARY KEY CLUSTERED ([PositionID])
)
GO

CREATE TABLE [dbo].[Provinces] (
    [ProvinceID]       NCHAR(2)       NOT NULL,
    [Description]      NVARCHAR(50)   NOT NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_Provinces_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_Provinces_ProvinceCode] PRIMARY KEY CLUSTERED ([ProvinceID])
)
GO

CREATE TABLE [dbo].[Categories] (
    [CategoryID]       INT            NOT NULL,
    [Description]      VARCHAR(50)    NOT NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_Categories_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_Categories_CategoryID] PRIMARY KEY CLUSTERED ([CategoryID])
)
GO

CREATE TABLE [dbo].[Coupons] (
    [CouponID]         INT            NOT NULL,
    [CouponIDValue]    NVARCHAR(10)   NOT NULL,
    [StartDate]        DATETIME       NOT NULL,
    [EndDate]          DATETIME       NOT NULL,
    [CouponDiscount]   INT            NOT NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_Coupons_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_Coupons_CouponID] PRIMARY KEY CLUSTERED ([CouponID])
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [UQ_Coupons_CouponIDValue]
    ON [dbo].[Coupons] ([CouponIDValue])
GO

--------------------------------------------------------------------------------
-- 2. Tables with single-level foreign keys
--------------------------------------------------------------------------------

CREATE TABLE [dbo].[Employees] (
    [EmployeeID]       INT            NOT NULL,
    [FirstName]        NVARCHAR(25)   NOT NULL,
    [LastName]         NVARCHAR(25)   NOT NULL,
    [DateHired]        DATETIME       NOT NULL,
    [DateReleased]     DATETIME       NULL,
    [PositionID]       INT            NOT NULL,
    [LoginID]          NVARCHAR(30)   NULL,
    [Address]          NVARCHAR(75)   NOT NULL,
    [City]             NVARCHAR(30)   NOT NULL,
    [ContactPhone]     NCHAR(12)      NOT NULL,
    [PostalCode]       NCHAR(6)       NOT NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_Employees_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_Employees_EmployeeID] PRIMARY KEY CLUSTERED ([EmployeeID])
)
GO

CREATE TABLE [dbo].[Vendors] (
    [VendorID]         INT            NOT NULL,
    [VendorName]       NVARCHAR(100)  NOT NULL,
    [Phone]            NVARCHAR(12)   NOT NULL,
    [Address]          NVARCHAR(30)   NOT NULL,
    [City]             VARCHAR(50)    NOT NULL,
    [ProvinceID]       NCHAR(2)       NOT NULL,
    [PostalCode]       NCHAR(6)       NOT NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_Vendors_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_Vendors_VendorID] PRIMARY KEY CLUSTERED ([VendorID])
)
GO

--------------------------------------------------------------------------------
-- 3. Tables depending on Employees / Vendors / Categories
--------------------------------------------------------------------------------

CREATE TABLE [dbo].[StockItems] (
    [StockItemID]      INT            NOT NULL,
    [Description]      NVARCHAR(50)   NOT NULL,
    [SellingPrice]     SMALLMONEY     NOT NULL,
    [PurchasePrice]    SMALLMONEY     NOT NULL,
    [QuantityOnHand]   INT            NOT NULL,
    [QuantityOnOrder]  INT            NOT NULL,
    [ReOrderLevel]     INT            NOT NULL,
    [Discontinued]     BIT            NOT NULL CONSTRAINT [DF_StockItems_Discontinued] DEFAULT (0),
    [VendorID]         INT            NOT NULL,
    [VendorStockNumber] NVARCHAR(25)  NULL,
    [CategoryID]       INT            NOT NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_StockItems_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_StockItems_StockItemID] PRIMARY KEY CLUSTERED ([StockItemID])
)
GO

--------------------------------------------------------------------------------
-- 4. PurchaseOrders (depends on Employees, Vendors)
--------------------------------------------------------------------------------

CREATE TABLE [dbo].[PurchaseOrders] (
    [PurchaseOrderID]  INT            NOT NULL,
    [OrderDate]        DATETIME       NULL,
    [VendorID]         INT            NOT NULL,
    [EmployeeID]       INT            NOT NULL,
    [TaxAmount]        MONEY          NOT NULL,
    [SubTotal]         MONEY          NOT NULL,
    [Closed]           BIT            NOT NULL CONSTRAINT [DF_PurchaseOrders_Closed] DEFAULT (0),
    [Notes]            NVARCHAR(100)  NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_PurchaseOrders_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_PurchaseOrders_PurchaseOrderID] PRIMARY KEY CLUSTERED ([PurchaseOrderID])
)
GO

--------------------------------------------------------------------------------
-- 5. PurchaseOrderDetails (depends on PurchaseOrders, StockItems)
--------------------------------------------------------------------------------

CREATE TABLE [dbo].[PurchaseOrderDetails] (
    [PurchaseOrderDetailID] INT        NOT NULL,
    [PurchaseOrderID]  INT            NOT NULL,
    [StockItemID]      INT            NOT NULL,
    [PurchasePrice]    MONEY          NOT NULL,
    [Quantity]         INT            NOT NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_PurchaseOrderDetails_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_PurchaseOrderDetails_OrderDetailID] PRIMARY KEY CLUSTERED ([PurchaseOrderDetailID])
)
GO

--------------------------------------------------------------------------------
-- 6. Sales tables (depend on Employees, Coupons, StockItems)
--------------------------------------------------------------------------------

CREATE TABLE [dbo].[Sales] (
    [SaleID]           INT            NOT NULL,
    [SaleDate]         DATETIME       NOT NULL CONSTRAINT [DF_Sales_SaleDate] DEFAULT (GETDATE()),
    [PaymentType]      NCHAR(1)       NOT NULL,
    [EmployeeID]       INT            NOT NULL,
    [TaxAmount]        SMALLMONEY     NOT NULL,
    [SubTotal]         MONEY          NOT NULL,
    [CouponID]         INT            NULL,
    [PaymentToken]     UNIQUEIDENTIFIER NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_Sales_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_Sales_SaleID] PRIMARY KEY CLUSTERED ([SaleID])
)
GO

CREATE TABLE [dbo].[SaleDetails] (
    [SaleDetailID]     INT            NOT NULL,
    [SaleID]           INT            NOT NULL,
    [StockItemID]      INT            NOT NULL,
    [SellingPrice]     SMALLMONEY     NOT NULL,
    [Quantity]         INT            NOT NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_SaleDetails_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_SaleDetails_SaleDetailID] PRIMARY KEY CLUSTERED ([SaleDetailID])
)
GO

CREATE TABLE [dbo].[SaleRefunds] (
    [SaleRefundID]     INT            NOT NULL,
    [SaleRefundDate]   DATETIME       NOT NULL CONSTRAINT [DF_SaleRefunds_SaleRefundDate] DEFAULT (GETDATE()),
    [SaleID]           INT            NOT NULL,
    [EmployeeID]       INT            NOT NULL,
    [TaxAmount]        MONEY          NOT NULL,
    [SubTotal]         MONEY          NOT NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_SaleRefunds_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_SaleRefunds_SaleRefundID] PRIMARY KEY CLUSTERED ([SaleRefundID])
)
GO

CREATE TABLE [dbo].[SaleRefundDetails] (
    [SaleRefundDetailID] INT          NOT NULL,
    [SaleRefundID]     INT            NOT NULL,
    [StockItemID]      INT            NOT NULL,
    [SellingPrice]     MONEY          NOT NULL,
    [Quantity]         INT            NOT NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_SaleRefundDetails_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_SaleRefundDetails_SaleRefundDetailID] PRIMARY KEY CLUSTERED ([SaleRefundDetailID])
)
GO

--------------------------------------------------------------------------------
-- 7. Receiving tables (depend on PurchaseOrders, PurchaseOrderDetails, Employees)
--------------------------------------------------------------------------------

CREATE TABLE [dbo].[ReceiveOrders] (
    [ReceiveOrderID]   INT            NOT NULL,
    [PurchaseOrderID]  INT            NOT NULL,
    [ReceiveDate]      DATETIME       NULL,
    [EmployeeID]       INT            NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_ReceiveOrders_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_ReceiveOrders_ReceiveOrderID] PRIMARY KEY CLUSTERED ([ReceiveOrderID])
)
GO

CREATE TABLE [dbo].[UnOrderedItems] (
    [UnOrderedItemID]  INT            NOT NULL,
    [ReceiveOrderID]   INT            NOT NULL,
    [ItemID]           INT            NOT NULL,
    [ItemName]         NVARCHAR(50)   NOT NULL,
    [VendorProductID]  NVARCHAR(25)   NOT NULL,
    [Quantity]         INT            NOT NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_UnOrderedItems_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_UnOrderedItems_ItemID] PRIMARY KEY CLUSTERED ([UnOrderedItemID])
)
GO

CREATE TABLE [dbo].[ReceiveOrderDetails] (
    [ReceiveOrderDetailID] INT        NOT NULL,
    [ReceiveOrderID]   INT            NOT NULL,
    [PurchaseOrderDetailID] INT       NOT NULL,
    [QuantityReceived] INT            NOT NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_ReceiveOrderDetails_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_ReceiveOrderDetails_ReceiveOrderDetailID] PRIMARY KEY CLUSTERED ([ReceiveOrderDetailID])
)
GO

CREATE TABLE [dbo].[ReturnedOrderDetails] (
    [ReturnedOrderDetailID] INT       NOT NULL,
    [ReceiveOrderID]   INT            NOT NULL,
    [PurchaseOrderDetailID] INT       NULL,
    [UnOrderedItemID]  INT            NULL,
    [ItemDescription]  NVARCHAR(50)   NULL,
    [Quantity]         INT            NOT NULL,
    [Reason]           NVARCHAR(50)   NOT NULL,
    [VendorStockNumber] NVARCHAR(15)  NULL,
    [RemoveFromViewFlag] BIT          NOT NULL CONSTRAINT [DF_ReturnedOrderDetails_RemoveFromViewFlag] DEFAULT (0),
    CONSTRAINT [PK_ReturnedOrderDetails_ReturnedOrderDetailID] PRIMARY KEY CLUSTERED ([ReturnedOrderDetailID])
)
GO

--------------------------------------------------------------------------------
-- 8. Foreign Key Constraints
--------------------------------------------------------------------------------

-- Employees -> Positions
ALTER TABLE [dbo].[Employees]
    ADD CONSTRAINT [FK_Employees_Positions_PositionID]
    FOREIGN KEY ([PositionID]) REFERENCES [dbo].[Positions] ([PositionID])
GO

-- Vendors -> Provinces
ALTER TABLE [dbo].[Vendors]
    ADD CONSTRAINT [FK_Vendors_Provinces_ProvinceID]
    FOREIGN KEY ([ProvinceID]) REFERENCES [dbo].[Provinces] ([ProvinceID])
GO

-- StockItems -> Categories
ALTER TABLE [dbo].[StockItems]
    ADD CONSTRAINT [FK_StockItems_Categories_CategoryID]
    FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Categories] ([CategoryID])
GO

-- StockItems -> Vendors
ALTER TABLE [dbo].[StockItems]
    ADD CONSTRAINT [FK_StockItems_Vendors_VendorID]
    FOREIGN KEY ([VendorID]) REFERENCES [dbo].[Vendors] ([VendorID])
GO

-- PurchaseOrders -> Employees
ALTER TABLE [dbo].[PurchaseOrders]
    ADD CONSTRAINT [FK_PurchaseOrders_Employees_EmployeeID]
    FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employees] ([EmployeeID])
GO

-- PurchaseOrders -> Vendors
ALTER TABLE [dbo].[PurchaseOrders]
    ADD CONSTRAINT [FK_PurchaseOrders_Vendors_VendorID]
    FOREIGN KEY ([VendorID]) REFERENCES [dbo].[Vendors] ([VendorID])
GO

-- PurchaseOrderDetails -> PurchaseOrders
ALTER TABLE [dbo].[PurchaseOrderDetails]
    ADD CONSTRAINT [FK_PurchaseOrderDetails_PurchaseOrders_OrderID]
    FOREIGN KEY ([PurchaseOrderID]) REFERENCES [dbo].[PurchaseOrders] ([PurchaseOrderID])
GO

-- PurchaseOrderDetails -> StockItems
ALTER TABLE [dbo].[PurchaseOrderDetails]
    ADD CONSTRAINT [FK_PurchaseOrderDetails_StockItems_StockItemID]
    FOREIGN KEY ([StockItemID]) REFERENCES [dbo].[StockItems] ([StockItemID])
GO

-- Sales -> Employees
ALTER TABLE [dbo].[Sales]
    ADD CONSTRAINT [FK_Sales_Employees_EmployeeID]
    FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employees] ([EmployeeID])
GO

-- Sales -> Coupons
ALTER TABLE [dbo].[Sales]
    ADD CONSTRAINT [FK_Sales_Coupons_CouponID]
    FOREIGN KEY ([CouponID]) REFERENCES [dbo].[Coupons] ([CouponID])
GO

-- SaleDetails -> Sales
ALTER TABLE [dbo].[SaleDetails]
    ADD CONSTRAINT [FK_SaleDetails_Sales_SaleID]
    FOREIGN KEY ([SaleID]) REFERENCES [dbo].[Sales] ([SaleID])
GO

-- SaleDetails -> StockItems
ALTER TABLE [dbo].[SaleDetails]
    ADD CONSTRAINT [FK_SaleDetails_StockItems_StockItemID]
    FOREIGN KEY ([StockItemID]) REFERENCES [dbo].[StockItems] ([StockItemID])
GO

-- SaleRefunds -> Employees
ALTER TABLE [dbo].[SaleRefunds]
    ADD CONSTRAINT [FK_SaleRefunds_Employees_EmployeeID]
    FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employees] ([EmployeeID])
GO

-- SaleRefunds -> Sales
ALTER TABLE [dbo].[SaleRefunds]
    ADD CONSTRAINT [FK_SaleRefunds_Sales_SaleID]
    FOREIGN KEY ([SaleID]) REFERENCES [dbo].[Sales] ([SaleID])
GO

-- SaleRefundDetails -> SaleRefunds
ALTER TABLE [dbo].[SaleRefundDetails]
    ADD CONSTRAINT [FK_SaleRefundDetails_SaleRefunds_SaleRefundID]
    FOREIGN KEY ([SaleRefundID]) REFERENCES [dbo].[SaleRefunds] ([SaleRefundID])
GO

-- SaleRefundDetails -> StockItems
ALTER TABLE [dbo].[SaleRefundDetails]
    ADD CONSTRAINT [FK_SaleRefundDetails_StockItems_StockItemID]
    FOREIGN KEY ([StockItemID]) REFERENCES [dbo].[StockItems] ([StockItemID])
GO

-- ReceiveOrders -> PurchaseOrders
ALTER TABLE [dbo].[ReceiveOrders]
    ADD CONSTRAINT [FK_ReceiveOrders_PurchaseOrders_OrderID]
    FOREIGN KEY ([PurchaseOrderID]) REFERENCES [dbo].[PurchaseOrders] ([PurchaseOrderID])
GO

-- ReceiveOrders -> Employees
ALTER TABLE [dbo].[ReceiveOrders]
    ADD CONSTRAINT [FK_ReceiveOrders_Employees_EmployeeID]
    FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employees] ([EmployeeID])
GO

-- ReceiveOrderDetails -> ReceiveOrders
ALTER TABLE [dbo].[ReceiveOrderDetails]
    ADD CONSTRAINT [FK_ReceiveOrderDetails_ReceiveOrders_ReceiveOrderID]
    FOREIGN KEY ([ReceiveOrderID]) REFERENCES [dbo].[ReceiveOrders] ([ReceiveOrderID])
GO

-- ReceiveOrderDetails -> PurchaseOrderDetails
ALTER TABLE [dbo].[ReceiveOrderDetails]
    ADD CONSTRAINT [FK_ReceiveOrderDetails_PurchaseOrderDetails_OrderDetailID]
    FOREIGN KEY ([PurchaseOrderDetailID]) REFERENCES [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID])
GO

-- UnOrderedItems -> ReceiveOrders
ALTER TABLE [dbo].[UnOrderedItems]
    ADD CONSTRAINT [FK_UnOrderedItems_ReceiveOrders_ReceiveOrderID]
    FOREIGN KEY ([ReceiveOrderID]) REFERENCES [dbo].[ReceiveOrders] ([ReceiveOrderID])
GO

-- ReturnedOrderDetails -> ReceiveOrders
ALTER TABLE [dbo].[ReturnedOrderDetails]
    ADD CONSTRAINT [FK_ReturnedOrderDetails_ReceiveOrders_ReceiveOrder]
    FOREIGN KEY ([ReceiveOrderID]) REFERENCES [dbo].[ReceiveOrders] ([ReceiveOrderID])
GO

-- ReturnedOrderDetails -> PurchaseOrderDetails
ALTER TABLE [dbo].[ReturnedOrderDetails]
    ADD CONSTRAINT [FK_ReturnedOrderDetails_PurchaseOrderDetails_OrderDetailID]
    FOREIGN KEY ([PurchaseOrderDetailID]) REFERENCES [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID])
GO

-- ReturnedOrderDetails -> UnOrderedItems
ALTER TABLE [dbo].[ReturnedOrderDetails]
    ADD CONSTRAINT [FK_ReturnedOrderDetails_UnOrderedItems_UnOrderedItemID]
    FOREIGN KEY ([UnOrderedItemID]) REFERENCES [dbo].[UnOrderedItems] ([UnOrderedItemID])
GO

PRINT 'Database [eTools2023] created successfully with all tables and constraints.'
GO

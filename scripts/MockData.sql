--------------------------------------------------------------------------------
-- MockData.sql
-- Populates the eTools2023 database with realistic mock data for all 17 tables.
-- Run after CreateDatabase.sql
--------------------------------------------------------------------------------

USE [eTools2023]
GO

SET NOCOUNT ON;

--------------------------------------------------------------------------------
-- 1. Positions
--------------------------------------------------------------------------------

INSERT INTO [dbo].[Positions] ([PositionID], [Description], [RemoveFromViewFlag]) VALUES
(1,  N'Store Manager',            0),
(2,  N'Assistant Manager',        0),
(3,  N'Sales Associate',          0),
(4,  N'Purchasing Agent',         0),
(5,  N'Receiving Clerk',          0),
(6,  N'Cashier',                  0),
(7,  N'Inventory Specialist',     0),
(8,  N'Customer Service Rep',     0),
(9,  N'Warehouse Worker',         0),
(10, N'Transferred',              1);
GO

--------------------------------------------------------------------------------
-- 2. Provinces
--------------------------------------------------------------------------------

INSERT INTO [dbo].[Provinces] ([ProvinceID], [Description], [RemoveFromViewFlag]) VALUES
('AB', N'Alberta',            0),
('BC', N'British Columbia',   0),
('MB', N'Manitoba',           0),
('NB', N'New Brunswick',      0),
('NL', N'Newfoundland and Labrador', 0),
('NS', N'Nova Scotia',        0),
('ON', N'Ontario',            0),
('PE', N'Prince Edward Island', 0),
('QC', N'Quebec',             0),
('SK', N'Saskatchewan',       0);
GO

--------------------------------------------------------------------------------
-- 3. Categories
--------------------------------------------------------------------------------

INSERT INTO [dbo].[Categories] ([CategoryID], [Description], [RemoveFromViewFlag]) VALUES
(1,  N'Hand Tools',           0),
(2,  N'Power Tools',          0),
(3,  N'Fasteners',            0),
(4,  N'Plumbing',             0),
(5,  N'Electrical',           0),
(6,  N'Lumber & Building Materials', 0),
(7,  N'Paint & Stains',       0),
(8,  N'Safety Equipment',     0),
(9,  N'Garden & Outdoor',     0),
(10, N'Hardware & Fittings',  0),
(11, N'Cordless Tool Accessories', 1);
GO

--------------------------------------------------------------------------------
-- 4. Coupons
--------------------------------------------------------------------------------

INSERT INTO [dbo].[Coupons] ([CouponID], [CouponIDValue], [StartDate], [EndDate], [CouponDiscount], [RemoveFromViewFlag]) VALUES
(1, N'SAVE10',    '2023-01-01', '2023-12-31', 10, 0),
(2, N'WELCOME15', '2023-03-01', '2023-06-30', 15, 0),
(3, N'FALL20',    '2023-09-01', '2023-11-30', 20, 0),
(4, N'NEWYEAR5',  '2024-01-01', '2024-01-31',  5, 0),
(5, N'CLOSEOUT25','2022-06-01', '2022-08-31', 25, 1);
GO

--------------------------------------------------------------------------------
-- 5. Employees
--------------------------------------------------------------------------------

INSERT INTO [dbo].[Employees] ([EmployeeID], [FirstName], [LastName], [DateHired], [DateReleased], [PositionID], [LoginID], [Address], [City], [ContactPhone], [PostalCode], [RemoveFromViewFlag]) VALUES
(1,  N'James',    N'Mitchell',   '2018-04-15', NULL,            1, N'jmitchell',  N'100 Main St',       N'Toronto',        N'4165550101', N'M5V2T6', 0),
(2,  N'Sarah',    N'Chen',       '2019-06-01', NULL,            2, N'schen',      N'250 Dundas St W',   N'Toronto',        N'4165550102', N'M5H2J2', 0),
(3,  N'Michael',  N'Brooks',     '2020-01-10', NULL,            4, N'mbrooks',    N'789 Bloor St W',    N'Toronto',        N'4165550103', N'M6G1L1', 0),
(4,  N'Emily',    N'Patel',      '2020-08-20', NULL,            3, N'epatel',     N'42 King St E',      N'Mississauga',    N'9055550104', N'L5H1A1', 0),
(5,  N'David',    N'Kowalski',   '2021-03-05', NULL,            5, N'dkowalski',  N'1500 Eglinton Ave', N'Scarborough',    N'4165550105', N'M1P2P5', 0),
(6,  N'Lisa',     N'Nguyen',     '2021-05-15', NULL,            6, N'lnguyen',    N'3300 Dufferin St',  N'Toronto',        N'4165550106', N'M6A2T3', 0),
(7,  N'Robert',   N'Thompson',   '2021-09-01', NULL,            3, N'rthompson',  N'88 Lakeshore Blvd', N'Toronto',        N'4165550107', N'M5V3R9', 0),
(8,  N'Amanda',   N'Garcia',     '2022-01-15', NULL,            7, N'agarcia',    N'600 Kennedy Rd',    N'Scarborough',    N'4165550108', N'M1K2B8', 0),
(9,  N'Chris',    N'MacDonald',  '2019-11-20', '2023-02-28',    3, N'cmacdonald', N'4500 Sheppard Ave', N'Toronto',        N'4165550109', N'M1S1R4', 1),
(10, N'Priya',    N'Sharma',     '2022-06-10', NULL,            8, N'psharma',    N'200 Victoria St',   N'Toronto',        N'4165550110', N'M5B1V8', 0),
(11, N'Marcus',   N'Wright',     '2022-09-01', NULL,            9, N'mwright',    N'1200 Wilson Ave',   N'North York',     N'4165550111', N'M3J2A2', 0),
(12, N'Olivia',   N'Drummond',   '2023-01-09', NULL,            3, N'odrummond',  N'750 Bay St',        N'Toronto',        N'4165550112', N'M5G1R5', 0);
GO

--------------------------------------------------------------------------------
-- 6. Vendors
--------------------------------------------------------------------------------

INSERT INTO [dbo].[Vendors] ([VendorID], [VendorName], [Phone], [Address], [City], [ProvinceID], [PostalCode], [RemoveFromViewFlag]) VALUES
(1,  N'ToolMaster Canada',     N'18005551001', N'500 Industry Rd',    N'Mississauga',  'ON', N'L5B3R2', 0),
(2,  N'Northern Supply Co',    N'18005551002', N'1200 Rutherford Rd', N'Brampton',     'ON', N'L6T1A3', 0),
(3,  N'Pacific Hardware Ltd',  N'18005551003', N'800 Terminal Ave',   N'Vancouver',    'BC', N'V6A2M7', 0),
(4,  N'Prairie Fasteners Inc', N'18005551004', N'300 Grain Exchange', N'Winnipeg',     'MB', N'R3B0R3', 0),
(5,  N'East Coast Building',   N'18005551005', N'150 Waterfront Ln',  N'Halifax',      'NS', N'B3H1B1', 0),
(6,  N'Atlas Tools Group',     N'18005551006', N'2000 Industrial Blvd', N'Scarborough','ON', N'M1N2E4', 0),
(7,  N'Quebec Paint & Supply', N'18005551007', N'450 St Laurent Blvd', N'Montreal',    'QC', N'H2X2Y6', 0),
(8,  N'Alberta Industrial',    N'18005551008', N'700 Eagle Rd',        N'Edmonton',     'AB', N'T5K2J1', 0),
(9,  N'Maritime Plumbing Supply', N'18005551009', N'100 Harbour View', N'Saint John',   'NB', N'E2L3T4', 0),
(10, N'Saskatchewan Lumber',   N'18005551010', N'600 Railway Ave',     N'Saskatoon',    'SK', N'S7K1C5', 0);
GO

--------------------------------------------------------------------------------
-- 7. StockItems
--------------------------------------------------------------------------------

INSERT INTO [dbo].[StockItems] ([StockItemID], [Description], [SellingPrice], [PurchasePrice], [QuantityOnHand], [QuantityOnOrder], [ReOrderLevel], [Discontinued], [VendorID], [VendorStockNumber], [CategoryID], [RemoveFromViewFlag]) VALUES
(1,  N'Claw Hammer 16oz',         19.99,   12.50,  48,  0,  15, 0, 1, N'TM-1001',  1, 0),
(2,  N'Combination Wrench Set',   34.99,   21.00,  25,  10, 10, 0, 1, N'TM-1002',  1, 0),
(3,  N'Cordless Drill 20V',       149.99,  89.00,  12,  5,  8,  0, 6, N'AT-2001',  2, 0),
(4,  N'Impact Driver 20V',        129.99,  78.50,  15,  5,  8,  0, 6, N'AT-2002',  2, 0),
(5,  N'Battery Charger 20V',      59.99,   35.00,  30,  0,  10, 0, 6, N'AT-2003',  2, 0),
(6,  N'Box of Wood Screws #8 x 2"', 4.99,  2.50,  200, 50,  50, 0, 4, N'PF-3001',  3, 0),
(7,  N'Box of Drywall Screws #6 x 1-1/4"', 3.99, 1.75, 300, 100, 75, 0, 4, N'PF-3002', 3, 0),
(8,  N'Copper Elbow 1/2" 90deg',  2.49,    1.20,  150, 0,   40, 0, 9, N'MP-4001',  4, 0),
(9,  N'PVC Pipe 1/2" x 10ft',     5.99,    3.25,  80,  0,   25, 0, 9, N'MP-4002',  4, 0),
(10, N'Wire Nuts Yellow 25pk',    3.49,    1.80,  100, 0,   30, 0, 2, N'NS-5001',  5, 0),
(11, N'Electrical Tape 1" x 60ft', 2.99,   1.50,  120, 0,   30, 0, 2, N'NS-5002',  5, 0),
(12, N'2x4x8 Stud',               5.49,    3.80,  500, 200, 100, 0, 10, N'SL-6001',  6, 0),
(13, N'1/2" Plywood Sheet 4x8',   42.99,   29.00, 60,  20,  20, 0, 10, N'SL-6002',  6, 0),
(14, N'Interior Latex Paint 1L',   32.99,   18.50, 45,  0,   15, 0, 7, N'QP-7001',  7, 0),
(15, N'Wood Stain 500mL',          18.99,   10.00, 35,  0,   10, 0, 7, N'QP-7002',  7, 0),
(16, N'Safety Glasses (Clear)',    6.99,    3.50,  80,  0,   25, 0, 1, N'TM-1003',  8, 0),
(17, N'Work Gloves Medium',        12.99,   7.00,  60,  0,   20, 0, 1, N'TM-1004',  8, 0),
(18, N'Garden Hose 50ft',          39.99,   24.00, 20,  5,   10, 0, 8, N'AI-8001',  9, 0),
(19, N'Phillips Screwdriver Set',  15.99,   9.00,  40,  0,   15, 0, 6, N'AT-2004',  1, 0),
(20, N'Adjustable Wrench 12"',     22.99,   14.00, 30,  0,   10, 0, 1, N'TM-1005',  1, 0),
(21, N'Hacksaw',                   14.99,   8.50,  22,  0,   10, 0, 6, N'AT-2005',  1, 0),
(22, N'Level 24" Aluminum',        29.99,   17.00, 18,  0,   8,  0, 6, N'AT-2006',  1, 0),
(23, N'Tape Measure 25ft',         9.99,    5.50,  55,  0,   20, 0, 1, N'TM-1006',  1, 0),
(24, N'Nut & Bolt Assortment Kit', 24.99,   15.00, 35,  10,  10, 0, 4, N'PF-3003',  10, 0),
(25, N'Door Hinge Set (3-pack)',   8.99,    5.00,  45,  0,   15, 0, 4, N'PF-3004',  10, 0),
(26, N'Extension Cord 50ft 14AWG', 34.99,   20.00, 14,  5,   8,  0, 2, N'NS-5003',  5, 0),
(27, N'Wood Glue 500mL',           8.99,    4.75,  65,  0,   20, 0, 1, N'TM-1007',  1, 0),
(28, N'Circular Saw 7-1/4"',       89.99,   55.00, 8,   3,   5,  0, 6, N'AT-2007',  2, 0),
(29, N'Jigsaw',                    69.99,   42.00, 10,  3,   5,  0, 6, N'AT-2008',  2, 0),
(30, N'Router 1/4"',               119.99,  72.00, 6,   2,   4,  0, 6, N'AT-2009',  2, 0),
(31, N'Outdoor Wood Stain 3.8L',   44.99,   28.00, 0,   0,   10, 1, 7, N'QP-7003',  7, 1),
(32, N'Fluorescent Bulb 4ft',      7.99,    4.25,  90,  0,   30, 0, 2, N'NS-5004',  5, 0);
GO

--------------------------------------------------------------------------------
-- 8. PurchaseOrders
--------------------------------------------------------------------------------

INSERT INTO [dbo].[PurchaseOrders] ([PurchaseOrderID], [OrderDate], [VendorID], [EmployeeID], [TaxAmount], [SubTotal], [Closed], [Notes], [RemoveFromViewFlag]) VALUES
(1,  '2023-01-15', 1, 3,  27.30,  175.00, 1, N'Restock hand tools',                    0),
(2,  '2023-02-20', 6, 3,  143.10,  925.00, 1, N'Power tools replenishment',              0),
(3,  '2023-03-05', 4, 3,  18.60,   120.00, 1, N'Fastener reorder',                       0),
(4,  '2023-04-10', 2, 3,  64.35,   415.00, 1, N'Electrical supplies',                    0),
(5,  '2023-05-18', 10,3,  93.30,   600.00, 1, N'Lumber restocking',                      0),
(6,  '2023-06-25', 7, 3,  46.80,   300.00, 1, N'Paint and stain',                        0),
(7,  '2023-07-12', 1, 3,  42.00,   270.00, 1, N'Safety gear + hand tools',               0),
(8,  '2023-08-01', 8, 3,  61.95,   399.00, 1, N'Garden items for spring closeout',       0),
(9,  '2023-09-10', 6, 3,  188.25, 1215.00, 1, N'Big power tool order',                   0),
(10, '2023-10-05', 3, 3,  53.70,   345.00, 0, N'Pending delivery - West Coast supplier', 0),
(11, '2023-11-15', 1, 3,  31.20,   200.00, 0, N'Late fall hand tool restock',            0),
(12, '2023-12-01', 9, 3,  23.40,   150.00, 1, N'Plumbing supplies',                      0);
GO

--------------------------------------------------------------------------------
-- 9. PurchaseOrderDetails
--------------------------------------------------------------------------------

INSERT INTO [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [StockItemID], [PurchasePrice], [Quantity], [RemoveFromViewFlag]) VALUES
(1,  1,  1,  12.50, 10, 0),
(2,  1,  2,  21.00,  5, 0),
(3,  1,  23, 5.50, 10, 0),
(4,  2,  3,  89.00,  5, 0),
(5,  2,  4,  78.50,  5, 0),
(6,  2,  28, 55.00,  3, 0),
(7,  3,  6,  2.50, 25, 0),
(8,  3,  7,  1.75, 50, 0),
(9,  3,  24, 15.00, 10, 0),
(10, 4,  10, 1.80, 50, 0),
(11, 4,  11, 1.50, 50, 0),
(12, 4,  26, 20.00,  5, 0),
(13, 5,  12, 3.80,100, 0),
(14, 5,  13, 29.00, 10, 0),
(15, 6,  14, 18.50, 15, 0),
(16, 6,  15, 10.00, 10, 0),
(17, 7,  16, 3.50, 20, 0),
(18, 7,  17, 7.00, 15, 0),
(19, 7,  1,  12.50, 10, 0),
(20, 8,  18, 24.00, 10, 0),
(21, 8,  27, 4.75, 15, 0),
(22, 9,  3,  89.00,  5, 0),
(23, 9,  29, 42.00,  5, 0),
(24, 9,  30, 72.00,  3, 0),
(25, 10, 8,  1.20,100, 0),
(26, 10, 9,  3.25, 50, 0),
(27, 11, 19, 9.00, 10, 0),
(28, 11, 20, 14.00,  5, 0),
(29, 11, 22, 17.00,  5, 0),
(30, 12, 8,  1.20, 50, 0),
(31, 12, 9,  3.25, 25, 0);
GO

--------------------------------------------------------------------------------
-- 10. Sales
--------------------------------------------------------------------------------

INSERT INTO [dbo].[Sales] ([SaleID], [SaleDate], [PaymentType], [EmployeeID], [TaxAmount], [SubTotal], [CouponID], [PaymentToken], [RemoveFromViewFlag]) VALUES
(1,  '2023-03-10 09:15:00', N'C', 4,  8.25,  56.96, NULL, NULL, 0),
(2,  '2023-03-12 14:30:00', N'D', 7,  4.50,  30.97, 1,    NULL, 0),
(3,  '2023-04-01 11:00:00', N'C', 4, 14.24,  98.95, NULL, NULL, 0),
(4,  '2023-04-15 16:45:00', N'V', 12, 6.00,  41.97, NULL, 'A1B2C3D4-5678-9ABC-DEF0-1234567890AB', 0),
(5,  '2023-05-03 10:20:00', N'D', 4, 12.74,  88.96, 2,    NULL, 0),
(6,  '2023-05-20 13:10:00', N'C', 7,  3.00,  20.97, NULL, NULL, 0),
(7,  '2023-06-01 09:00:00', N'V', 4, 18.00, 125.96, NULL, 'B2C3D4E5-6789-ABCD-EF01-2345678901BC', 0),
(8,  '2023-06-15 15:30:00', N'C', 12, 5.25,  36.96, 3,    NULL, 0),
(9,  '2023-07-04 11:15:00', N'D', 4, 21.74, 151.96, NULL, NULL, 0),
(10, '2023-07-20 14:00:00', N'C', 7,  9.00,  62.97, NULL, NULL, 0),
(11, '2023-08-05 10:45:00', N'V', 4, 15.74, 110.95, NULL, 'C3D4E5F6-789A-BCDE-F012-3456789012CD', 0),
(12, '2023-08-22 16:00:00', N'D', 12, 3.75,  25.97, NULL, NULL, 0),
(13, '2023-09-10 09:30:00', N'C', 4, 10.50,  72.96, 1,    NULL, 0),
(14, '2023-09-28 13:45:00', N'C', 7,  6.00,  41.97, NULL, NULL, 0),
(15, '2023-10-15 11:00:00', N'V', 4, 24.74, 172.96, NULL, 'D4E5F6A7-89AB-CDEF-0123-4567890123DE', 0),
(16, '2023-11-01 10:15:00', N'C', 12, 4.50,  30.97, 4,    NULL, 0),
(17, '2023-11-20 14:30:00', N'D', 4,  7.50,  51.96, NULL, NULL, 0),
(18, '2023-12-05 09:45:00', N'C', 7, 12.00,  83.96, NULL, NULL, 0);
GO

--------------------------------------------------------------------------------
-- 11. SaleDetails
--------------------------------------------------------------------------------

INSERT INTO [dbo].[SaleDetails] ([SaleDetailID], [SaleID], [StockItemID], [SellingPrice], [Quantity], [RemoveFromViewFlag]) VALUES
(1,  1,  1,  19.99, 2, 0),
(2,  1,  23, 9.99,  1, 0),
(3,  2,  6,   4.99, 3, 0),
(4,  2,  10,  3.49, 2, 0),
(5,  3,  3, 149.99, 1, 0),
(6,  3,  5,  59.99, 1, 0),
(7,  4,  16,  6.99, 3, 0),
(8,  4,  17, 12.99, 1, 0),
(9,  5,  19, 15.99, 2, 0),
(10, 5,  27,  8.99, 2, 0),
(11, 6,  8,   2.49, 4, 0),
(12, 6,  11,  2.99, 3, 0),
(13, 7,  28, 89.99, 1, 0),
(14, 7,  2,  34.99, 1, 0),
(15, 8,  6,   4.99, 5, 0),
(16, 9,  3, 149.99, 1, 0),
(17, 9,  19, 15.99, 1, 0),
(18, 10, 14, 32.99, 1, 0),
(19, 10, 25,  8.99, 2, 0),
(20, 11, 29, 69.99, 1, 0),
(21, 11, 23,  9.99, 2, 0),
(22, 12, 6,   4.99, 2, 0),
(23, 12, 10,  3.49, 2, 0),
(24, 13, 20, 22.99, 2, 0),
(25, 13, 16,  6.99, 1, 0),
(26, 14, 12,  5.49, 4, 0),
(27, 14, 14, 32.99, 1, 0),
(28, 15, 30,119.99, 1, 0),
(29, 15, 2,  34.99, 1, 0),
(30, 16, 8,   2.49, 5, 0),
(31, 16, 9,   5.99, 2, 0),
(32, 17, 22, 29.99, 1, 0),
(33, 17, 24, 24.99, 1, 0),
(34, 18, 19, 15.99, 2, 0),
(35, 18, 17, 12.99, 2, 0);
GO

--------------------------------------------------------------------------------
-- 12. SaleRefunds
--------------------------------------------------------------------------------

INSERT INTO [dbo].[SaleRefunds] ([SaleRefundID], [SaleRefundDate], [SaleID], [EmployeeID], [TaxAmount], [SubTotal], [RemoveFromViewFlag]) VALUES
(1, '2023-04-20 10:00:00', 2,  4,  1.50, 10.48, 0),
(2, '2023-06-10 14:15:00', 6,  7,  1.00,  6.98, 0),
(3, '2023-08-30 11:30:00', 10, 4,  4.50, 31.48, 0),
(4, '2023-10-25 09:45:00', 14, 12, 3.00, 20.98, 0);
GO

--------------------------------------------------------------------------------
-- 13. SaleRefundDetails
--------------------------------------------------------------------------------

INSERT INTO [dbo].[SaleRefundDetails] ([SaleRefundDetailID], [SaleRefundID], [StockItemID], [SellingPrice], [Quantity], [RemoveFromViewFlag]) VALUES
(1, 1, 6,   4.99, 1, 0),
(2, 1, 10,  3.49, 1, 0),
(3, 2, 8,   2.49, 1, 0),
(4, 2, 11,  2.99, 1, 0),
(5, 3, 14, 32.99, 1, 0),
(6, 4, 12,  5.49, 2, 0);
GO

--------------------------------------------------------------------------------
-- 14. ReceiveOrders
--------------------------------------------------------------------------------

INSERT INTO [dbo].[ReceiveOrders] ([ReceiveOrderID], [PurchaseOrderID], [ReceiveDate], [EmployeeID], [RemoveFromViewFlag]) VALUES
(1,  1,  '2023-01-22 08:30:00', 5, 0),
(2,  2,  '2023-02-28 09:00:00', 5, 0),
(3,  3,  '2023-03-12 10:15:00', 5, 0),
(4,  4,  '2023-04-18 08:45:00', 5, 0),
(5,  5,  '2023-05-25 09:30:00', 11,0),
(6,  6,  '2023-07-02 11:00:00', 5, 0),
(7,  7,  '2023-07-20 08:00:00', 11,0),
(8,  8,  '2023-08-10 10:30:00', 5, 0),
(9,  9,  '2023-09-18 09:15:00', 5, 0),
(10, 12, '2023-12-08 08:30:00', 11,0);
GO

--------------------------------------------------------------------------------
-- 15. UnOrderedItems
--------------------------------------------------------------------------------

INSERT INTO [dbo].[UnOrderedItems] ([UnOrderedItemID], [ReceiveOrderID], [ItemID], [ItemName], [VendorProductID], [Quantity], [RemoveFromViewFlag]) VALUES
(1, 9, 99001, N'Sanding Disc 5"',           N'AT-UN-001', 10, 0),
(2, 9, 99002, N'Cordless Reciprocating Saw', N'AT-UN-002',  2, 0),
(3, 7, 99003, N'Utility Knife Blades 10pk', N'TM-UN-001', 20, 0);
GO

--------------------------------------------------------------------------------
-- 16. ReceiveOrderDetails
--------------------------------------------------------------------------------

INSERT INTO [dbo].[ReceiveOrderDetails] ([ReceiveOrderDetailID], [ReceiveOrderID], [PurchaseOrderDetailID], [QuantityReceived], [RemoveFromViewFlag]) VALUES
(1,  1,  1,  10, 0),
(2,  1,  2,   5, 0),
(3,  1,  3,  10, 0),
(4,  2,  4,   5, 0),
(5,  2,  5,   5, 0),
(6,  2,  6,   3, 0),
(7,  3,  7,  25, 0),
(8,  3,  8,  50, 0),
(9,  3,  9,  10, 0),
(10, 4, 10,  50, 0),
(11, 4, 11,  50, 0),
(12, 4, 12,   5, 0),
(13, 5, 13, 100, 0),
(14, 5, 14,  10, 0),
(15, 6, 15,  15, 0),
(16, 6, 16,  10, 0),
(17, 7, 17,  20, 0),
(18, 7, 18,  15, 0),
(19, 7, 19,  10, 0),
(20, 8, 20,  10, 0),
(21, 8, 21,  15, 0),
(22, 9, 22,   5, 0),
(23, 9, 23,   5, 0),
(24, 9, 24,   3, 0),
(25, 10,30,  50, 0),
(26, 10,31,  25, 0);
GO

--------------------------------------------------------------------------------
-- 17. ReturnedOrderDetails
--------------------------------------------------------------------------------

INSERT INTO [dbo].[ReturnedOrderDetails] ([ReturnedOrderDetailID], [ReceiveOrderID], [PurchaseOrderDetailID], [UnOrderedItemID], [ItemDescription], [Quantity], [Reason], [VendorStockNumber], [RemoveFromViewFlag]) VALUES
(1, 9, 23, NULL, NULL,                  1, N'Damaged during shipping',       N'AT-2002', 0),
(2, 9, NULL, 2,   N'Cordless Reciprocating Saw', 1, N'Wrong item shipped',  N'AT-UN-002', 0),
(3, 7, 19, NULL, NULL,                  2, N'Defective product',             N'TM-1001', 0);
GO

PRINT 'Mock data inserted successfully into [eTools2023].'
GO

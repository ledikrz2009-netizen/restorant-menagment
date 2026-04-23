IF DB_ID('RestaurantManagementDb') IS NULL
BEGIN
    CREATE DATABASE RestaurantManagementDb;
END
GO

USE RestaurantManagementDb;
GO

IF OBJECT_ID('Payments','U') IS NOT NULL DROP TABLE Payments;
IF OBJECT_ID('OrderItems','U') IS NOT NULL DROP TABLE OrderItems;
IF OBJECT_ID('Orders','U') IS NOT NULL DROP TABLE Orders;
IF OBJECT_ID('StaffSchedules','U') IS NOT NULL DROP TABLE StaffSchedules;
IF OBJECT_ID('Waiters','U') IS NOT NULL DROP TABLE Waiters;
IF OBJECT_ID('MenuItems','U') IS NOT NULL DROP TABLE MenuItems;
IF OBJECT_ID('MenuCategories','U') IS NOT NULL DROP TABLE MenuCategories;
IF OBJECT_ID('RestaurantTables','U') IS NOT NULL DROP TABLE RestaurantTables;
IF OBJECT_ID('Users','U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('Roles','U') IS NOT NULL DROP TABLE Roles;
GO

CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(300) NOT NULL,
    RoleId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CardUid NVARCHAR(100) NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY(RoleId) REFERENCES Roles(RoleId)
);

CREATE TABLE Waiters (
    WaiterId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    FullName NVARCHAR(120) NOT NULL,
    Phone NVARCHAR(50) NULL,
    Notes NVARCHAR(300) NULL,
    CONSTRAINT FK_Waiters_Users FOREIGN KEY(UserId) REFERENCES Users(UserId)
);

CREATE TABLE StaffSchedules (
    ScheduleId INT IDENTITY(1,1) PRIMARY KEY,
    WaiterId INT NOT NULL,
    WorkDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    ShiftName NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_StaffSchedules_Waiters FOREIGN KEY(WaiterId) REFERENCES Waiters(WaiterId)
);

CREATE TABLE RestaurantTables (
    TableId INT IDENTITY(1,1) PRIMARY KEY,
    TableName NVARCHAR(50) NOT NULL UNIQUE,
    Status INT NOT NULL
);

CREATE TABLE MenuCategories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE MenuItems (
    MenuItemId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryId INT NOT NULL,
    Name NVARCHAR(120) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_MenuItems_MenuCategories FOREIGN KEY(CategoryId) REFERENCES MenuCategories(CategoryId)
);

CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    TableId INT NOT NULL,
    WaiterId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    ClosedAt DATETIME2 NULL,
    Status INT NOT NULL,
    CONSTRAINT FK_Orders_Tables FOREIGN KEY(TableId) REFERENCES RestaurantTables(TableId),
    CONSTRAINT FK_Orders_Waiters FOREIGN KEY(WaiterId) REFERENCES Waiters(WaiterId)
);

CREATE TABLE OrderItems (
    OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    MenuItemId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY(OrderId) REFERENCES Orders(OrderId),
    CONSTRAINT FK_OrderItems_MenuItems FOREIGN KEY(MenuItemId) REFERENCES MenuItems(MenuItemId)
);

CREATE TABLE Payments (
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL UNIQUE,
    AmountPaid DECIMAL(10,2) NOT NULL,
    CashReceived DECIMAL(10,2) NOT NULL,
    ChangeAmount DECIMAL(10,2) NOT NULL,
    PaidAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Payments_Orders FOREIGN KEY(OrderId) REFERENCES Orders(OrderId)
);
GO

INSERT INTO Roles(Name) VALUES('Admin'),('Waiter');

-- Password hash in app = Base64 of plain text (for demo deploy)
INSERT INTO Users(Username, PasswordHash, RoleId, IsActive, CardUid)
VALUES
('admin', 'YWRtaW4xMjM=', 1, 1, NULL),
('waiter1', 'd2FpdGVyMTIz', 2, 1, 'MOCK-WAITER-CARD-001');

INSERT INTO Waiters(UserId, FullName, Phone, Notes)
VALUES
((SELECT UserId FROM Users WHERE Username='waiter1'), 'Ardit Kola', '+355691112233', 'Turni i paradites');

INSERT INTO StaffSchedules(WaiterId, WorkDate, StartTime, EndTime, ShiftName)
VALUES
(1, CAST(GETDATE() AS DATE), '08:00', '16:00', 'Paradite');

INSERT INTO RestaurantTables(TableName, Status)
VALUES ('T1',1),('T2',1),('T3',1),('T4',1),('T5',1);

INSERT INTO MenuCategories(Name) VALUES('Pije'),('Ushqim');

INSERT INTO MenuItems(CategoryId, Name, Price, IsActive)
VALUES
(1, 'Kafe', 120, 1),
(1, 'Uje 0.5L', 80, 1),
(2, 'Pizza Margherita', 650, 1),
(2, 'Pasta Carbonara', 700, 1);
GO

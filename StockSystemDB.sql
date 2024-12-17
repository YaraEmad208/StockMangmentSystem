create database StockMangmentSystem 

use StockMangmentSystem 

CREATE TABLE Supplier (
    SupplierId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Contact NVARCHAR(50),
    Email NVARCHAR(100),
    Address NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE()
);


CREATE TABLE Product (
    ProductId INT PRIMARY KEY IDENTITY(1,1),
    SupplierId INT NULL FOREIGN KEY REFERENCES Supplier(SupplierId),
    Name NVARCHAR(100) NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);


-- User Table (Already Defined)
CREATE TABLE [User] (
    ID INT NOT NULL PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Email NVARCHAR(50) NOT NULL,
    Password NVARCHAR(50) NOT NULL,
    Phone NVARCHAR(40),
    Role NVARCHAR(40)
);

-- Order Table with Foreign Key to User
CREATE TABLE [Order] (
    OrderId INT NOT NULL PRIMARY KEY IDENTITY,
    UserId INT NOT NULL, -- Foreign key to User table
    TotalPrice DECIMAL(18, 2) NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    CONSTRAINT FK_Order_User FOREIGN KEY (UserId) REFERENCES [User](ID) ON DELETE CASCADE
);

-- OrderDetails Table with Foreign Keys to Order and Product
CREATE TABLE [OrderDetails] (
    OrderDetailId INT NOT NULL PRIMARY KEY IDENTITY,
    OrderId INT NOT NULL, -- Foreign key to Order table
    ProductId INT NOT NULL, -- Foreign key to Product table
    Quantity INT NOT NULL,
    PricePerUnit DECIMAL(18, 2) NOT NULL,
    CONSTRAINT FK_OrderDetails_Order FOREIGN KEY (OrderId) REFERENCES [Order](OrderId) ON DELETE CASCADE,
    CONSTRAINT FK_OrderDetails_Product FOREIGN KEY (ProductId) REFERENCES Product(ProductId) ON DELETE CASCADE
);



-- Insert Dummy Data into Supplier
INSERT INTO Supplier (Name, Contact, Email, Address)
VALUES 
('TechWorld Suppliers', '123-456-7890', 'contact@techworld.com', '123 Electronics Blvd'),
('GadgetPro Distributors', '987-654-3210', 'support@gadgetpro.com', '456 Tech Avenue'),
('FutureGear Inc.', '555-555-5555', 'sales@futuregear.com', '789 Innovation St');

-- Insert Dummy Data into Product
INSERT INTO Product (SupplierId, Name, Quantity, Price)
VALUES 
(1, 'Smartphone Model A', 50, 699.99),
(1, 'Laptop Pro 15"', 30, 1299.49),
(2, 'Wireless Headphones', 100, 199.99),
(2, 'Bluetooth Speaker', 75, 99.99),
(3, '4K LED TV 55"', 20, 799.99),
(3, 'Gaming Console X', 40, 499.99);

-- Insert Dummy Data into [User]
INSERT INTO [User] (ID, Name, Email, Password, Phone, Role)
VALUES
(1, 'Alice Cooper', 'alice.cooper@example.com', 'aliceSecurePass', '123-456-7890', 'Customer'),
(2, 'Bob Carter', 'bob.carter@example.com', 'bobPassword123', '987-654-3210', 'Admin'),
(3, 'Charlie Brown', 'charlie.brown@example.com', 'charlieSecret!', '555-555-1234', 'Customer');


-- Insert Dummy Data into [Order]
INSERT INTO [Order] (UserId, TotalPrice, Status)
VALUES
(1, 1399.98, 'Completed'), -- Smartphone + Headphones
(1, 1999.48, 'Pending'),   -- Laptop Pro + Speaker
(3, 1299.98, 'Completed'); -- 4K TV + Console

-- Insert Dummy Data into [OrderDetails]
INSERT INTO [OrderDetails] (OrderId, ProductId, Quantity, PricePerUnit)
VALUES
(1, 1, 1, 699.99), -- 1 x Smartphone Model A
(1, 3, 1, 199.99), -- 1 x Wireless Headphones
(2, 2, 1, 1299.49), -- 1 x Laptop Pro 15"
(2, 4, 1, 99.99), -- 1 x Bluetooth Speaker
(3, 5, 1, 799.99), -- 1 x 4K LED TV 55"
(3, 6, 1, 499.99); -- 1 x Gaming Console X

-- Insert Roles if they don't exist
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Admin')
BEGIN
    INSERT INTO Roles (Name, Description)
    VALUES ('Admin', 'Administrator with full access');
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Customer')
BEGIN
    INSERT INTO Roles (Name, Description)
    VALUES ('Customer', 'Regular customer');
END

-- Insert Admin User if it doesn't exist
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@laroze.com')
BEGIN
    INSERT INTO Users (FirstName, LastName, Email, Password, Phone, DateOfBirth, Gender, RegistrationDate, LastLogin, Status, RoleId)
    VALUES ('Admin', 'User', 'admin@laroze.com', '$2a$11$ysX.3Gq0XR.8G9t9UKuK3.4VBKFqHxLxEJ6Vx7QgpfRqT5g8SBGxe', '1234567890', '1990-01-01', 'Male', GETDATE(), GETDATE(), 'Active', 1);
END

-- Insert Customer Users if they don't exist
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'john@example.com')
BEGIN
    INSERT INTO Users (FirstName, LastName, Email, Password, Phone, DateOfBirth, Gender, RegistrationDate, LastLogin, Status, RoleId)
    VALUES ('John', 'Doe', 'john@example.com', '$2a$11$ysX.3Gq0XR.8G9t9UKuK3.4VBKFqHxLxEJ6Vx7QgpfRqT5g8SBGxe', '1234567891', '1985-05-15', 'Male', DATEADD(DAY, -30, GETDATE()), DATEADD(DAY, -2, GETDATE()), 'Active', 2);
END

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'jane@example.com')
BEGIN
    INSERT INTO Users (FirstName, LastName, Email, Password, Phone, DateOfBirth, Gender, RegistrationDate, LastLogin, Status, RoleId)
    VALUES ('Jane', 'Smith', 'jane@example.com', '$2a$11$ysX.3Gq0XR.8G9t9UKuK3.4VBKFqHxLxEJ6Vx7QgpfRqT5g8SBGxe', '1234567892', '1990-08-21', 'Female', DATEADD(DAY, -20, GETDATE()), DATEADD(DAY, -1, GETDATE()), 'Active', 2);
END

-- Insert User Addresses if they don't exist
DECLARE @JohnUserId INT = (SELECT UserId FROM Users WHERE Email = 'john@example.com');
DECLARE @JaneUserId INT = (SELECT UserId FROM Users WHERE Email = 'jane@example.com');

IF @JohnUserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM UserAddresses WHERE UserId = @JohnUserId)
BEGIN
    INSERT INTO UserAddresses (UserId, FullName, StreetAddress, City, State, PostalCode, Country, PhoneNumber, IsDefault)
    VALUES (@JohnUserId, 'John Doe', '123 Main Street', 'New York', 'NY', '10001', 'USA', '1234567891', 1);
END

IF @JaneUserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM UserAddresses WHERE UserId = @JaneUserId)
BEGIN
    INSERT INTO UserAddresses (UserId, FullName, StreetAddress, ApartmentNumber, City, State, PostalCode, Country, PhoneNumber, IsDefault)
    VALUES (@JaneUserId, 'Jane Smith', '456 Oak Avenue', 'Apt 7B', 'Los Angeles', 'CA', '90001', 'USA', '1234567892', 1);
END

-- Insert Categories if they don't exist
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Men')
BEGIN
    INSERT INTO Categories (Name, Description)
    VALUES ('Men', 'Clothing and accessories for men');
END

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Women')
BEGIN
    INSERT INTO Categories (Name, Description)
    VALUES ('Women', 'Clothing and accessories for women');
END

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Kids')
BEGIN
    INSERT INTO Categories (Name, Description)
    VALUES ('Kids', 'Clothing and accessories for children');
END

-- Insert SubCategories if they don't exist
DECLARE @MenCategoryId INT = (SELECT CategoryId FROM Categories WHERE Name = 'Men');
DECLARE @WomenCategoryId INT = (SELECT CategoryId FROM Categories WHERE Name = 'Women');
DECLARE @KidsCategoryId INT = (SELECT CategoryId FROM Categories WHERE Name = 'Kids');

IF NOT EXISTS (SELECT 1 FROM SubCategories WHERE SubCategoryName = 'T-Shirts' AND CategoryId = @MenCategoryId)
BEGIN
    INSERT INTO SubCategories (SubCategoryName, CategoryId, IsActive)
    VALUES ('T-Shirts', @MenCategoryId, 1);
END

IF NOT EXISTS (SELECT 1 FROM SubCategories WHERE SubCategoryName = 'Shirts' AND CategoryId = @MenCategoryId)
BEGIN
    INSERT INTO SubCategories (SubCategoryName, CategoryId, IsActive)
    VALUES ('Shirts', @MenCategoryId, 1);
END

IF NOT EXISTS (SELECT 1 FROM SubCategories WHERE SubCategoryName = 'Blouses' AND CategoryId = @WomenCategoryId)
BEGIN
    INSERT INTO SubCategories (SubCategoryName, CategoryId, IsActive)
    VALUES ('Blouses', @WomenCategoryId, 1);
END

IF NOT EXISTS (SELECT 1 FROM SubCategories WHERE SubCategoryName = 'Dresses' AND CategoryId = @WomenCategoryId)
BEGIN
    INSERT INTO SubCategories (SubCategoryName, CategoryId, IsActive)
    VALUES ('Dresses', @WomenCategoryId, 1);
END

-- Insert Products if they don't exist
IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = 'Classic White T-Shirt')
BEGIN
    INSERT INTO Products (Name, Description, Price, StockQuantity, Sku, CategoryId, ImageUrl, CreatedAt, UpdatedAt, Status)
    VALUES ('Classic White T-Shirt', 'A comfortable white t-shirt made from 100% cotton.', 19.99, 100, 'MTS001', @MenCategoryId, '/images/products/men-tshirt-white.jpg', DATEADD(DAY, -60, GETDATE()), DATEADD(DAY, -60, GETDATE()), 'Active');
END

IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = 'Floral Summer Dress')
BEGIN
    INSERT INTO Products (Name, Description, Price, StockQuantity, Sku, CategoryId, ImageUrl, CreatedAt, UpdatedAt, Status)
    VALUES ('Floral Summer Dress', 'A beautiful floral dress perfect for summer.', 59.99, 50, 'WDR001', @WomenCategoryId, '/images/products/women-dress-floral.jpg', DATEADD(DAY, -45, GETDATE()), DATEADD(DAY, -45, GETDATE()), 'Active');
END

IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = 'Cartoon Print T-Shirt')
BEGIN
    INSERT INTO Products (Name, Description, Price, StockQuantity, Sku, CategoryId, ImageUrl, CreatedAt, UpdatedAt, Status)
    VALUES ('Cartoon Print T-Shirt', 'Fun cartoon print t-shirt for kids.', 14.99, 80, 'KTS001', @KidsCategoryId, '/images/products/kids-tshirt-cartoon.jpg', DATEADD(DAY, -30, GETDATE()), DATEADD(DAY, -30, GETDATE()), 'Active');
END

-- Insert Product Images if they don't exist
DECLARE @MenTShirtId INT = (SELECT ProductId FROM Products WHERE Name = 'Classic White T-Shirt');
DECLARE @WomenDressId INT = (SELECT ProductId FROM Products WHERE Name = 'Floral Summer Dress');
DECLARE @KidsTShirtId INT = (SELECT ProductId FROM Products WHERE Name = 'Cartoon Print T-Shirt');

IF @MenTShirtId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM ProductImages WHERE ProductId = @MenTShirtId)
BEGIN
    INSERT INTO ProductImages (ProductId, ImageUrl, DefaultImage)
    VALUES (@MenTShirtId, '/images/products/men-tshirt-white.jpg', 1);
END

IF @WomenDressId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM ProductImages WHERE ProductId = @WomenDressId)
BEGIN
    INSERT INTO ProductImages (ProductId, ImageUrl, DefaultImage)
    VALUES (@WomenDressId, '/images/products/women-dress-floral.jpg', 1);
END

IF @KidsTShirtId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM ProductImages WHERE ProductId = @KidsTShirtId)
BEGIN
    INSERT INTO ProductImages (ProductId, ImageUrl, DefaultImage)
    VALUES (@KidsTShirtId, '/images/products/kids-tshirt-cartoon.jpg', 1);
END

-- Insert Orders and Payments if they don't exist
IF @JohnUserId IS NOT NULL AND EXISTS (SELECT 1 FROM UserAddresses WHERE UserId = @JohnUserId) AND NOT EXISTS (SELECT 1 FROM Orders WHERE UserId = @JohnUserId)
BEGIN
    -- Get John's address
    DECLARE @JohnAddressId INT = (SELECT TOP 1 Id FROM UserAddresses WHERE UserId = @JohnUserId);

    -- Insert Payment
    INSERT INTO Payments (PaymentMethod, Status, TransactionId, PaymentDate, Amount, PaymentDetails)
    VALUES ('Credit Card', 'Completed', 'TRX123456', DATEADD(DAY, -5, GETDATE()), 19.99, 'Payment processed successfully');

    DECLARE @PaymentId INT = SCOPE_IDENTITY();

    -- Insert Order
    INSERT INTO Orders (UserId, ShippingAddressId, TotalAmount, OrderDate, Status, TrackingNumber, PaymentId)
    VALUES (@JohnUserId, @JohnAddressId, 19.99, DATEADD(DAY, -5, GETDATE()), 2, 'TRK' + CONVERT(VARCHAR(8), GETDATE(), 112) + '1234', @PaymentId);

    DECLARE @OrderId INT = SCOPE_IDENTITY();

    -- Insert Order Item
    INSERT INTO OrderItems (OrderId, ProductId, Quantity, Price, ProductName, ProductSku, ProductImage)
    VALUES (@OrderId, @MenTShirtId, 1, 19.99, 'Classic White T-Shirt', 'MTS001', '/images/products/men-tshirt-white.jpg');
END

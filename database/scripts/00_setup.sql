USE MASTER;
GO

-- Safely drop the database if it exists
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'LeisureAustralasiaDB')
BEGIN
    ALTER DATABASE LeisureAustralasiaDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE LeisureAustralasiaDB;
END
GO

-- Create database with appropriate settings for EF Core
CREATE DATABASE LeisureAustralasiaDB
COLLATE SQL_Latin1_General_CP1_CI_AS;
GO

USE LeisureAustralasiaDB;
GO

-- ============================================================================
-- Reference Tables (Lookup Tables)
-- ============================================================================

CREATE TABLE Status (
    StatusId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255) NULL
);

CREATE TABLE Currency (
    CurrencyId INT IDENTITY(1,1) PRIMARY KEY,
    CurrencyCode NVARCHAR(3) NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    Symbol NVARCHAR(5) NULL,
    IsBaseCurrency BIT NOT NULL DEFAULT 0
);

CREATE TABLE Country (
    CountryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CountryCode NVARCHAR(3) NOT NULL,
    DefaultCurrencyId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Country_Currency FOREIGN KEY (DefaultCurrencyId) REFERENCES Currency(CurrencyId)
);

CREATE TABLE City (
    CityId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CountryId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_City_Country FOREIGN KEY (CountryId) REFERENCES Country(CountryId)
);

CREATE TABLE Role (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255) NULL,
    CanAuthorizeDiscounts BIT NOT NULL DEFAULT 0,
    CanAuthorizePackages BIT NOT NULL DEFAULT 0,
    MaxDiscountPercentage DECIMAL(5,2) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastModifiedDate DATETIME2 NULL
);

CREATE TABLE Department (
    DepartmentId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE FacilityType (
    FacilityTypeId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NULL,
    DefaultCapacity INT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE ServiceType (
    ServiceTypeId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE ServiceCategory (
    ServiceCategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(20) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NULL,
    ServiceTypeId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_ServiceCategory_ServiceType FOREIGN KEY (ServiceTypeId) REFERENCES ServiceType(ServiceTypeId),
    CONSTRAINT UQ_ServiceCategory_Code UNIQUE (Code)
);

CREATE TABLE PaymentMethod (
    PaymentMethodId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- ============================================================================
-- Main Entity Tables
-- ============================================================================

CREATE TABLE Hotel (
    HotelId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    CityId INT NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    Description NVARCHAR(500) NULL,
    TotalCapacity INT NOT NULL,
    BaseCurrencyId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastModifiedDate DATETIME2 NULL,
    CONSTRAINT FK_Hotel_City FOREIGN KEY (CityId) REFERENCES City(CityId),
    CONSTRAINT FK_Hotel_Currency FOREIGN KEY (BaseCurrencyId) REFERENCES Currency(CurrencyId)
);

CREATE TABLE Facility (
    FacilityId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    FacilityTypeId INT NOT NULL,
    HotelId INT NOT NULL,
    Description NVARCHAR(500) NULL,
    Capacity INT NULL,
    StatusId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastModifiedDate DATETIME2 NULL,
    CONSTRAINT FK_Facility_FacilityType FOREIGN KEY (FacilityTypeId) REFERENCES FacilityType(FacilityTypeId),
    CONSTRAINT FK_Facility_Hotel FOREIGN KEY (HotelId) REFERENCES Hotel(HotelId),
    CONSTRAINT FK_Facility_Status FOREIGN KEY (StatusId) REFERENCES Status(StatusId)
);

CREATE TABLE Employee (
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NULL,
    PhoneNumber NVARCHAR(20) NULL,
    RoleId INT NOT NULL,
    DepartmentId INT NOT NULL,
    HotelId INT NOT NULL,
    CanAuthorizeDiscounts BIT NOT NULL DEFAULT 0,
    MaxDiscountPercentage DECIMAL(5,2) NULL,
    HireDate DATE NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastModifiedDate DATETIME2 NULL,
    CONSTRAINT FK_Employee_Role FOREIGN KEY (RoleId) REFERENCES Role(RoleId),
    CONSTRAINT FK_Employee_Department FOREIGN KEY (DepartmentId) REFERENCES Department(DepartmentId),
    CONSTRAINT FK_Employee_Hotel FOREIGN KEY (HotelId) REFERENCES Hotel(HotelId)
);

CREATE TABLE ServiceItem (
    ServiceItemId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    ServiceCategoryId INT NOT NULL,
    FacilityTypeId INT NULL,
    BaseCost DECIMAL(10,2) NOT NULL,
    BaseCurrencyId INT NOT NULL,
    Capacity INT NULL,
    AvailableTimes NVARCHAR(255) NULL,
    Restrictions NVARCHAR(500) NULL,
    Notes NVARCHAR(500) NULL,
    Comments NVARCHAR(500) NULL,
    StatusId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastModifiedDate DATETIME2 NULL,
    CONSTRAINT FK_ServiceItem_ServiceCategory FOREIGN KEY (ServiceCategoryId) REFERENCES ServiceCategory(ServiceCategoryId),
    CONSTRAINT FK_ServiceItem_FacilityType FOREIGN KEY (FacilityTypeId) REFERENCES FacilityType(FacilityTypeId),
    CONSTRAINT FK_ServiceItem_Currency FOREIGN KEY (BaseCurrencyId) REFERENCES Currency(CurrencyId),
    CONSTRAINT FK_ServiceItem_Status FOREIGN KEY (StatusId) REFERENCES Status(StatusId)
);

CREATE TABLE HotelServiceItem (
    HotelServiceItemId INT IDENTITY(1,1) PRIMARY KEY,
    HotelId INT NOT NULL,
    ServiceItemId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_HotelServiceItem_Hotel FOREIGN KEY (HotelId) REFERENCES Hotel(HotelId),
    CONSTRAINT FK_HotelServiceItem_ServiceItem FOREIGN KEY (ServiceItemId) REFERENCES ServiceItem(ServiceItemId),
    CONSTRAINT UQ_HotelServiceItem UNIQUE (HotelId, ServiceItemId)
);

CREATE TABLE Customer (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NULL,
    PhoneNumber NVARCHAR(20) NULL,
    Address NVARCHAR(255) NULL,
    CityId INT NULL,
    LoyaltyPoints INT DEFAULT 0,
    JoinDate DATE DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastModifiedDate DATETIME2 NULL,
    CONSTRAINT FK_Customer_City FOREIGN KEY (CityId) REFERENCES City(CityId)
);

CREATE TABLE Guest (
    GuestId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NULL,
    PhoneNumber NVARCHAR(20) NULL,
    Address NVARCHAR(255) NULL,
    CityId INT NULL,
    CustomerId INT NULL, -- Link to customer if they are registered
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastModifiedDate DATETIME2 NULL,
    CONSTRAINT FK_Guest_City FOREIGN KEY (CityId) REFERENCES City(CityId),
    CONSTRAINT FK_Guest_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId)
);

-- ============================================================================
-- Package Management
-- ============================================================================

CREATE TABLE AdvertisedPackage (
    PackageId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    AdvertisedPrice DECIMAL(10,2) NOT NULL,
    AdvertisedCurrencyId INT NOT NULL,
    StatusId INT NOT NULL,
    Inclusions NVARCHAR(500) NULL,
    Exclusions NVARCHAR(500) NULL,
    GracePeriodDays INT NOT NULL DEFAULT 1,
    AuthorizedByEmployeeId INT NULL,
    IsStandardPackage BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastModifiedDate DATETIME2 NULL,

    
    CONSTRAINT FK_AdvertisedPackage_Currency FOREIGN KEY (AdvertisedCurrencyId) REFERENCES Currency(CurrencyId),
    CONSTRAINT FK_AdvertisedPackage_Status FOREIGN KEY (StatusId) REFERENCES Status(StatusId),
    CONSTRAINT FK_AdvertisedPackage_Employee FOREIGN KEY (AuthorizedByEmployeeId) REFERENCES Employee(EmployeeId),
    CONSTRAINT CK_Package_Dates CHECK (EndDate >= StartDate)
);

CREATE TABLE PackageServiceItem (
    PackageServiceItemId INT IDENTITY(1,1) PRIMARY KEY,
    PackageId INT NOT NULL,
    ServiceItemId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    IsOptional BIT NOT NULL DEFAULT 0,
    AdditionalCost DECIMAL(10,2) NULL,
    CONSTRAINT FK_PackageServiceItem_Package FOREIGN KEY (PackageId) REFERENCES AdvertisedPackage(PackageId),
    CONSTRAINT FK_PackageServiceItem_ServiceItem FOREIGN KEY (ServiceItemId) REFERENCES ServiceItem(ServiceItemId),
    CONSTRAINT UQ_PackageServiceItem UNIQUE (PackageId, ServiceItemId)
);

CREATE TABLE HotelPackage (
    HotelPackageId INT IDENTITY(1,1) PRIMARY KEY,
    HotelId INT NOT NULL,
    PackageId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_HotelPackage_Hotel FOREIGN KEY (HotelId) REFERENCES Hotel(HotelId),
    CONSTRAINT FK_HotelPackage_Package FOREIGN KEY (PackageId) REFERENCES AdvertisedPackage(PackageId),
    CONSTRAINT UQ_HotelPackage UNIQUE (HotelId, PackageId)
);

-- ============================================================================
-- Booking and Reservation Tables
-- ============================================================================

CREATE TABLE PaymentInformation (
    PaymentInformationId INT IDENTITY(1,1) PRIMARY KEY,
    PaymentDate DATETIME2 NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    PaymentMethodId INT NOT NULL,
    TransactionReference NVARCHAR(100) NULL,
    Status NVARCHAR(50) NOT NULL,
    CurrencyId INT NOT NULL,
    CONSTRAINT FK_PaymentInformation_PaymentMethod FOREIGN KEY (PaymentMethodId) REFERENCES PaymentMethod(PaymentMethodId),
    CONSTRAINT FK_PaymentInformation_Currency FOREIGN KEY (CurrencyId) REFERENCES Currency(CurrencyId)
);

CREATE TABLE Reservation (
    ReservationId INT IDENTITY(1,1) PRIMARY KEY,
    ReservationNumber NVARCHAR(20) NOT NULL,
    CustomerId INT NOT NULL,
    HotelId INT NOT NULL,
    ReservationDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    ReservationType NVARCHAR(50) NOT NULL, -- Online, Phone, In-Person
    TotalAmount DECIMAL(10,2) NOT NULL,
    DepositAmount DECIMAL(10,2) NULL,
    DepositPaymentInformationId INT NULL,
    IsFullyPaid BIT NOT NULL DEFAULT 0,
    StatusId INT NOT NULL,
    CancellationDate DATETIME2 NULL,
    IsCancelledAfterGracePeriod BIT NULL,
    CancellationFee DECIMAL(10,2) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastModifiedDate DATETIME2 NULL,
    CONSTRAINT FK_Reservation_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId),
    CONSTRAINT FK_Reservation_Hotel FOREIGN KEY (HotelId) REFERENCES Hotel(HotelId),
    CONSTRAINT FK_Reservation_PaymentInformation FOREIGN KEY (DepositPaymentInformationId) REFERENCES PaymentInformation(PaymentInformationId),
    CONSTRAINT FK_Reservation_Status FOREIGN KEY (StatusId) REFERENCES Status(StatusId),
    CONSTRAINT UQ_Reservation_Number UNIQUE (ReservationNumber)
);

CREATE TABLE Booking (
    BookingId INT IDENTITY(1,1) PRIMARY KEY,
    ReservationId INT NOT NULL,
    PackageId INT NULL, -- NULL if not a package booking
    ServiceItemId INT NULL, -- NULL if a package booking
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    StatusId INT NOT NULL,
    CheckInDateTime DATETIME2 NULL,
    CheckOutDateTime DATETIME2 NULL,
    CancellationDate DATETIME2 NULL,
    IsCancelledAfterGracePeriod BIT NULL,
    CancellationFee DECIMAL(10,2) NULL,
    Notes NVARCHAR(500) NULL,
    CONSTRAINT FK_Booking_Reservation FOREIGN KEY (ReservationId) REFERENCES Reservation(ReservationId),
    CONSTRAINT FK_Booking_Package FOREIGN KEY (PackageId) REFERENCES AdvertisedPackage(PackageId),
    CONSTRAINT FK_Booking_ServiceItem FOREIGN KEY (ServiceItemId) REFERENCES ServiceItem(ServiceItemId),
    CONSTRAINT FK_Booking_Status FOREIGN KEY (StatusId) REFERENCES Status(StatusId),
    CONSTRAINT CK_Booking_Dates CHECK (EndDate >= StartDate),
    CONSTRAINT CK_Booking_PackageOrService CHECK (
        (PackageId IS NULL AND ServiceItemId IS NOT NULL) OR
        (PackageId IS NOT NULL AND ServiceItemId IS NULL)
    )
);

CREATE TABLE BookingFacility (
    BookingFacilityId INT IDENTITY(1,1) PRIMARY KEY,
    BookingId INT NOT NULL,
    FacilityId INT NOT NULL,
    StartDateTime DATETIME2 NOT NULL,
    EndDateTime DATETIME2 NOT NULL,
    CONSTRAINT FK_BookingFacility_Booking FOREIGN KEY (BookingId) REFERENCES Booking(BookingId),
    CONSTRAINT FK_BookingFacility_Facility FOREIGN KEY (FacilityId) REFERENCES Facility(FacilityId),
    CONSTRAINT CK_BookingFacility_Dates CHECK (EndDateTime >= StartDateTime)
);

CREATE TABLE BookingGuest (
    BookingGuestId INT IDENTITY(1,1) PRIMARY KEY,
    BookingId INT NOT NULL,
    GuestId INT NOT NULL,
    IsPrimaryGuest BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_BookingGuest_Booking FOREIGN KEY (BookingId) REFERENCES Booking(BookingId),
    CONSTRAINT FK_BookingGuest_Guest FOREIGN KEY (GuestId) REFERENCES Guest(GuestId),
    CONSTRAINT UQ_BookingGuest UNIQUE (BookingId, GuestId)
);

-- ============================================================================
-- Billing Tables
-- ============================================================================

CREATE TABLE ChargeType (
    ChargeTypeId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE Charge (
    ChargeId INT IDENTITY(1,1) PRIMARY KEY,
    BookingId INT NOT NULL,
    ServiceItemId INT NULL, -- NULL if it's not a service-related charge
    ChargeTypeId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    CurrencyId INT NOT NULL,
    Description NVARCHAR(255) NULL,
    ChargeDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    EmployeeId INT NULL, -- Employee who added the charge
    IsPackageInclusion BIT NOT NULL DEFAULT 0, -- Whether this charge is included in a package
    CONSTRAINT FK_Charge_Booking FOREIGN KEY (BookingId) REFERENCES Booking(BookingId),
    CONSTRAINT FK_Charge_ServiceItem FOREIGN KEY (ServiceItemId) REFERENCES ServiceItem(ServiceItemId),
    CONSTRAINT FK_Charge_ChargeType FOREIGN KEY (ChargeTypeId) REFERENCES ChargeType(ChargeTypeId),
    CONSTRAINT FK_Charge_Currency FOREIGN KEY (CurrencyId) REFERENCES Currency(CurrencyId),
    CONSTRAINT FK_Charge_Employee FOREIGN KEY (EmployeeId) REFERENCES Employee(EmployeeId)
);

CREATE TABLE Bill (
    BillId INT IDENTITY(1,1) PRIMARY KEY,
    ReservationId INT NOT NULL,
    IssuedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    DueDate DATETIME2 NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    DepositAmount DECIMAL(10,2) NULL DEFAULT 0,
    DiscountAmount DECIMAL(10,2) NULL DEFAULT 0,
    DiscountPercentage DECIMAL(5,2) NULL,
    IsDiscountAuthorized BIT NOT NULL DEFAULT 0,
    DiscountAuthorizedByEmployeeId INT NULL,
    RequiresHeadOfficeAuthorization BIT NOT NULL DEFAULT 0,
    HeadOfficeAuthorizationStatus NVARCHAR(50) NULL,
    PaidAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
    StatusId INT NOT NULL,
    Notes NVARCHAR(500) NULL,
    CurrencyId INT NOT NULL,
    CONSTRAINT FK_Bill_Reservation FOREIGN KEY (ReservationId) REFERENCES Reservation(ReservationId),
    CONSTRAINT FK_Bill_Status FOREIGN KEY (StatusId) REFERENCES Status(StatusId),
    CONSTRAINT FK_Bill_Employee FOREIGN KEY (DiscountAuthorizedByEmployeeId) REFERENCES Employee(EmployeeId),
    CONSTRAINT FK_Bill_Currency FOREIGN KEY (CurrencyId) REFERENCES Currency(CurrencyId)
);

CREATE TABLE BillCharge (
    BillChargeId INT IDENTITY(1,1) PRIMARY KEY,
    BillId INT NOT NULL,
    ChargeId INT NOT NULL,
    CONSTRAINT FK_BillCharge_Bill FOREIGN KEY (BillId) REFERENCES Bill(BillId),
    CONSTRAINT FK_BillCharge_Charge FOREIGN KEY (ChargeId) REFERENCES Charge(ChargeId),
    CONSTRAINT UQ_BillCharge UNIQUE (BillId, ChargeId)
);

CREATE TABLE BillPayment (
    BillPaymentId INT IDENTITY(1,1) PRIMARY KEY,
    BillId INT NOT NULL,
    PaymentInformationId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    PaymentDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_BillPayment_Bill FOREIGN KEY (BillId) REFERENCES Bill(BillId),
    CONSTRAINT FK_BillPayment_PaymentInformation FOREIGN KEY (PaymentInformationId) REFERENCES PaymentInformation(PaymentInformationId)
);

-- ============================================================================
-- Seed Data for Essential Tables
-- ============================================================================

-- Status data
INSERT INTO Status (Name, Description)
VALUES
('Active', 'Item is currently active and available'),
('Inactive', 'Item is currently inactive and unavailable'),
('Reserved', 'Item is currently reserved'),
('Occupied', 'Item is currently occupied'),
('Maintenance', 'Item is under maintenance'),
('Pending', 'Item is pending approval or activation'),
('Confirmed', 'Item has been confirmed'),
('Cancelled', 'Item has been cancelled'),
('Completed', 'Item has been completed successfully'),
('CheckedIn', 'Guest has checked in'),
('CheckedOut', 'Guest has checked out'),
('Paid', 'Payment has been received in full'),
('PartiallyPaid', 'Partial payment has been received');

-- Service Types
INSERT INTO ServiceType (Name, Description)
VALUES
('Accommodation', 'Room and lodging services'),
('Food', 'Food and beverage services'),
('Venue', 'Event venue services'),
('Wellness', 'Spa, gym and health services'),
('Transportation', 'Transport and travel services'),
('Laundry', 'Laundry and cleaning services'),
('Entertainment', 'Entertainment services'),
('Tours', 'Sight-seeing and guided tours');

-- Facility Types
INSERT INTO FacilityType (Name, Description, DefaultCapacity)
VALUES
('Standard Room', 'Basic hotel room for 1-2 guests', 2),
('Family Room', 'Larger room for families', 4),
('Suite', 'Luxury suite with separate living area', 2),
('Conference Hall', 'Venue for meetings and conferences', 100),
('Restaurant', 'Food service area', 50),
('Swimming Pool', 'Pool facility', 30),
('Gym', 'Fitness center', 20),
('Spa', 'Wellness and massage facility', 10);

-- Currency data
INSERT INTO Currency (CurrencyCode, Name, Symbol, IsBaseCurrency)
VALUES
('AUD', 'Australian Dollar', '$', 1),
('SGD', 'Singapore Dollar', 'S$', 0),
('THB', 'Thai Baht', '฿', 0),
('VND', 'Vietnamese Dong', '₫', 0),
('INR', 'Indian Rupee', '₹', 0),
('LKR', 'Sri Lankan Rupee', 'Rs', 0);

GO

USE [master];
GO
USE LeisureAustralasiaDB;
GO

-- Create custom table type for service items
IF TYPE_ID('ServiceItemList') IS NOT NULL
    DROP TYPE ServiceItemList;
GO

CREATE TYPE ServiceItemList AS TABLE
(
    ServiceItemId INT NOT NULL,
    Quantity INT NOT NULL,
    IsOptional BIT NOT NULL DEFAULT 0,
    AdditionalCost DECIMAL(10, 2) NULL
);
GO

-- Create or alter the stored procedure
IF OBJECT_ID('dbo.usp_CreatePackage', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_CreatePackage;
GO

CREATE PROCEDURE dbo.usp_CreatePackage
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @StartDate DATE,
    @EndDate DATE,
    @AdvertisedPrice DECIMAL(10, 2),
    @AdvertisedCurrencyId INT,
    @HotelId INT,
    @AuthorizingEmployeeId INT,
    @Inclusions NVARCHAR(500) = NULL,
    @Exclusions NVARCHAR(500) = NULL,
    @GracePeriodDays INT = 1,
    @IsStandardPackage BIT = 0,
    @ServiceItems ServiceItemList READONLY,
    @PackageId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;
    DECLARE @StatusId INT;
    
    BEGIN TRY
        -- Validate required parameters
        IF @Name IS NULL OR LEN(TRIM(@Name)) = 0
            THROW 50001, 'Package name is required', 1;
        
        IF @StartDate IS NULL OR @EndDate IS NULL
            THROW 50002, 'Start and end dates are required', 1;
            
        IF @EndDate < @StartDate
            THROW 50003, 'End date must be after start date', 1;
            
        IF @AdvertisedPrice <= 0
            THROW 50004, 'Advertised price must be greater than zero', 1;
            
        IF NOT EXISTS (SELECT 1 FROM Currency WHERE CurrencyId = @AdvertisedCurrencyId)
            THROW 50005, 'Invalid currency specified', 1;
            
        IF NOT EXISTS (SELECT 1 FROM Hotel WHERE HotelId = @HotelId)
            THROW 50006, 'Invalid hotel specified', 1;
            
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE EmployeeId = @AuthorizingEmployeeId)
            THROW 50007, 'Invalid authorizing employee specified', 1;
            
        IF NOT EXISTS (SELECT 1 FROM Employee e 
                       INNER JOIN Role r ON e.RoleId = r.RoleId 
                       WHERE e.EmployeeId = @AuthorizingEmployeeId AND r.CanAuthorizePackages = 1)
            THROW 50008, 'Employee does not have authorization to create packages', 1;
            
        -- Check if service items exist and are valid for the specified hotel
        IF EXISTS (
            SELECT 1 FROM @ServiceItems si
            WHERE NOT EXISTS (
                SELECT 1 FROM ServiceItem s 
                INNER JOIN HotelServiceItem hsi ON s.ServiceItemId = hsi.ServiceItemId
                WHERE s.ServiceItemId = si.ServiceItemId AND hsi.HotelId = @HotelId
            )
        )
            THROW 50009, 'One or more service items are not available at the specified hotel', 1;
        
        -- Set initial status to Active
        SELECT @StatusId = StatusId FROM Status WHERE Name = 'Active';
        
        BEGIN TRANSACTION;
        
        -- Insert the package
        INSERT INTO AdvertisedPackage (
            Name, 
            Description, 
            StartDate, 
            EndDate, 
            AdvertisedPrice, 
            AdvertisedCurrencyId, 
            StatusId, 
            Inclusions, 
            Exclusions, 
            GracePeriodDays, 
            AuthorizedByEmployeeId, 
            IsStandardPackage,
            IsActive,
            CreatedDate
        )
        VALUES (
            @Name, 
            @Description, 
            @StartDate, 
            @EndDate, 
            @AdvertisedPrice, 
            @AdvertisedCurrencyId, 
            @StatusId, 
            @Inclusions, 
            @Exclusions, 
            @GracePeriodDays, 
            @AuthorizingEmployeeId,
            @IsStandardPackage,
            1,
            GETDATE()
        );
        
        SET @PackageId = SCOPE_IDENTITY();
        
        -- Associate the package with the hotel
        INSERT INTO HotelPackage (HotelId, PackageId, IsActive)
        VALUES (@HotelId, @PackageId, 1);
        
        -- Add the service items to the package
        INSERT INTO PackageServiceItem (
            PackageId, 
            ServiceItemId, 
            Quantity, 
            IsOptional, 
            AdditionalCost
        )
        SELECT 
            @PackageId, 
            ServiceItemId, 
            Quantity, 
            IsOptional, 
            AdditionalCost
        FROM @ServiceItems;
        
        COMMIT TRANSACTION;
        
        RETURN 0; -- Success
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();
            
        -- Log the error
        INSERT INTO ErrorLog (
            ErrorProcedure,
            ErrorLine,
            ErrorMessage,
            ErrorNumber,
            ErrorSeverity,
            ErrorState,
            ErrorDateTime
        )
        VALUES (
            ERROR_PROCEDURE(),
            ERROR_LINE(),
            @ErrorMessage,
            ERROR_NUMBER(),
            @ErrorSeverity,
            @ErrorState,
            GETDATE()
        );
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
        
        RETURN -1; -- Failure
    END CATCH;
END;
GO
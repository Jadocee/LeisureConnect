USE LeisureAustralasiaDB;
GO

-- Create custom table types for reservation items and guests
IF TYPE_ID('ReservationItemList') IS NOT NULL
    DROP TYPE ReservationItemList;
GO

CREATE TYPE ReservationItemList AS TABLE
(
    PackageId INT NULL,
    ServiceItemId INT NULL,
    Quantity INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    SpecialRequests NVARCHAR(500) NULL
);
GO

IF TYPE_ID('GuestList') IS NOT NULL
    DROP TYPE GuestList;
GO

CREATE TYPE GuestList AS TABLE
(
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NULL,
    PhoneNumber NVARCHAR(20) NULL,
    Address NVARCHAR(255) NULL,
    IsPrimaryGuest BIT NOT NULL DEFAULT 0
);
GO

-- Create or alter the stored procedure
IF OBJECT_ID('dbo.usp_MakeReservation', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_MakeReservation;
GO

CREATE PROCEDURE dbo.usp_MakeReservation
    @CustomerFirstName NVARCHAR(50),
    @CustomerLastName NVARCHAR(50),
    @CustomerEmail NVARCHAR(100) = NULL,
    @CustomerPhoneNumber NVARCHAR(20) = NULL,
    @CustomerAddress NVARCHAR(255) = NULL,
    @CustomerCityId INT = NULL,
    @HotelId INT,
    @ReservationType NVARCHAR(50),
    @PaymentMethodId INT = NULL,
    @PaymentReference NVARCHAR(100) = NULL,
    @CurrencyId INT,
    @ReservationItems ReservationItemList READONLY,
    @Guests GuestList READONLY,
    @ReservationId INT OUTPUT,
    @ReservationNumber NVARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;
    DECLARE @StatusId INT;
    DECLARE @CustomerId INT;
    DECLARE @TotalAmount DECIMAL(10, 2) = 0;
    DECLARE @DepositAmount DECIMAL(10, 2) = 0;
    DECLARE @PaymentInformationId INT;
    DECLARE @GuestCount INT;
    DECLARE @BookingId INT;
    
    BEGIN TRY
        -- Validate required parameters
        IF @CustomerFirstName IS NULL OR LEN(TRIM(@CustomerFirstName)) = 0 OR
           @CustomerLastName IS NULL OR LEN(TRIM(@CustomerLastName)) = 0
            THROW 50001, 'Customer name is required', 1;
            
        IF @HotelId IS NULL OR NOT EXISTS (SELECT 1 FROM Hotel WHERE HotelId = @HotelId AND IsActive = 1)
            THROW 50002, 'Invalid hotel specified', 1;
            
        IF @ReservationType IS NULL OR LEN(TRIM(@ReservationType)) = 0
            THROW 50003, 'Reservation type is required', 1;
            
        IF @CurrencyId IS NULL OR NOT EXISTS (SELECT 1 FROM Currency WHERE CurrencyId = @CurrencyId)
            THROW 50004, 'Invalid currency specified', 1;
            
        -- Check if we have at least one reservation item
        IF NOT EXISTS (SELECT 1 FROM @ReservationItems)
            THROW 50005, 'At least one reservation item is required', 1;
            
        -- Check if we have at least one guest
        IF NOT EXISTS (SELECT 1 FROM @Guests)
            THROW 50006, 'At least one guest is required', 1;
            
        -- Validate reservation items: Each item must have either PackageId or ServiceItemId (not both)
        IF EXISTS (
            SELECT 1 FROM @ReservationItems 
            WHERE (PackageId IS NULL AND ServiceItemId IS NULL) OR (PackageId IS NOT NULL AND ServiceItemId IS NOT NULL)
        )
            THROW 50007, 'Each reservation item must specify either a package or a service (not both)', 1;
            
        -- Verify dates are valid
        IF EXISTS (
            SELECT 1 FROM @ReservationItems
            WHERE EndDate < StartDate
        )
            THROW 50008, 'End date must be after start date for all reservation items', 1;
            
        -- Check if packages are valid and available at the hotel
        IF EXISTS (
            SELECT 1 FROM @ReservationItems ri
            WHERE ri.PackageId IS NOT NULL AND NOT EXISTS (
                SELECT 1 FROM AdvertisedPackage ap 
                INNER JOIN HotelPackage hp ON ap.PackageId = hp.PackageId
                WHERE ap.PackageId = ri.PackageId 
                  AND hp.HotelId = @HotelId
                  AND ap.IsActive = 1
                  AND hp.IsActive = 1
                  AND ap.StartDate <= ri.StartDate
                  AND ap.EndDate >= ri.EndDate
            )
        )
            THROW 50009, 'One or more packages are not available at the specified hotel during the requested dates', 1;
            
        -- Check if services are valid and available at the hotel
        IF EXISTS (
            SELECT 1 FROM @ReservationItems ri
            WHERE ri.ServiceItemId IS NOT NULL AND NOT EXISTS (
                SELECT 1 FROM ServiceItem si 
                INNER JOIN HotelServiceItem hsi ON si.ServiceItemId = hsi.ServiceItemId
                WHERE si.ServiceItemId = ri.ServiceItemId 
                  AND hsi.HotelId = @HotelId
                  AND si.IsActive = 1
                  AND hsi.IsActive = 1
            )
        )
            THROW 50010, 'One or more services are not available at the specified hotel', 1;
            
        SELECT @GuestCount = COUNT(*) FROM @Guests;
        
        -- Check capacity constraints for each reservation item
        DECLARE @ItemId INT, @ItemType NVARCHAR(10), @Capacity INT, @ItemQuantity INT, @FacilityTypeId INT;
        
        DECLARE capacityCursor CURSOR FOR 
        SELECT 
            CASE WHEN ri.PackageId IS NOT NULL THEN ri.PackageId ELSE ri.ServiceItemId END AS ItemId,
            CASE WHEN ri.PackageId IS NOT NULL THEN 'Package' ELSE 'Service' END AS ItemType,
            ri.Quantity
        FROM @ReservationItems ri;
        
        OPEN capacityCursor;
        FETCH NEXT FROM capacityCursor INTO @ItemId, @ItemType, @ItemQuantity;
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            IF @ItemType = 'Service'
            BEGIN
                SELECT @FacilityTypeId = FacilityTypeId FROM ServiceItem WHERE ServiceItemId = @ItemId;
                
                IF @FacilityTypeId IS NOT NULL
                BEGIN
                    SELECT @Capacity = DefaultCapacity FROM FacilityType WHERE FacilityTypeId = @FacilityTypeId;
                    
                    IF @Capacity IS NOT NULL AND (@GuestCount * @ItemQuantity) > @Capacity
                        THROW 50011, 'Number of guests exceeds facility capacity for one or more services', 1;
                END
            END
            ELSE -- Package
            BEGIN
                -- For packages, check capacity constraints of each service in the package
                DECLARE @ServiceId INT, @ServiceCapacity INT, @ServiceFacilityTypeId INT;
                DECLARE @MaxCapacityExceeded BIT = 0;
                
                DECLARE packageServiceCursor CURSOR FOR
                SELECT si.ServiceItemId, si.FacilityTypeId
                FROM PackageServiceItem psi
                INNER JOIN ServiceItem si ON psi.ServiceItemId = si.ServiceItemId
                WHERE psi.PackageId = @ItemId;
                
                OPEN packageServiceCursor;
                FETCH NEXT FROM packageServiceCursor INTO @ServiceId, @ServiceFacilityTypeId;
                
                WHILE @@FETCH_STATUS = 0 AND @MaxCapacityExceeded = 0
                BEGIN
                    IF @ServiceFacilityTypeId IS NOT NULL
                    BEGIN
                        SELECT @ServiceCapacity = DefaultCapacity FROM FacilityType WHERE FacilityTypeId = @ServiceFacilityTypeId;
                        
                        IF @ServiceCapacity IS NOT NULL AND (@GuestCount * @ItemQuantity) > @ServiceCapacity
                            SET @MaxCapacityExceeded = 1;
                    END
                    
                    FETCH NEXT FROM packageServiceCursor INTO @ServiceId, @ServiceFacilityTypeId;
                END
                
                CLOSE packageServiceCursor;
                DEALLOCATE packageServiceCursor;
                
                IF @MaxCapacityExceeded = 1
                    THROW 50012, 'Number of guests exceeds facility capacity for one or more services in the package', 1;
            END
            
            FETCH NEXT FROM capacityCursor INTO @ItemId, @ItemType, @ItemQuantity;
        END
        
        CLOSE capacityCursor;
        DEALLOCATE capacityCursor;
        
        -- Generate reservation number (Format: YYYYMMDD-HOTEL-XXXX)
        SET @ReservationNumber = 
            CONVERT(NVARCHAR(8), GETDATE(), 112) + '-' +
            CAST(@HotelId AS NVARCHAR(5)) + '-' +
            RIGHT('0000' + CAST(NEXT VALUE FOR ReservationNumberSequence AS NVARCHAR(4)), 4);
            
                    -- Calculate total amount
        DECLARE @ItemTotalAmount DECIMAL(10, 2);
        
        DECLARE amountCursor CURSOR FOR 
        SELECT 
            CASE 
                WHEN ri.PackageId IS NOT NULL THEN 
                    (SELECT AdvertisedPrice FROM AdvertisedPackage WHERE PackageId = ri.PackageId)
                ELSE 
                    (SELECT BaseCost FROM ServiceItem WHERE ServiceItemId = ri.ServiceItemId)
            END * ri.Quantity * DATEDIFF(DAY, ri.StartDate, DATEADD(DAY, 1, ri.EndDate)) AS TotalAmount
        FROM @ReservationItems ri;
        
        OPEN amountCursor;
        FETCH NEXT FROM amountCursor INTO @ItemTotalAmount;
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @TotalAmount = @TotalAmount + @ItemTotalAmount;
            FETCH NEXT FROM amountCursor INTO @ItemTotalAmount;
        END
        
        CLOSE amountCursor;
        DEALLOCATE amountCursor;
        
        -- Calculate deposit (10% of total amount)
        SET @DepositAmount = @TotalAmount * 0.10;
        
        -- Get 'Confirmed' status
        SELECT @StatusId = StatusId FROM Status WHERE Name = 'Confirmed';
        
        BEGIN TRANSACTION;
        
        -- Check if customer already exists
        IF @CustomerEmail IS NOT NULL
        BEGIN
            SELECT @CustomerId = CustomerId 
            FROM Customer 
            WHERE Email = @CustomerEmail AND IsActive = 1;
        END
        
        -- If customer doesn't exist, create a new one
        IF @CustomerId IS NULL
        BEGIN
            INSERT INTO Customer (
                FirstName,
                LastName,
                Email,
                PhoneNumber,
                Address,
                CityId,
                IsActive,
                CreatedDate
            )
            VALUES (
                @CustomerFirstName,
                @CustomerLastName,
                @CustomerEmail,
                @CustomerPhoneNumber,
                @CustomerAddress,
                @CustomerCityId,
                1,
                GETDATE()
            );
            
            SET @CustomerId = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            -- Update customer information if needed
            UPDATE Customer
            SET 
                FirstName = ISNULL(@CustomerFirstName, FirstName),
                LastName = ISNULL(@CustomerLastName, LastName),
                PhoneNumber = ISNULL(@CustomerPhoneNumber, PhoneNumber),
                Address = ISNULL(@CustomerAddress, Address),
                CityId = ISNULL(@CustomerCityId, CityId),
                LastModifiedDate = GETDATE()
            WHERE CustomerId = @CustomerId;
        END
        
        -- Create payment information for deposit
        IF @PaymentMethodId IS NOT NULL
        BEGIN
            INSERT INTO PaymentInformation (
                PaymentDate,
                Amount,
                PaymentMethodId,
                TransactionReference,
                Status,
                CurrencyId
            )
            VALUES (
                GETDATE(),
                @DepositAmount,
                @PaymentMethodId,
                @PaymentReference,
                'Processed',
                @CurrencyId
            );
            
            SET @PaymentInformationId = SCOPE_IDENTITY();
        END
        
        -- Create the reservation
        INSERT INTO Reservation (
            ReservationNumber,
            CustomerId,
            HotelId,
            ReservationDate,
            ReservationType,
            TotalAmount,
            DepositAmount,
            DepositPaymentInformationId,
            IsFullyPaid,
            StatusId,
            CreatedDate
        )
        VALUES (
            @ReservationNumber,
            @CustomerId,
            @HotelId,
            GETDATE(),
            @ReservationType,
            @TotalAmount,
            @DepositAmount,
            @PaymentInformationId,
            0, -- Not fully paid
            @StatusId,
            GETDATE()
        );
        
        SET @ReservationId = SCOPE_IDENTITY();
        
        -- Create guests
        DECLARE @GuestIds TABLE (RowId INT IDENTITY(1,1), GuestId INT);
        DECLARE @GuestFirstName NVARCHAR(50), @GuestLastName NVARCHAR(50), @GuestEmail NVARCHAR(100), 
                @GuestPhone NVARCHAR(20), @GuestAddress NVARCHAR(255), @IsPrimaryGuest BIT;
        
        INSERT INTO Guest (
            FirstName,
            LastName,
            Email,
            PhoneNumber,
            Address,
            CustomerId,
            IsActive,
            CreatedDate
        )
        OUTPUT INSERTED.GuestId INTO @GuestIds(GuestId)
        SELECT 
            FirstName,
            LastName,
            Email,
            PhoneNumber,
            Address,
            CASE WHEN IsPrimaryGuest = 1 THEN @CustomerId ELSE NULL END,
            1,
            GETDATE()
        FROM @Guests;
        
        -- Create bookings for each reservation item
        DECLARE @ReservationItemId INT, @PackageId INT, @ServiceItemId INT, @StartDate DATE, @EndDate DATE, 
                @Quantity INT, @SpecialRequests NVARCHAR(500);
        
        DECLARE bookingCursor CURSOR FOR 
        SELECT 
            PackageId,
            ServiceItemId,
            StartDate,
            EndDate,
            Quantity,
            SpecialRequests
        FROM @ReservationItems;
        
        OPEN bookingCursor;
        FETCH NEXT FROM bookingCursor INTO @PackageId, @ServiceItemId, @StartDate, @EndDate, @Quantity, @SpecialRequests;
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            INSERT INTO Booking (
                ReservationId,
                PackageId,
                ServiceItemId,
                StartDate,
                EndDate,
                Quantity,
                StatusId,
                Notes
            )
            VALUES (
                @ReservationId,
                @PackageId,
                @ServiceItemId,
                @StartDate,
                @EndDate,
                @Quantity,
                @StatusId,
                @SpecialRequests
            );
            
            SET @BookingId = SCOPE_IDENTITY();
            
            -- Associate guests with this booking
            INSERT INTO BookingGuest (
                BookingId,
                GuestId,
                IsPrimaryGuest
            )
            SELECT 
                @BookingId,
                gi.GuestId,
                g.IsPrimaryGuest
            FROM @GuestIds gi
            INNER JOIN (
                SELECT *, ROW_NUMBER() OVER (ORDER BY LastName, FirstName) AS RowNum 
                FROM @Guests
            ) g ON gi.RowId = g.RowNum;
            
            -- If it's a package booking, find appropriate facilities and reserve them
            IF @PackageId IS NOT NULL
            BEGIN
                -- Find services in the package that require facilities
                DECLARE @PackageServiceId INT, @PackageServiceFacilityTypeId INT;
                
                DECLARE packageServiceFacilityCursor CURSOR FOR
                SELECT si.ServiceItemId, si.FacilityTypeId
                FROM PackageServiceItem psi
                INNER JOIN ServiceItem si ON psi.ServiceItemId = si.ServiceItemId
                WHERE psi.PackageId = @PackageId AND si.FacilityTypeId IS NOT NULL;
                
                OPEN packageServiceFacilityCursor;
                FETCH NEXT FROM packageServiceFacilityCursor INTO @PackageServiceId, @PackageServiceFacilityTypeId;
                
                WHILE @@FETCH_STATUS = 0
                BEGIN
                    -- Find an available facility of the required type
                    DECLARE @FacilityId INT;
                    
                    SELECT TOP 1 @FacilityId = f.FacilityId
                    FROM Facility f
                    WHERE f.FacilityTypeId = @PackageServiceFacilityTypeId 
                      AND f.HotelId = @HotelId
                      AND f.StatusId = (SELECT StatusId FROM Status WHERE Name = 'Active')
                      AND f.IsActive = 1
                      AND NOT EXISTS (
                          SELECT 1 FROM BookingFacility bf
                          WHERE bf.FacilityId = f.FacilityId
                            AND (
                                (bf.StartDateTime <= CONVERT(DATETIME, @EndDate) + '23:59:59' AND 
                                 bf.EndDateTime >= CONVERT(DATETIME, @StartDate))
                            )
                      );
                    
                    IF @FacilityId IS NOT NULL
                    BEGIN
                        -- Reserve the facility
                        INSERT INTO BookingFacility (
                            BookingId,
                            FacilityId,
                            StartDateTime,
                            EndDateTime
                        )
                        VALUES (
                            @BookingId,
                            @FacilityId,
                            CONVERT(DATETIME, @StartDate),
                            CONVERT(DATETIME, @EndDate) + '23:59:59'
                        );
                    END
                    
                    FETCH NEXT FROM packageServiceFacilityCursor INTO @PackageServiceId, @PackageServiceFacilityTypeId;
                END
                
                CLOSE packageServiceFacilityCursor;
                DEALLOCATE packageServiceFacilityCursor;
            END
            -- If it's a service booking that requires a facility
            ELSE IF @ServiceItemId IS NOT NULL
            BEGIN
                SELECT @FacilityTypeId = FacilityTypeId FROM ServiceItem WHERE ServiceItemId = @ServiceItemId;
                
                IF @FacilityTypeId IS NOT NULL
                BEGIN
                    -- Find an available facility of the required type
                    DECLARE @ServiceFacilityId INT;
                    
                    SELECT TOP 1 @ServiceFacilityId = f.FacilityId
                    FROM Facility f
                    WHERE f.FacilityTypeId = @FacilityTypeId 
                      AND f.HotelId = @HotelId
                      AND f.StatusId = (SELECT StatusId FROM Status WHERE Name = 'Active')
                      AND f.IsActive = 1
                      AND NOT EXISTS (
                          SELECT 1 FROM BookingFacility bf
                          WHERE bf.FacilityId = f.FacilityId
                            AND (
                                (bf.StartDateTime <= CONVERT(DATETIME, @EndDate) + '23:59:59' AND 
                                 bf.EndDateTime >= CONVERT(DATETIME, @StartDate))
                            )
                      );
                    
                    IF @ServiceFacilityId IS NOT NULL
                    BEGIN
                        -- Reserve the facility
                        INSERT INTO BookingFacility (
                            BookingId,
                            FacilityId,
                            StartDateTime,
                            EndDateTime
                        )
                        VALUES (
                            @BookingId,
                            @ServiceFacilityId,
                            CONVERT(DATETIME, @StartDate),
                            CONVERT(DATETIME, @EndDate) + '23:59:59'
                        );
                    END
                END
            END
            
            FETCH NEXT FROM bookingCursor INTO @PackageId, @ServiceItemId, @StartDate, @EndDate, @Quantity, @SpecialRequests;
        END
        
        CLOSE bookingCursor;
        DEALLOCATE bookingCursor;
        
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
        IF OBJECT_ID('dbo.ErrorLog', 'U') IS NOT NULL
        BEGIN
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
        END
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
        
        RETURN -1; -- Failure
    END CATCH;
END;
GO

-- Create a sequence for reservation numbers if it doesn't exist
IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name = 'ReservationNumberSequence')
BEGIN
    CREATE SEQUENCE ReservationNumberSequence
    START WITH 1001
    INCREMENT BY 1;
END
GO
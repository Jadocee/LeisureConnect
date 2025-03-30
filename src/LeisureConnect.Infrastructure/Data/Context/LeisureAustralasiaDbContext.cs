using LeisureConnect.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeisureConnect.Infrastructure.Data.Context;

public class LeisureAustralasiaDbContext : DbContext
{
    public LeisureAustralasiaDbContext()
    {
    }

    public LeisureAustralasiaDbContext(DbContextOptions<LeisureAustralasiaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdvertisedPackage> AdvertisedPackages { get; set; }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<BillCharge> BillCharges { get; set; }

    public virtual DbSet<BillPayment> BillPayments { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingFacility> BookingFacilities { get; set; }

    public virtual DbSet<BookingGuest> BookingGuests { get; set; }

    public virtual DbSet<Charge> Charges { get; set; }

    public virtual DbSet<ChargeType> ChargeTypes { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Facility> Facilities { get; set; }

    public virtual DbSet<FacilityType> FacilityTypes { get; set; }

    public virtual DbSet<Guest> Guests { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<HotelPackage> HotelPackages { get; set; }

    public virtual DbSet<HotelServiceItem> HotelServiceItems { get; set; }

    public virtual DbSet<PackageServiceItem> PackageServiceItems { get; set; }

    public virtual DbSet<PaymentInformation> PaymentInformations { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }

    public virtual DbSet<ServiceItem> ServiceItems { get; set; }

    public virtual DbSet<ServiceType> ServiceTypes { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LeisureAustralasiaDbContext).Assembly);


        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__City__F2D21B76CCF2CD85");

            entity.ToTable("City");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_City_Country");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK__Country__10D1609F29475750");

            entity.ToTable("Country");

            entity.Property(e => e.CountryCode).HasMaxLength(3);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.DefaultCurrency).WithMany(p => p.Countries)
                .HasForeignKey(d => d.DefaultCurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Country_Currency");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.CurrencyId).HasName("PK__Currency__14470AF0D4EE310C");

            entity.ToTable("Currency");

            entity.Property(e => e.CurrencyCode).HasMaxLength(3);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Symbol).HasMaxLength(5);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D8A59C50FC");

            entity.ToTable("Customer");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.JoinDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.LoyaltyPoints).HasDefaultValue(0);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.City).WithMany(p => p.Customers)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_Customer_City");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BEDACC4CE60");

            entity.ToTable("Department");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F11EAD0FB47");

            entity.ToTable("Employee");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MaxDiscountPercentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Department");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Employees)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Hotel");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Role");
        });

        modelBuilder.Entity<Facility>(entity =>
        {
            entity.HasKey(e => e.FacilityId).HasName("PK__Facility__5FB08A7438685852");

            entity.ToTable("Facility");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.FacilityType).WithMany(p => p.Facilities)
                .HasForeignKey(d => d.FacilityTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Facility_FacilityType");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Facilities)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Facility_Hotel");

            entity.HasOne(d => d.Status).WithMany(p => p.Facilities)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Facility_Status");
        });

        modelBuilder.Entity<FacilityType>(entity =>
        {
            entity.HasKey(e => e.FacilityTypeId).HasName("PK__Facility__08F8C3DC98275A37");

            entity.ToTable("FacilityType");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasKey(e => e.GuestId).HasName("PK__Guest__0C423C128FFD90CE");

            entity.ToTable("Guest");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.City).WithMany(p => p.Guests)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_Guest_City");

            entity.HasOne(d => d.Customer).WithMany(p => p.Guests)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Guest_Customer");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.HotelId).HasName("PK__Hotel__46023BDFC8C0D535");

            entity.ToTable("Hotel");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.BaseCurrency).WithMany(p => p.Hotels)
                .HasForeignKey(d => d.BaseCurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Hotel_Currency");

            entity.HasOne(d => d.City).WithMany(p => p.Hotels)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Hotel_City");
        });

        modelBuilder.Entity<HotelPackage>(entity =>
        {
            entity.HasKey(e => e.HotelPackageId).HasName("PK__HotelPac__D3A1CFFB8812D6F0");

            entity.ToTable("HotelPackage");

            entity.HasIndex(e => new { e.HotelId, e.PackageId }, "UQ_HotelPackage").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Hotel).WithMany(p => p.HotelPackages)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HotelPackage_Hotel");

            entity.HasOne(d => d.Package).WithMany(p => p.HotelPackages)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HotelPackage_Package");
        });

        modelBuilder.Entity<HotelServiceItem>(entity =>
        {
            entity.HasKey(e => e.HotelServiceItemId).HasName("PK__HotelSer__9790479790A4F296");

            entity.ToTable("HotelServiceItem");

            entity.HasIndex(e => new { e.HotelId, e.ServiceItemId }, "UQ_HotelServiceItem").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Hotel).WithMany(p => p.HotelServiceItems)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HotelServiceItem_Hotel");

            entity.HasOne(d => d.ServiceItem).WithMany(p => p.HotelServiceItems)
                .HasForeignKey(d => d.ServiceItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HotelServiceItem_ServiceItem");
        });

        modelBuilder.Entity<PackageServiceItem>(entity =>
        {
            entity.HasKey(e => e.PackageServiceItemId).HasName("PK__PackageS__253729595766F727");

            entity.ToTable("PackageServiceItem");

            entity.HasIndex(e => new { e.PackageId, e.ServiceItemId }, "UQ_PackageServiceItem").IsUnique();

            entity.Property(e => e.AdditionalCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Package).WithMany(p => p.PackageServiceItems)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PackageServiceItem_Package");

            entity.HasOne(d => d.ServiceItem).WithMany(p => p.PackageServiceItems)
                .HasForeignKey(d => d.ServiceItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PackageServiceItem_ServiceItem");
        });

        modelBuilder.Entity<PaymentInformation>(entity =>
        {
            entity.HasKey(e => e.PaymentInformationId).HasName("PK__PaymentI__EF3284797FB3759A");

            entity.ToTable("PaymentInformation");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TransactionReference).HasMaxLength(100);

            entity.HasOne(d => d.Currency).WithMany(p => p.PaymentInformations)
                .HasForeignKey(d => d.CurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaymentInformation_Currency");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.PaymentInformations)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaymentInformation_PaymentMethod");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1D32661BF69");

            entity.ToTable("PaymentMethod");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__B7EE5F2431A057A6");

            entity.ToTable("Reservation");

            entity.HasIndex(e => e.ReservationNumber, "UQ_Reservation_Number").IsUnique();

            entity.Property(e => e.CancellationFee).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DepositAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ReservationDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ReservationNumber).HasMaxLength(20);
            entity.Property(e => e.ReservationType).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservation_Customer");

            entity.HasOne(d => d.DepositPaymentInformation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.DepositPaymentInformationId)
                .HasConstraintName("FK_Reservation_PaymentInformation");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservation_Hotel");

            entity.HasOne(d => d.Status).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservation_Status");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A7869425D");

            entity.ToTable("Role");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.MaxDiscountPercentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<ServiceCategory>(entity =>
        {
            entity.HasKey(e => e.ServiceCategoryId).HasName("PK__ServiceC__E4CC7EAA94EA1EAD");

            entity.ToTable("ServiceCategory");

            entity.HasIndex(e => e.Code, "UQ_ServiceCategory_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.ServiceType).WithMany(p => p.ServiceCategories)
                .HasForeignKey(d => d.ServiceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServiceCategory_ServiceType");
        });

        modelBuilder.Entity<ServiceItem>(entity =>
        {
            entity.HasKey(e => e.ServiceItemId).HasName("PK__ServiceI__CC153FB8C00ABC38");

            entity.ToTable("ServiceItem");

            entity.Property(e => e.AvailableTimes).HasMaxLength(255);
            entity.Property(e => e.BaseCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Comments).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Restrictions).HasMaxLength(500);

            entity.HasOne(d => d.BaseCurrency).WithMany(p => p.ServiceItems)
                .HasForeignKey(d => d.BaseCurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServiceItem_Currency");

            entity.HasOne(d => d.FacilityType).WithMany(p => p.ServiceItems)
                .HasForeignKey(d => d.FacilityTypeId)
                .HasConstraintName("FK_ServiceItem_FacilityType");

            entity.HasOne(d => d.ServiceCategory).WithMany(p => p.ServiceItems)
                .HasForeignKey(d => d.ServiceCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServiceItem_ServiceCategory");

            entity.HasOne(d => d.Status).WithMany(p => p.ServiceItems)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServiceItem_Status");
        });

        modelBuilder.Entity<ServiceType>(entity =>
        {
            entity.HasKey(e => e.ServiceTypeId).HasName("PK__ServiceT__8ADFAA6C8726544F");

            entity.ToTable("ServiceType");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Status__C8EE2063718DD1A7");

            entity.ToTable("Status");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(50);
        });
        
        modelBuilder.HasSequence("ReservationNumberSequence").StartsAt(1001L);
    }

}

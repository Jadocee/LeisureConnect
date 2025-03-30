using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class AdvertisedPackage
{
    public int PackageId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal AdvertisedPrice { get; set; }

    public int AdvertisedCurrencyId { get; set; }

    public int StatusId { get; set; }

    public string? Inclusions { get; set; }

    public string? Exclusions { get; set; }

    public int GracePeriodDays { get; set; }

    public int? AuthorizedByEmployeeId { get; set; }

    public bool IsStandardPackage { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public virtual Currency AdvertisedCurrency { get; set; } = null!;

    public virtual Employee? AuthorizedByEmployee { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<HotelPackage> HotelPackages { get; set; } = new List<HotelPackage>();

    public virtual ICollection<PackageServiceItem> PackageServiceItems { get; set; } = new List<PackageServiceItem>();

    public virtual Status Status { get; set; } = null!;
}

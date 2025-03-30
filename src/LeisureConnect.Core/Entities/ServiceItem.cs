using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class ServiceItem
{
    public int ServiceItemId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int ServiceCategoryId { get; set; }

    public int? FacilityTypeId { get; set; }

    public decimal BaseCost { get; set; }

    public int BaseCurrencyId { get; set; }

    public int? Capacity { get; set; }

    public string? AvailableTimes { get; set; }

    public string? Restrictions { get; set; }

    public string? Notes { get; set; }

    public string? Comments { get; set; }

    public int StatusId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public virtual Currency BaseCurrency { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Charge> Charges { get; set; } = new List<Charge>();

    public virtual FacilityType? FacilityType { get; set; }

    public virtual ICollection<HotelServiceItem> HotelServiceItems { get; set; } = new List<HotelServiceItem>();

    public virtual ICollection<PackageServiceItem> PackageServiceItems { get; set; } = new List<PackageServiceItem>();

    public virtual ServiceCategory ServiceCategory { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}

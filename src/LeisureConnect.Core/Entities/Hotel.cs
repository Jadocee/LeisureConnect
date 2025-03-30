using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class Hotel
{
    public int HotelId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int CityId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Description { get; set; }

    public int TotalCapacity { get; set; }

    public int BaseCurrencyId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public virtual Currency BaseCurrency { get; set; } = null!;

    public virtual City City { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Facility> Facilities { get; set; } = new List<Facility>();

    public virtual ICollection<HotelPackage> HotelPackages { get; set; } = new List<HotelPackage>();

    public virtual ICollection<HotelServiceItem> HotelServiceItems { get; set; } = new List<HotelServiceItem>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

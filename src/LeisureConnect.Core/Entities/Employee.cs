using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public int RoleId { get; set; }

    public int DepartmentId { get; set; }

    public int HotelId { get; set; }

    public bool CanAuthorizeDiscounts { get; set; }

    public decimal? MaxDiscountPercentage { get; set; }

    public DateOnly HireDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public virtual ICollection<AdvertisedPackage> AdvertisedPackages { get; set; } = new List<AdvertisedPackage>();

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<Charge> Charges { get; set; } = new List<Charge>();

    public virtual Department Department { get; set; } = null!;

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}

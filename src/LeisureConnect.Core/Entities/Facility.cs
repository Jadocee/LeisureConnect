using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class Facility
{
    public int FacilityId { get; set; }

    public string Name { get; set; } = null!;

    public int FacilityTypeId { get; set; }

    public int HotelId { get; set; }

    public string? Description { get; set; }

    public int? Capacity { get; set; }

    public int StatusId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public virtual ICollection<BookingFacility> BookingFacilities { get; set; } = new List<BookingFacility>();

    public virtual FacilityType FacilityType { get; set; } = null!;

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}

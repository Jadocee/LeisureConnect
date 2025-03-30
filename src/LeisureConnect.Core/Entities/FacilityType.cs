using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class FacilityType
{
    public int FacilityTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? DefaultCapacity { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Facility> Facilities { get; set; } = new List<Facility>();

    public virtual ICollection<ServiceItem> ServiceItems { get; set; } = new List<ServiceItem>();
}

using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class ServiceType
{
    public int ServiceTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ServiceCategory> ServiceCategories { get; set; } = new List<ServiceCategory>();
}

using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class ServiceCategory
{
    public int ServiceCategoryId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int ServiceTypeId { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ServiceItem> ServiceItems { get; set; } = new List<ServiceItem>();

    public virtual ServiceType ServiceType { get; set; } = null!;
}

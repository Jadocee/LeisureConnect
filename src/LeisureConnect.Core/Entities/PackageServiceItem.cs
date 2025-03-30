using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class PackageServiceItem
{
    public int PackageServiceItemId { get; set; }

    public int PackageId { get; set; }

    public int ServiceItemId { get; set; }

    public int Quantity { get; set; }

    public bool IsOptional { get; set; }

    public decimal? AdditionalCost { get; set; }

    public virtual AdvertisedPackage Package { get; set; } = null!;

    public virtual ServiceItem ServiceItem { get; set; } = null!;
}

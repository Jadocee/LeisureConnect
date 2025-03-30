using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class HotelPackage
{
    public int HotelPackageId { get; set; }

    public int HotelId { get; set; }

    public int PackageId { get; set; }

    public bool IsActive { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual AdvertisedPackage Package { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class HotelServiceItem
{
    public int HotelServiceItemId { get; set; }

    public int HotelId { get; set; }

    public int ServiceItemId { get; set; }

    public bool IsActive { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual ServiceItem ServiceItem { get; set; } = null!;
}

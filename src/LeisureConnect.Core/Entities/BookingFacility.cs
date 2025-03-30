using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class BookingFacility
{
    public int BookingFacilityId { get; set; }

    public int BookingId { get; set; }

    public int FacilityId { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Facility Facility { get; set; } = null!;
}

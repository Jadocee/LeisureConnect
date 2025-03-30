using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class BookingGuest
{
    public int BookingGuestId { get; set; }

    public int BookingId { get; set; }

    public int GuestId { get; set; }

    public bool IsPrimaryGuest { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Guest Guest { get; set; } = null!;
}

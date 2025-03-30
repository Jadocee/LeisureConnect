using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class Booking
{
    public int BookingId { get; set; }

    public int ReservationId { get; set; }

    public int? PackageId { get; set; }

    public int? ServiceItemId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int Quantity { get; set; }

    public int StatusId { get; set; }

    public DateTime? CheckInDateTime { get; set; }

    public DateTime? CheckOutDateTime { get; set; }

    public DateTime? CancellationDate { get; set; }

    public bool? IsCancelledAfterGracePeriod { get; set; }

    public decimal? CancellationFee { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<BookingFacility> BookingFacilities { get; set; } = new List<BookingFacility>();

    public virtual ICollection<BookingGuest> BookingGuests { get; set; } = new List<BookingGuest>();

    public virtual ICollection<Charge> Charges { get; set; } = new List<Charge>();

    public virtual AdvertisedPackage? Package { get; set; }

    public virtual Reservation Reservation { get; set; } = null!;

    public virtual ServiceItem? ServiceItem { get; set; }

    public virtual Status Status { get; set; } = null!;
}

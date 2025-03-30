using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public string ReservationNumber { get; set; } = null!;

    public int CustomerId { get; set; }

    public int HotelId { get; set; }

    public DateTime ReservationDate { get; set; }

    public string ReservationType { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public decimal? DepositAmount { get; set; }

    public int? DepositPaymentInformationId { get; set; }

    public bool IsFullyPaid { get; set; }

    public int StatusId { get; set; }

    public DateTime? CancellationDate { get; set; }

    public bool? IsCancelledAfterGracePeriod { get; set; }

    public decimal? CancellationFee { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Customer Customer { get; set; } = null!;

    public virtual PaymentInformation? DepositPaymentInformation { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}

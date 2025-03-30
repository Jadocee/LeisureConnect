using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class Guest
{
    public int GuestId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public int? CityId { get; set; }

    public int? CustomerId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public virtual ICollection<BookingGuest> BookingGuests { get; set; } = new List<BookingGuest>();

    public virtual City? City { get; set; }

    public virtual Customer? Customer { get; set; }
}

using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public int? CityId { get; set; }

    public int? LoyaltyPoints { get; set; }

    public DateOnly? JoinDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public virtual City? City { get; set; }

    public virtual ICollection<Guest> Guests { get; set; } = new List<Guest>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

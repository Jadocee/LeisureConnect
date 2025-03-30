using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class City
{
    public int CityId { get; set; }

    public string Name { get; set; } = null!;

    public int CountryId { get; set; }

    public bool IsActive { get; set; }

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Guest> Guests { get; set; } = new List<Guest>();

    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}

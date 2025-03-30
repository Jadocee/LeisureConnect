using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class Country
{
    public int CountryId { get; set; }

    public string Name { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public int DefaultCurrencyId { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual Currency DefaultCurrency { get; set; } = null!;
}

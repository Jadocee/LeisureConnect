using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class Currency
{
    public int CurrencyId { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Symbol { get; set; }

    public bool IsBaseCurrency { get; set; }

    public virtual ICollection<AdvertisedPackage> AdvertisedPackages { get; set; } = new List<AdvertisedPackage>();

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<Charge> Charges { get; set; } = new List<Charge>();

    public virtual ICollection<Country> Countries { get; set; } = new List<Country>();

    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();

    public virtual ICollection<PaymentInformation> PaymentInformations { get; set; } = new List<PaymentInformation>();

    public virtual ICollection<ServiceItem> ServiceItems { get; set; } = new List<ServiceItem>();
}

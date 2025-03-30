using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class Charge
{
    public int ChargeId { get; set; }

    public int BookingId { get; set; }

    public int? ServiceItemId { get; set; }

    public int ChargeTypeId { get; set; }

    public decimal Amount { get; set; }

    public int CurrencyId { get; set; }

    public string? Description { get; set; }

    public DateTime ChargeDate { get; set; }

    public int? EmployeeId { get; set; }

    public bool IsPackageInclusion { get; set; }

    public virtual ICollection<BillCharge> BillCharges { get; set; } = new List<BillCharge>();

    public virtual Booking Booking { get; set; } = null!;

    public virtual ChargeType ChargeType { get; set; } = null!;

    public virtual Currency Currency { get; set; } = null!;

    public virtual Employee? Employee { get; set; }

    public virtual ServiceItem? ServiceItem { get; set; }
}

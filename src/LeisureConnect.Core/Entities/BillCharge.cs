using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class BillCharge
{
    public int BillChargeId { get; set; }

    public int BillId { get; set; }

    public int ChargeId { get; set; }

    public virtual Bill Bill { get; set; } = null!;

    public virtual Charge Charge { get; set; } = null!;
}

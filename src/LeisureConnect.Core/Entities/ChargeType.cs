using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class ChargeType
{
    public int ChargeTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Charge> Charges { get; set; } = new List<Charge>();
}

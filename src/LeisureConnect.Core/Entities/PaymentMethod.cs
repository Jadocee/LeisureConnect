using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<PaymentInformation> PaymentInformations { get; set; } = new List<PaymentInformation>();
}

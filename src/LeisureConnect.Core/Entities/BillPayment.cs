using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class BillPayment
{
    public int BillPaymentId { get; set; }

    public int BillId { get; set; }

    public int PaymentInformationId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public virtual Bill Bill { get; set; } = null!;

    public virtual PaymentInformation PaymentInformation { get; set; } = null!;
}

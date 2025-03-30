using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class PaymentInformation
{
    public int PaymentInformationId { get; set; }

    public DateTime PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public int PaymentMethodId { get; set; }

    public string? TransactionReference { get; set; }

    public string Status { get; set; } = null!;

    public int CurrencyId { get; set; }

    public virtual ICollection<BillPayment> BillPayments { get; set; } = new List<BillPayment>();

    public virtual Currency Currency { get; set; } = null!;

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

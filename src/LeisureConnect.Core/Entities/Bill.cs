using System;
using System.Collections.Generic;

namespace LeisureConnect.Core.Entities;

public partial class Bill
{
    public int BillId { get; set; }

    public int ReservationId { get; set; }

    public DateTime IssuedDate { get; set; }

    public DateTime DueDate { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? DepositAmount { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public bool IsDiscountAuthorized { get; set; }

    public int? DiscountAuthorizedByEmployeeId { get; set; }

    public bool RequiresHeadOfficeAuthorization { get; set; }

    public string? HeadOfficeAuthorizationStatus { get; set; }

    public decimal PaidAmount { get; set; }

    public int StatusId { get; set; }

    public string? Notes { get; set; }

    public int CurrencyId { get; set; }

    public virtual ICollection<BillCharge> BillCharges { get; set; } = new List<BillCharge>();

    public virtual ICollection<BillPayment> BillPayments { get; set; } = new List<BillPayment>();

    public virtual Currency Currency { get; set; } = null!;

    public virtual Employee? DiscountAuthorizedByEmployee { get; set; }

    public virtual Reservation Reservation { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}

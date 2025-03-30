using System.ComponentModel.DataAnnotations;

namespace LeisureConnect.Core.DTOs;

public class ReservationResponse
{
    public int ReservationId { get; set; }
    public string ReservationNumber { get; set; }
    public DateTime ReservationDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? DepositAmount { get; set; }
    public string CustomerFullName { get; set; }
    public string HotelName { get; set; }
    public string Status { get; set; }
    public string CurrencyCode { get; set; }
}
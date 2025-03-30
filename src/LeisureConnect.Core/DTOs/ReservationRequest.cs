using System.ComponentModel.DataAnnotations;

namespace LeisureConnect.Core.DTOs;

public class ReservationRequest
{
    #region Customer Details
    [Required]
    [StringLength(50)]
    public string CustomerFirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string CustomerLastName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string CustomerAddress { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string CustomerPhoneNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string CustomerEmail { get; set; } = string.Empty;

    public int? CustomerCityId { get; set; }
    #endregion

    #region Reservation Details
    [Required]
    public int HotelId { get; set; }

    [Required]
    [StringLength(50)]
    public string ReservationType { get; set; } = "Phone"; // TODO: Make this an enum
    #endregion

    #region Payment Details
    public int? PaymentMethodId { get; set; }
    public string PaymentReference { get; set; } = string.Empty;
    public int CurrencyId { get; set; } = 1; // TODO: Make this an enum (1 = AUD, 2 = NZD, 3 = USD, 4 = GBP, 5 = EUR)
    #endregion

    #region Reserved Items
    [Required]
    [MinLength(1, ErrorMessage = "At least one reservation item is required.")]
    public List<ReservationItemDto> ReservationItems { get; set; } = new List<ReservationItemDto>();

    [Required]
    [MinLength(1, ErrorMessage = "At least one guest is required.")]
    public List<GuestDto> Guests { get; set; } = new List<GuestDto>();
    #endregion
}
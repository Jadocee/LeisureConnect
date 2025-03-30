using System.ComponentModel.DataAnnotations;

namespace LeisureConnect.Core.DTOs;

public class ReservationItemDto
{
    public int? PackageId { get; set; }
    public int? ServiceItemId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public int Quantity { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateOnly StartDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateOnly EndDate { get; set; }

    public string SpecialRequests { get; set; } = string.Empty;
}
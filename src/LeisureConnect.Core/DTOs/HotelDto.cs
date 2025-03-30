namespace LeisureConnect.Core.DTOs;

public class HotelDto
{
    public int HotelId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public int CityId { get; set; }
    public string Country { get; set; }
    public int CountryId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}
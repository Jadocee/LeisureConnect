namespace LeisureConnect.Core.DTOs;

public class CountryDto
{
    public int CountryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public List<CityDto> Cities { get; set; } = new List<CityDto>();
}
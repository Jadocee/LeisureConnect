namespace LeisureConnect.Core.DTOs;

public class FacilityDto
{
    public int FacilityId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string FacilityTypeName { get; set; }
    public int? Capacity { get; set; }
    public string Status { get; set; }
}
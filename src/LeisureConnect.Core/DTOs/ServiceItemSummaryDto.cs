namespace LeisureConnect.Core.DTOs;

public class ServiceItemSummaryDto
{
    public int ServiceItemId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal BaseCost { get; set; }
    public string CurrencyCode { get; set; }
    public int CurrencyId { get; set; }
    public string ServiceCategory { get; set; }
    public int ServiceCategoryId { get; set; }
}
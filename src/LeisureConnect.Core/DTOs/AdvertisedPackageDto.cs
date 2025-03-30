using System.ComponentModel.DataAnnotations;

namespace LeisureConnect.Core.DTOs;

public class AdvertisedPackageDto
{
    public int PackageId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal AdvertisedPrice { get; set; }
    public string CurrencyCode { get; set; }
    public int CurrencyId { get; set; }
    public string? Inclusions { get; set; }
    public string? Exclusions { get; set; }
    public int GracePeriodDays { get; set; }
    public bool IsStandardPackage { get; set; }
    public List<ServiceItemSummaryDto> IncludedServices { get; set; }
}
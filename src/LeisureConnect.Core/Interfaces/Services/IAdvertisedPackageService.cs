using LeisureConnect.Core.DTOs;
using LeisureConnect.Core.Entities;

namespace LeisureConnect.Core.Interfaces.Services;

public interface IAdvertisedPackageService
{
    Task<List<AdvertisedPackageDto>> GetAvailablePackagesAsync(
        int hotelId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);
    Task<AdvertisedPackageDto?> GetAdvertisedPackageByIdAsync(
        int id, 
        CancellationToken cancellationToken = default);
}
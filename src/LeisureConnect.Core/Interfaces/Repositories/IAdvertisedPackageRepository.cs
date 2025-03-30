using LeisureConnect.Core.Entities;

namespace LeisureConnect.Core.Interfaces.Repositories;

public interface IAdvertisedPackageRepository
{
    Task<AdvertisedPackage?> GetAdvertisedPackageByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
    Task<bool> IsAdvertisedPackageAvailableAtHotelAsync(
        int advertisedPackageId,
        int hotelId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);
    Task<List<AdvertisedPackage>> GetAvailablePackagesAsync(
        int hotelId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);
    
}
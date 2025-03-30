using LeisureConnect.Core.DTOs;
using LeisureConnect.Core.Entities;
using LeisureConnect.Core.Interfaces.Repositories;
using LeisureConnect.Core.Interfaces.Services;
using LeisureConnect.Infrastructure.Data.Context;
using LeisureConnect.Infrastructure.Extensions;
using LeisureConnect.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LeisureConnect.Infrastructure.Services;

public class AdvertisedPackageService : IAdvertisedPackageService
{
    private readonly IAdvertisedPackageRepository _advertisedPackageRepository;
    private readonly IHotelRepository _hotelRepository;

    public AdvertisedPackageService(IDbContextFactory<LeisureAustralasiaDbContext> contextFactory)
    {
        LeisureAustralasiaDbContext context = contextFactory.CreateDbContext();
        _advertisedPackageRepository = new AdvertisedPackageRepository(context);
        _hotelRepository = new HotelRepository(context);
    }

    public async Task<AdvertisedPackageDto?> GetAdvertisedPackageByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        AdvertisedPackage? package = await _advertisedPackageRepository.GetAdvertisedPackageByIdAsync(id, cancellationToken);
        if (package == null)
        {
            return null;
        }

        return package.ToDto();
    }

    public async Task<List<AdvertisedPackageDto>> GetAvailablePackagesAsync(
        int hotelId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        bool hotelExists = await _hotelRepository.HotelExistsAsync(hotelId, cancellationToken);
        if (!hotelExists)
        {
            throw new ArgumentException($"Hotel with ID {hotelId} does not exist.", nameof(hotelId));
        }

        if (startDate > endDate)
        {
            throw new ArgumentException("Start date cannot be later than end date.", nameof(startDate));
        }

        List<AdvertisedPackage> packages = await _advertisedPackageRepository.GetAvailablePackagesAsync(
            hotelId,
            startDate,
            endDate,
            cancellationToken);

        return packages.Select(p => p.ToDto()).ToList();        
    }
}
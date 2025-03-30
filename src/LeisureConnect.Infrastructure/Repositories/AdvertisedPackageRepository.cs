using LeisureConnect.Core.Entities;
using LeisureConnect.Core.Interfaces.Repositories;
using LeisureConnect.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LeisureConnect.Infrastructure.Repositories;

public class AdvertisedPackageRepository : IAdvertisedPackageRepository
{
    private readonly LeisureAustralasiaDbContext _context;

    public AdvertisedPackageRepository(LeisureAustralasiaDbContext context)
    {
        _context = context;
    }

    public async Task<AdvertisedPackage?> GetAdvertisedPackageByIdAsync(
        int id, 
        CancellationToken cancellationToken = default)
    {
        return await _context.AdvertisedPackages
            .AsNoTracking()
            .Include(p => p.AdvertisedCurrency)
            .Include(p => p.PackageServiceItems)
                .ThenInclude(ps => ps.ServiceItem)
                    .ThenInclude(s => s.BaseCurrency)
            .Include(p => p.PackageServiceItems)
                .ThenInclude(ps => ps.ServiceItem)
                    .ThenInclude(s => s.ServiceCategory)
            .Where(p => 
                    p.PackageId == id &&
                    p.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsAdvertisedPackageAvailableAtHotelAsync(
        int advertisedPackageId,
        int hotelId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        int activeStatusId = await _context.Statuses
            .AsNoTracking()
            .Where(s => EF.Functions.Like(s.Name, "Active"))
            .Select(s => s.StatusId)
            .FirstOrDefaultAsync(cancellationToken);

        if (activeStatusId == 0)
        {
            return false;
        }

        return await _context.AdvertisedPackages
            .AsNoTracking()
            .Where(ap =>
                ap.PackageId == advertisedPackageId &&
                ap.IsActive &&
                ap.StatusId == activeStatusId &&
                ap.StartDate <= startDate &&
                ap.EndDate >= endDate &&
                ap.HotelPackages.Any(hp => hp.HotelId == hotelId && hp.IsActive))
            .AnyAsync(cancellationToken);
    }

    public async Task<List<AdvertisedPackage>> GetAvailablePackagesAsync(
        int hotelId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.AdvertisedPackages
            .AsNoTracking()
            .Include(p => p.AdvertisedCurrency)
            .Include(p => p.PackageServiceItems)
                .ThenInclude(ps => ps.ServiceItem)
                    .ThenInclude(s => s.BaseCurrency)
            .Include(p => p.PackageServiceItems)
                .ThenInclude(ps => ps.ServiceItem)
                    .ThenInclude(s => s.ServiceCategory)
            .Where(ap =>
                ap.IsActive &&
                ap.StartDate <= startDate &&
                ap.EndDate >= endDate &&
                ap.HotelPackages.Any(hp => hp.HotelId == hotelId && hp.IsActive))
            .ToListAsync(cancellationToken);
    }
}
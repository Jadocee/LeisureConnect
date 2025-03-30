using LeisureConnect.Core.Entities;
using LeisureConnect.Core.Interfaces.Repositories;
using LeisureConnect.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LeisureConnect.Infrastructure.Repositories;

public class ServiceItemRepository : IServiceItemRepository
{
    private readonly LeisureAustralasiaDbContext _context;

    public ServiceItemRepository(LeisureAustralasiaDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceItem?> GetServiceItemByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.ServiceItems
            .AsNoTracking()
            .Where(si =>
                si.ServiceItemId == id &&
                si.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsServiceItemAvailableAtHotelAsync(
        int serviceItemId,
        int hotelId,
        CancellationToken cancellationToken = default)
    {
        int activeStatusId = await _context.Statuses
            .AsNoTracking()
            .Where(s => s.Name == "Active")
            .Select(s => s.StatusId)
            .FirstOrDefaultAsync(cancellationToken);

        if (activeStatusId == 0)
        {
            return false;
        }

        return await _context.HotelServiceItems
            .AsNoTracking()
            .Where(hsi =>
                hsi.ServiceItemId == serviceItemId &&
                hsi.HotelId == hotelId &&
                hsi.IsActive &&
                hsi.ServiceItem.IsActive &&
                hsi.ServiceItem.StatusId == activeStatusId)
            .AnyAsync(cancellationToken);
    }
}
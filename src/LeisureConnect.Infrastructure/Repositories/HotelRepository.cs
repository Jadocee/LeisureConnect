using LeisureConnect.Core.Entities;
using LeisureConnect.Core.Interfaces.Repositories;
using LeisureConnect.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LeisureConnect.Infrastructure.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly LeisureAustralasiaDbContext _context;

    public HotelRepository(LeisureAustralasiaDbContext context)
    {
        _context = context;
    }

    public async Task<List<Hotel>> GetAllHotelsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Hotels
            .AsNoTracking()
            .Include(h => h.City)
                .ThenInclude(c => c.Country)
            .Where(h => h.IsActive)
            .OrderBy(h => h.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Hotel?> GetHotelByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Hotels
            .AsNoTracking()
            .Include(h => h.City)
                .ThenInclude(c => c.Country)
            .Where(h => 
                h.HotelId == id &&
                h.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> HotelExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Hotels
            .AsNoTracking()
            .Where(h => h.HotelId == id)
            .AnyAsync(cancellationToken);
    }
}
using LeisureConnect.Core.DTOs;
using LeisureConnect.Core.Entities;
using LeisureConnect.Core.Interfaces.Repositories;
using LeisureConnect.Core.Interfaces.Services;
using LeisureConnect.Infrastructure.Data.Context;
using LeisureConnect.Infrastructure.Extensions;
using LeisureConnect.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LeisureConnect.Infrastructure.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;

    public HotelService(IDbContextFactory<LeisureAustralasiaDbContext> dbContextFactory)
    {
        LeisureAustralasiaDbContext context = dbContextFactory.CreateDbContext();
        _hotelRepository = new HotelRepository(context);
    }

    public async Task<List<HotelDto>> GetAllHotelsAsync(CancellationToken cancellationToken = default)
    {
        List<Hotel> hotels = await _hotelRepository.GetAllHotelsAsync(cancellationToken);
        return hotels.Select(h => h.ToDto()).ToList();
    }

    public async Task<HotelDto?> GetHotelByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        Hotel? hotel = await _hotelRepository.GetHotelByIdAsync(id, cancellationToken);
        return hotel?.ToDto();
    }

    public async Task<bool> HotelExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _hotelRepository.HotelExistsAsync(id, cancellationToken);
    }
}
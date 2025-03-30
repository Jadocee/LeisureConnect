using LeisureConnect.Core.DTOs;

namespace LeisureConnect.Core.Interfaces.Services;

public interface IHotelService
{
    Task<List<HotelDto>> GetAllHotelsAsync(CancellationToken cancellationToken = default);
    Task<HotelDto?> GetHotelByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> HotelExistsAsync(int id, CancellationToken cancellationToken = default);
}
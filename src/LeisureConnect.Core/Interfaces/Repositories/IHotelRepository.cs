using LeisureConnect.Core.Entities;

namespace LeisureConnect.Core.Interfaces.Repositories;

public interface IHotelRepository
{
    Task<List<Hotel>> GetAllHotelsAsync(CancellationToken cancellationToken = default);
    Task<Hotel?> GetHotelByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> HotelExistsAsync(int id, CancellationToken cancellationToken = default);
}
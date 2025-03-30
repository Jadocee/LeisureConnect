using LeisureConnect.Core.Entities;

namespace LeisureConnect.Core.Interfaces.Repositories;

public interface IServiceItemRepository
{
    Task<ServiceItem?> GetServiceItemByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> IsServiceItemAvailableAtHotelAsync(
        int serviceItemId,
        int hotelId,
        CancellationToken cancellationToken = default);
}
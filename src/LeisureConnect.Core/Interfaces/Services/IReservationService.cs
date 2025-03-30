using LeisureConnect.Core.DTOs;

namespace LeisureConnect.Core.Interfaces.Services;

public interface IReservationService
{
    Task<ReservationResponse?> CreateReservationAsync(ReservationRequest request, CancellationToken cancellationToken = default);
    Task<ReservationResponse?> GetReservationByIdAsync(int reservationId, CancellationToken cancellationToken = default);
}
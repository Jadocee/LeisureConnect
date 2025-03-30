using LeisureConnect.Core.DTOs;

namespace LeisureConnect.Core.Interfaces.Repositories;

public interface IReservationRepository
{
    Task<(int ReservationId, string ReservationNumber)> CreateReservationAsync(ReservationRequest request, CancellationToken cancellationToken = default);
    Task<ReservationResponse?> GetReservationByIdAsync(int reservationId, CancellationToken cancellationToken = default);
}
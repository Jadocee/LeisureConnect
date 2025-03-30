using LeisureConnect.Core.DTOs;
using LeisureConnect.Core.Entities;
using LeisureConnect.Core.Interfaces.Repositories;
using LeisureConnect.Core.Interfaces.Services;
using LeisureConnect.Infrastructure.Data.Context;
using LeisureConnect.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LeisureConnect.Infrastructure.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IServiceItemRepository _serviceItemRepository;
    private readonly IAdvertisedPackageRepository _advertisedPackageRepository;

    public ReservationService(IDbContextFactory<LeisureAustralasiaDbContext> dbContextFactory)
    {
        LeisureAustralasiaDbContext context = dbContextFactory.CreateDbContext();
        _reservationRepository = new ReservationRepository(context);
        _hotelRepository = new HotelRepository(context);
        _serviceItemRepository = new ServiceItemRepository(context);
        _advertisedPackageRepository = new AdvertisedPackageRepository(context);
    }

    public async Task<ReservationResponse?> CreateReservationAsync(ReservationRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateReservationRequestAsync(request, cancellationToken);
        (int reservationId, string reservationNumber) = await _reservationRepository.CreateReservationAsync(request, cancellationToken);

        return await _reservationRepository.GetReservationByIdAsync(reservationId, cancellationToken);
    }

    public async Task<ReservationResponse?> GetReservationByIdAsync(int reservationId, CancellationToken cancellationToken = default)
    {
        return await _reservationRepository.GetReservationByIdAsync(reservationId, cancellationToken);
    }

    private async Task ValidateReservationRequestAsync(ReservationRequest request, CancellationToken cancellationToken = default)
    {
        Hotel? hotel = await _hotelRepository.GetHotelByIdAsync(request.HotelId, cancellationToken);
        if (hotel == null)
        {
            throw new ArgumentException($"Hotel with ID {request.HotelId} does not exist.", nameof(request.HotelId));
        }

        foreach (ReservationItemDto item in request.ReservationItems)
        {
            if (item.EndDate < item.StartDate)
            {
                throw new ArgumentException("End date cannot be earlier than start date.", nameof(item.EndDate));
            }

            if (item.StartDate <= DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new ArgumentException("Start date cannot be in the past.", nameof(item.StartDate));
            }

            if ((item.PackageId == null && item.ServiceItemId == null) ||
                (item.PackageId != null && item.ServiceItemId != null))
            {
                throw new ArgumentException("Either PackageId or ServiceItemId must be provided, not both.", nameof(item));
            }

            if (item.PackageId.HasValue)
            {
                AdvertisedPackage? package = await _advertisedPackageRepository.GetAdvertisedPackageByIdAsync(item.PackageId.Value, cancellationToken);
                if (package == null)
                {
                    throw new ArgumentException($"Advertised package with ID {item.PackageId} does not exist.", nameof(item.PackageId));
                }

                bool isAvailable = await _advertisedPackageRepository.IsAdvertisedPackageAvailableAtHotelAsync(
                    item.PackageId.Value,
                    request.HotelId,
                    item.StartDate,
                    item.EndDate,
                    cancellationToken);

                if (!isAvailable)
                {
                    throw new ArgumentException($"Advertised package with ID {item.PackageId} is not available at the selected hotel.", nameof(item.PackageId));
                }
            }

            if (item.ServiceItemId.HasValue)
            {
                ServiceItem? serviceItem = await _serviceItemRepository.GetServiceItemByIdAsync(item.ServiceItemId.Value, cancellationToken);
                if (serviceItem == null)
                {
                    throw new ArgumentException($"Service item with ID {item.ServiceItemId} does not exist.", nameof(item.ServiceItemId));
                }

                bool isAvailable = await _serviceItemRepository.IsServiceItemAvailableAtHotelAsync(
                    item.ServiceItemId.Value,
                    request.HotelId,
                    cancellationToken);

                if (!isAvailable)
                {
                    throw new ArgumentException($"Service item with ID {item.ServiceItemId} is not available at the selected hotel.", nameof(item.ServiceItemId));
                }
            }

            bool hasPrimaryGuest = false;
            foreach (GuestDto guest in request.Guests)
            {
                if (guest.IsPrimaryGuest)
                {
                    hasPrimaryGuest = true;
                    break;
                }
            }

            if (!hasPrimaryGuest)
            {
                throw new ArgumentException("At least one guest must be marked as primary.", nameof(request.Guests));
            }
        }
    }
}
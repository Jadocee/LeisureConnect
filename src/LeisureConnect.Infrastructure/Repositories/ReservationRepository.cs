using System.Data;
using System.Data.Common;
using LeisureConnect.Core.DTOs;
using LeisureConnect.Core.Interfaces.Repositories;
using LeisureConnect.Infrastructure.Data.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace LeisureConnect.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly LeisureAustralasiaDbContext _context;

    public ReservationRepository(LeisureAustralasiaDbContext context)
    {
        _context = context;
    }

    public async Task<(int ReservationId, string ReservationNumber)> CreateReservationAsync(ReservationRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            SqlParameter reservationIdParam = new SqlParameter()
            {
                ParameterName = "@ReservationId",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            SqlParameter reservationNumberParam = new SqlParameter()
            {
                ParameterName = "@ReservationNumber",
                SqlDbType = SqlDbType.NVarChar,
                Size = 20,
                Direction = ParameterDirection.Output
            };

            SqlParameter reservationItemsParam = CreateReservationItemsParameter(request.ReservationItems);
            SqlParameter guestsParam = CreateGuestsParameter(request.Guests);

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC usp_MakeReservation @CustomerFirstName, @CustomerLastName, @CustomerEmail, @CustomerPhoneNumber, " +
                "@CustomerAddress, @CustomerCityId, @HotelId, @ReservationType, @PaymentMethodId, @PaymentReference, " +
                "@CurrencyId, @ReservationItems, @Guests, @ReservationId OUTPUT, @ReservationNumber OUTPUT",
                new SqlParameter("@CustomerFirstName", request.CustomerFirstName),
                new SqlParameter("@CustomerLastName", request.CustomerLastName),
                new SqlParameter("@CustomerEmail", request.CustomerEmail ?? (object)DBNull.Value),
                new SqlParameter("@CustomerPhoneNumber", request.CustomerPhoneNumber),
                new SqlParameter("@CustomerAddress", request.CustomerAddress),
                new SqlParameter("@CustomerCityId", request.CustomerCityId ?? (object)DBNull.Value),
                new SqlParameter("@HotelId", request.HotelId),
                new SqlParameter("@ReservationType", request.ReservationType),
                new SqlParameter("@PaymentMethodId", request.PaymentMethodId ?? (object)DBNull.Value),
                new SqlParameter("@PaymentReference", request.PaymentReference ?? (object)DBNull.Value),
                new SqlParameter("@CurrencyId", request.CurrencyId),
                reservationItemsParam,
                guestsParam,
                reservationIdParam,
                reservationNumberParam);

            int reservationId = (int)reservationIdParam.Value;
            string reservationNumber = (string)reservationNumberParam.Value;

            return new(reservationId, reservationNumber);
        }
        catch
        {
            throw;
        }
    }

    public async Task<ReservationResponse?> GetReservationByIdAsync(int reservationId, CancellationToken cancellationToken = default)
    {
        try
        {
            ReservationResponse? reservationResponse = await _context.Reservations
                .AsNoTracking()
                .Where(r => r.ReservationId == reservationId)
                .Select(r => new ReservationResponse()
                {
                    ReservationId = r.ReservationId,
                    ReservationNumber = r.ReservationNumber,
                    ReservationDate = r.ReservationDate,
                    TotalAmount = r.TotalAmount,
                    DepositAmount = r.DepositAmount,
                    CustomerFullName = $"{r.Customer.FirstName} {r.Customer.LastName}",
                    HotelName = r.Hotel.Name,
                    Status = r.Status.Name,
                    CurrencyCode = r.Hotel.BaseCurrency.CurrencyCode
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (reservationResponse == null)
            {
                return null;
            }

            return reservationResponse;
        }
        catch (Exception ex)
        {
            // Log the exception (not implemented here)
            throw new Exception("An error occurred while retrieving the reservation.", ex);
        }
    }

    private static SqlParameter CreateReservationItemsParameter(List<ReservationItemDto> items)
    {
        DataTable table = new DataTable();
        table.Columns.Add("PackageId", typeof(int));
        table.Columns.Add("ServiceItemId", typeof(int));
        table.Columns.Add("Quantity", typeof(int));
        table.Columns.Add("StartDate", typeof(DateTime));
        table.Columns.Add("EndDate", typeof(DateTime));
        table.Columns.Add("SpecialRequests", typeof(string));

        foreach (ReservationItemDto item in items)
        {
            table.Rows.Add(
                item.PackageId.HasValue ? item.PackageId : DBNull.Value,
                item.ServiceItemId.HasValue ? item.ServiceItemId : DBNull.Value,
                item.Quantity,
                item.StartDate,
                item.EndDate,
                item.SpecialRequests ?? (object)DBNull.Value);
        }

        SqlParameter parameter = new SqlParameter("@ReservationItems", SqlDbType.Structured)
        {
            TypeName = "ReservationItemList",
            Value = table
        };

        return parameter;
    }

    private static SqlParameter CreateGuestsParameter(List<GuestDto> guests)
    {
        DataTable table = new DataTable();
        table.Columns.Add("FirstName", typeof(string));
        table.Columns.Add("LastName", typeof(string));
        table.Columns.Add("Email", typeof(string));
        table.Columns.Add("PhoneNumber", typeof(string));
        table.Columns.Add("Address", typeof(string));
        table.Columns.Add("IsPrimaryGuest", typeof(bool));

        foreach (GuestDto guest in guests)
        {
            table.Rows.Add(
                guest.FirstName,
                guest.LastName,
                guest.Email ?? (object)DBNull.Value,
                guest.PhoneNumber ?? (object)DBNull.Value,
                guest.Address ?? (object)DBNull.Value,
                guest.IsPrimaryGuest);
        }

        SqlParameter parameter = new SqlParameter("@Guests", SqlDbType.Structured)
        {
            TypeName = "GuestList",
            Value = table
        };

        return parameter;
    }
}
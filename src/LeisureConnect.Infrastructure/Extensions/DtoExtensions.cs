using LeisureConnect.Core.DTOs;
using LeisureConnect.Core.Entities;

namespace LeisureConnect.Infrastructure.Extensions;

public static class DtoExtensions
{
    public static HotelDto ToDto(this Hotel hotel)
    {
        return new HotelDto()
        {
            HotelId = hotel.HotelId,
            Name = hotel.Name,
            Address = hotel.Address,
            City = hotel.City.Name,
            CityId = hotel.CityId,
            Country = hotel.City.Country.Name,
            CountryId = hotel.City.CountryId,
            PhoneNumber = hotel.PhoneNumber,
            Email = hotel.Email
        };
    }

    public static AdvertisedPackageDto ToDto(this AdvertisedPackage package)
    {
        return new AdvertisedPackageDto()
        {
            PackageId = package.PackageId,
            Name = package.Name,
            Description = package.Description,
            StartDate = package.StartDate,
            EndDate = package.EndDate,
            AdvertisedPrice = package.AdvertisedPrice,
            CurrencyCode = package.AdvertisedCurrency.CurrencyCode,
            CurrencyId = package.AdvertisedCurrency.CurrencyId,
            Inclusions = package.Inclusions,
            Exclusions = package.Exclusions,
            GracePeriodDays = package.GracePeriodDays,
            IsStandardPackage = package.IsStandardPackage,
            IncludedServices = package.PackageServiceItems.Select(x => x.ServiceItem.ToDto()).ToList()
        };
    }

    public static ServiceItemSummaryDto ToDto(this ServiceItem serviceItem)
    {
        return new ServiceItemSummaryDto()
        {
            ServiceItemId = serviceItem.ServiceItemId,
            Name = serviceItem.Name,
            Description = serviceItem.Description,
            BaseCost = serviceItem.BaseCost,
            CurrencyCode = serviceItem.BaseCurrency.CurrencyCode,
            CurrencyId = serviceItem.BaseCurrencyId,
            ServiceCategory = serviceItem.ServiceCategory.Name,
            ServiceCategoryId = serviceItem.ServiceCategoryId
        };
    }

    public static PaymentMethodDto ToDto(this PaymentMethod paymentMethod)
    {
        return new PaymentMethodDto()
        {
            PaymentMethodId = paymentMethod.PaymentMethodId,
            Name = paymentMethod.Name,
            Description = paymentMethod.Description,
            IsActive = paymentMethod.IsActive
        };
    }
}

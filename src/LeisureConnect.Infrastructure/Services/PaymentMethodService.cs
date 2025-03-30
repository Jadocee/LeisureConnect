using LeisureConnect.Core.DTOs;
using LeisureConnect.Core.Entities;
using LeisureConnect.Core.Interfaces.Repositories;
using LeisureConnect.Core.Interfaces.Services;
using LeisureConnect.Infrastructure.Data.Context;
using LeisureConnect.Infrastructure.Extensions;
using LeisureConnect.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LeisureConnect.Infrastructure.Services;

public class PaymentMethodService : IPaymentMethodService
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public PaymentMethodService(IDbContextFactory<LeisureAustralasiaDbContext> dbContextFactory)
    {
        LeisureAustralasiaDbContext context = dbContextFactory.CreateDbContext();
        _paymentMethodRepository = new PaymentMethodRepository(context);
    }

    public async Task<List<PaymentMethodDto>> GetAllPaymentMethodsAsync(CancellationToken cancellationToken = default)
    {
        List<PaymentMethod> paymentMethods = await _paymentMethodRepository.GetAllPaymentMethodsAsync(cancellationToken);
        return paymentMethods.Select(x => x.ToDto()).ToList();
    }
}
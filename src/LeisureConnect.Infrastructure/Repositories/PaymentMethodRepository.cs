using LeisureConnect.Core.Entities;
using LeisureConnect.Core.Interfaces.Repositories;
using LeisureConnect.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LeisureConnect.Infrastructure.Repositories;

public class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly LeisureAustralasiaDbContext _context;

    public PaymentMethodRepository(LeisureAustralasiaDbContext context)
    {
        _context = context;
    }

    public async Task<List<PaymentMethod>> GetAllPaymentMethodsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PaymentMethods
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
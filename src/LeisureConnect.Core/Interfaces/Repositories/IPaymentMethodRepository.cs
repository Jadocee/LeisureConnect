using LeisureConnect.Core.Entities;

namespace LeisureConnect.Core.Interfaces.Repositories;

public interface IPaymentMethodRepository
{
    Task<List<PaymentMethod>> GetAllPaymentMethodsAsync(CancellationToken cancellationToken = default);
}
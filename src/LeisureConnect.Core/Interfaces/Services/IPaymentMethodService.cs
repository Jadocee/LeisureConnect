using LeisureConnect.Core.DTOs;

namespace LeisureConnect.Core.Interfaces.Services;

public interface IPaymentMethodService
{
    Task<List<PaymentMethodDto>> GetAllPaymentMethodsAsync(CancellationToken cancellationToken = default);
}
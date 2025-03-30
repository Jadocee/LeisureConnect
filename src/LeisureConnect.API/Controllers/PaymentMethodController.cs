using LeisureConnect.Core.DTOs;
using LeisureConnect.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeisureConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentMethodController : ControllerBase
{
    private readonly IPaymentMethodService _paymentMethodService;

    public PaymentMethodController(IPaymentMethodService paymentMethodService)
    {
        _paymentMethodService = paymentMethodService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<PaymentMethodDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllPaymentMethods()
    {
        try
        {
            List<PaymentMethodDto> paymentMethods = await _paymentMethodService.GetAllPaymentMethodsAsync();
            return Ok(paymentMethods);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails()
            {
                Title = "Internal Server Error",
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}
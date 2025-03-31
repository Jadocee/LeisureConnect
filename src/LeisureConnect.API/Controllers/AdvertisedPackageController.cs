using LeisureConnect.Core.DTOs;
using LeisureConnect.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeisureConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdvertisedPackageController : ControllerBase
{
    private readonly IAdvertisedPackageService _advertisedPackageService;
    private readonly ILogger<AdvertisedPackageController> _logger;

    public AdvertisedPackageController(IAdvertisedPackageService advertisedPackageService, ILogger<AdvertisedPackageController> logger)
    {
        _advertisedPackageService = advertisedPackageService;
        _logger = logger;
    }

    [HttpGet("available")]
    [ProducesResponseType(typeof(List<AdvertisedPackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailablePackages(
        [FromQuery] int hotelId,
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate)
    {
        _logger.LogInformation("GetAvailablePackages called with hotelId: {hotelId}, startDate: {startDate}, endDate: {endDate}", hotelId, startDate, endDate);

        if (hotelId <= 0)
        {
            return BadRequest(new ProblemDetails()
            {
                Title = "Invalid Hotel ID",
                Detail = "Hotel ID must be a positive integer.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (endDate < startDate)
        {
            return BadRequest(new ProblemDetails()
            {
                Title = "Invalid Date Range",
                Detail = "End date cannot be earlier than start date",
                Status = StatusCodes.Status400BadRequest
            });
        }

        try
        {
            List<AdvertisedPackageDto> packages = await _advertisedPackageService.GetAvailablePackagesAsync(hotelId, startDate, endDate);
            return Ok(packages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching available packages for hotelId: {hotelId}, startDate: {startDate}, endDate: {endDate}", hotelId, startDate, endDate);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails()
            {
                Title = "Internal Server Error",
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AdvertisedPackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAdvertisedPackage(int id)
    {
        _logger.LogInformation("GetAdvertisedPackage called with ID: {id}", id);

        if (id <= 0)
        {
            return BadRequest(new ProblemDetails()
            {
                Title = "Invalid Package ID",
                Detail = "Package ID must be a positive integer.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        try
        {
            AdvertisedPackageDto? advertisedPackage = await _advertisedPackageService.GetAdvertisedPackageByIdAsync(id);
            if (advertisedPackage == null)
            {
                return NotFound(new ProblemDetails()
                {
                    Title = "Advertised Package Not Found",
                    Detail = $"No advertised package found with ID {id}.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(advertisedPackage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching advertised package with ID: {id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails()
            {
                Title = "Internal Server Error",
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}
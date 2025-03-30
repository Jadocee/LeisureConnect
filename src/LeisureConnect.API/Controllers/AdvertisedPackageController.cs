using LeisureConnect.Core.DTOs;
using LeisureConnect.Core.Entities;
using LeisureConnect.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeisureConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdvertisedPackageController : ControllerBase
{
    private readonly IAdvertisedPackageService _advertisedPackageService;

    public AdvertisedPackageController(IAdvertisedPackageService advertisedPackageService)
    {
        _advertisedPackageService = advertisedPackageService;
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
        try
        {
            if (endDate < startDate)
            {
                return BadRequest(new ProblemDetails()
                {
                    Title = "Invalid Date Range",
                    Detail = "End date cannot be earlier than start date",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            List<AdvertisedPackageDto> packages = await _advertisedPackageService.GetAvailablePackagesAsync(hotelId, startDate, endDate);

            return Ok(packages);
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

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AdvertisedPackageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAdvertisedPackage(int id)
    {
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
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails()
            {
                Title = "Internal Server Error",
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}
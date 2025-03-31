using LeisureConnect.Core.DTOs;
using LeisureConnect.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeisureConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly ILogger<HotelController> _logger;

    public HotelController(IHotelService hotelService, ILogger<HotelController> logger)
    {
        _logger = logger;
        _hotelService = hotelService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<HotelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllHotels()
    {
        _logger.LogInformation("GetAllHotels called");

        try
        {
            List<HotelDto> hotels = await _hotelService.GetAllHotelsAsync();

            return Ok(hotels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching all hotels");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails()
            {
                Title = "Internal Server Error",
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HotelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHotelById(int id)
    {
        _logger.LogInformation("GetHotelById called with id: {id}", id);

        if (id <= 0)
        {
            return BadRequest(new ProblemDetails()
            {
                Title = "Invalid Hotel ID",
                Detail = "Hotel ID must be a positive integer.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        try
        {
            HotelDto? hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null)
            {
                return NotFound(new ProblemDetails()
                {
                    Title = "Hotel Not Found",
                    Detail = $"Hotel with ID {id} not found.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(hotel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching hotel with ID: {id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails()
            {
                Title = "Internal Server Error",
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}
using LeisureConnect.Core.DTOs;
using LeisureConnect.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LeisureConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            return BadRequest(new ProblemDetails()
            {
                Title = "Invalid Reservation Request",
                Detail = "Request body cannot be null.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        try
        {
            ReservationResponse? result = await _reservationService.CreateReservationAsync(request, cancellationToken);
            if (result == null)
            {
                return NotFound(new ProblemDetails()
                {
                    Title = "Reservation Not Found",
                    Detail = "The reservation could not be created.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return CreatedAtAction(nameof(GetReservation), new { id = result.ReservationId }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails()
            {
                Title = "Invalid Reservation Request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
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
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetReservation(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new ProblemDetails()
            {
                Title = "Invalid Reservation ID",
                Detail = "Reservation ID must be greater than zero.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        try
        {
            ReservationResponse? reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound(new ProblemDetails()
                {
                    Title = "Reservation Not Found",
                    Detail = $"No reservation found with ID {id}.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(reservation);
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
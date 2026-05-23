using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/screenings")]
public sealed class ScreeningsController : ControllerBase
{
    private readonly IScreeningService _screeningService;
    private readonly IBookingService _bookingService;

    public ScreeningsController(IScreeningService screeningService, IBookingService bookingService)
    {
        _screeningService = screeningService;
        _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetScreenings([FromQuery] int? eventId, [FromQuery] int? movieId)
    {
        if (eventId.HasValue)
        {
            var screenings = await _screeningService.GetScreeningsByEventAsync(eventId.Value);
            return Ok(screenings);
        }
        
        if (movieId.HasValue)
        {
            var screenings = await _screeningService.GetScreeningsByMovieAsync(movieId.Value);
            return Ok(screenings);
        }

        return BadRequest("Must provide either eventId or movieId.");
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetScreeningById(int id)
    {
        var screening = await _screeningService.GetScreeningAsync(id);
        if (screening == null)
        {
            return NotFound();
        }
        return Ok(screening);
    }

    [HttpGet("{id:int}/seats")]
    public async Task<IActionResult> GetSeats(int id)
    {
        var seats = await _screeningService.GetAvailableSeatsAsync(id);
        return Ok(seats);
    }

    [HttpPost("{id:int}/book")]
    public async Task<IActionResult> BookSeats(int id, [FromBody] BookSeatsRequestBody body)
    {
        var seatsToBook = body.Seats.Select(s => (s.Row, s.Column)).ToList();
        var success = await _bookingService.BookSeatsAsync(id, body.UserId, seatsToBook);
        
        if (!success)
        {
            return Conflict("One or more seats are already taken.");
        }
        
        return Ok();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
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

        var allScreenings = await _screeningService.GetAllScreeningsAsync();
        return Ok(allScreenings);
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

    [HttpGet("{id:int}/details")]
    public async Task<IActionResult> GetDetails(int id)
    {
        var details = await _screeningService.GetScreeningDetailsAsync(id);
        if (details is null)
        {
            return NotFound();
        }

        return Ok(details);
    }

    [HttpPost]
    public async Task<IActionResult> CreateScreening([FromBody] MovieApp.WebDTOs.DTOs.RequestDTOs.CreateScreeningRequestBody body)
    {
        try
        {
            var screening = new MovieApp.DataLayer.Models.Screening
            {
                Id = 0,
                EventId = body.EventId,
                MovieId = body.MovieId,
                RoomId = body.RoomId,
                ScreeningTime = body.ScreeningTime,
                TicketPrice = body.TicketPrice,
            };
            await _screeningService.AddScreeningAsync(screening);
            return Ok(screening);
        }
        catch (System.InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
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

    [HttpGet("{id:int}/bookings")]
    public async Task<IActionResult> GetBookings(int id)
    {
        var bookings = await _bookingService.GetBookingsForScreeningAsync(id);
        return Ok(bookings);
    }

    [HttpGet("bookings/user/{userId:int}")]
    public async Task<IActionResult> GetBookingsForUser(int userId)
    {
        var bookings = await _bookingService.GetBookingsForUserAsync(userId);
        return Ok(bookings);
    }

    [HttpPost("bookings/{bookingId:int}/cancel")]
    public async Task<IActionResult> CancelBooking(int bookingId, [FromBody] CancelBookingRequestBody body)
    {
        var success = await _bookingService.CancelBookingAsync(bookingId, body.UserId);
        if (!success)
        {
            return NotFound("Booking not found or does not belong to the user.");
        }

        return Ok();
    }
}

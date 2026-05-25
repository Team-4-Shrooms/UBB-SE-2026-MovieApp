namespace MovieApp.Web.Controllers;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Web.Models;
using MovieApp.WebDTOs.DTOs;

public sealed class ScreeningsController : Controller
{
    private const int DefaultEventId = 1;
    private const int DefaultRoomId = 1;

    private readonly IScreeningService _screeningService;
    private readonly IBookingService _bookingService;
    private readonly IMovieService _movieService;
    private readonly ICurrentUserService _currentUserService;

    public ScreeningsController(
        IScreeningService screeningService,
        IBookingService bookingService,
        IMovieService movieService,
        ICurrentUserService currentUserService)
    {
        _screeningService = screeningService;
        _bookingService = bookingService;
        _movieService = movieService;
        _currentUserService = currentUserService;
    }

    public async Task<IActionResult> Index(int? eventId, int? movieId)
    {
        IReadOnlyList<MovieApp.DataLayer.Models.Screening> screenings;

        if (eventId.HasValue)
        {
            screenings = await _screeningService.GetScreeningsByEventAsync(eventId.Value);
            ViewBag.EventId = eventId.Value;
        }
        else if (movieId.HasValue)
        {
            screenings = await _screeningService.GetScreeningsByMovieAsync(movieId.Value);
            ViewBag.MovieId = movieId.Value;
        }
        else
        {
            return BadRequest("Provide eventId or movieId.");
        }

        var cards = new List<ScreeningDetailsDto>();
        foreach (var screening in screenings)
        {
            var details = await _screeningService.GetScreeningDetailsAsync(screening.Id);
            if (details != null)
            {
                cards.Add(details);
            }
        }

        return View(cards);
    }

    public async Task<IActionResult> MyBookings()
    {
        int userId = _currentUserService.UserId;
        var bookings = await _bookingService.GetBookingsForUserAsync(userId);

        var grouped = bookings
            .GroupBy(b => b.ScreeningId)
            .ToList();

        var result = new List<MyBookingGroupViewModel>();
        foreach (var group in grouped)
        {
            var details = await _screeningService.GetScreeningDetailsAsync(group.Key);
            if (details is null)
            {
                continue;
            }

            result.Add(new MyBookingGroupViewModel
            {
                Details = details,
                Bookings = group.OrderBy(b => b.Row).ThenBy(b => b.Column).ToList(),
            });
        }

        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelBooking(int bookingId)
    {
        var success = await _bookingService.CancelBookingAsync(bookingId, _currentUserService.UserId);
        TempData[success ? "Success" : "Error"] = success
            ? "Booking cancelled."
            : "Could not cancel booking.";
        return RedirectToAction(nameof(MyBookings));
    }

    public async Task<IActionResult> Detail(int id)
    {
        var details = await _screeningService.GetScreeningDetailsAsync(id);
        if (details is null)
        {
            return NotFound();
        }

        return View(new ScreeningDetailViewModel { Details = details });
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var movies = await _movieService.GetAllMoviesAsync();
        ViewBag.Movies = movies;
        ViewBag.DefaultEventId = DefaultEventId;
        ViewBag.DefaultRoomId = DefaultRoomId;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int eventId, int movieId, int roomId, System.DateTime screeningTime)
    {
        try
        {
            var screening = new MovieApp.DataLayer.Models.Screening
            {
                Id = 0,
                EventId = eventId,
                MovieId = movieId,
                RoomId = roomId,
                ScreeningTime = screeningTime,
            };
            await _screeningService.AddScreeningAsync(screening);
            TempData["Success"] = "Screening created.";
            return RedirectToAction(nameof(Index), new { eventId });
        }
        catch (System.Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Create));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(int id, string? seats)
    {
        var parsed = ParseSeats(seats);
        if (parsed.Count == 0)
        {
            TempData["Error"] = "Please select at least one seat.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        var success = await _bookingService.BookSeatsAsync(id, _currentUserService.UserId, parsed);
        if (!success)
        {
            TempData["Error"] = "One or more selected seats are no longer available. Please try again.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        TempData["Success"] = $"Booked {parsed.Count} seat(s) successfully.";
        return RedirectToAction(nameof(MyBookings));
    }

    private static List<(int Row, int Column)> ParseSeats(string? raw)
    {
        var result = new List<(int Row, int Column)>();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return result;
        }

        foreach (var token in raw.Split(';', System.StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = token.Split(',');
            if (parts.Length == 2
                && int.TryParse(parts[0], out int row)
                && int.TryParse(parts[1], out int column))
            {
                result.Add((row, column));
            }
        }

        return result;
    }
}

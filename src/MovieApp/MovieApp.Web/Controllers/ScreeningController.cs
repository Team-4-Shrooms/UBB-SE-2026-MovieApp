namespace MovieApp.Web.Controllers;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Web.Models;

public sealed class ScreeningController : Controller
{
    private readonly IScreeningService _screeningService;
    private readonly IBookingService _bookingService;
    private readonly ICurrentUserService _currentUserService;

    public ScreeningController(
        IScreeningService screeningService,
        IBookingService bookingService,
        ICurrentUserService currentUserService)
    {
        _screeningService = screeningService;
        _bookingService = bookingService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> ForMovie(int id)
    {
        var screenings = await _screeningService.GetScreeningsByMovieAsync(id);

        var viewModel = new ScreeningListViewModel { MovieId = id };

        foreach (var screening in screenings)
        {
            var details = await _screeningService.GetScreeningDetailsAsync(screening.Id);
            if (details is null)
            {
                continue;
            }

            if (viewModel.MovieTitle.Length == 0)
            {
                viewModel.MovieTitle = details.MovieTitle;
                viewModel.MoviePosterUrl = details.MoviePosterUrl;
            }

            viewModel.Screenings.Add(details);
        }

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Checkout(int id)
    {
        var details = await _screeningService.GetScreeningDetailsAsync(id);
        if (details is null)
        {
            return NotFound();
        }

        var seats = details.Seats
            .Select(s => new SeatViewModel { Row = s.Row, Col = s.Column, IsAvailable = s.IsAvailable })
            .ToList();

        return View(new CheckoutViewModel { Screening = details, Seats = seats });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve(int screeningId, string selectedSeats)
    {
        var parsed = ParseSeats(selectedSeats);
        if (parsed.Count == 0)
        {
            TempData["Error"] = "Please select at least one seat.";
            return RedirectToAction(nameof(Checkout), new { id = screeningId });
        }

        bool success;
        try
        {
            success = await _bookingService.BookSeatsAsync(screeningId, _currentUserService.UserId, parsed);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Checkout), new { id = screeningId });
        }
        if (!success)
        {
            TempData["Error"] = "One or more selected seats are no longer available. Please try again.";
            return RedirectToAction(nameof(Checkout), new { id = screeningId });
        }

        TempData["ConfirmedCount"] = parsed.Count;
        return RedirectToAction(nameof(Confirmation), new { screeningId });
    }

    [HttpGet]
    public async Task<IActionResult> Confirmation(int screeningId)
    {
        var details = await _screeningService.GetScreeningDetailsAsync(screeningId);
        if (details is null)
        {
            return NotFound();
        }

        return View(details);
    }

    private static List<(int Row, int Column)> ParseSeats(string? raw)
    {
        var result = new List<(int Row, int Column)>();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return result;
        }

        foreach (var token in raw.Split(',', System.StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = token.Split('-');
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

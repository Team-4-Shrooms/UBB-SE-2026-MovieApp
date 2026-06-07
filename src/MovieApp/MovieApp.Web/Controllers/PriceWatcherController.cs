using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Web.Controllers;

[Microsoft.AspNetCore.Authorization.Authorize]
public sealed class PriceWatcherController : Controller
{
    private readonly IPriceWatcherService _priceWatcherService;
    private readonly IEventService _eventService;

    public PriceWatcherController(IPriceWatcherService priceWatcherService, IEventService eventService)
    {
        _priceWatcherService = priceWatcherService;
        _eventService = eventService;
    }

    public async Task<IActionResult> MyWatchers()
    {
        var watchers = await _priceWatcherService.GetAllWatchedEventsAsync();
        return View(watchers);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Watch(int eventId)
    {
        try
        {
            var movieEvent = await _eventService.GetEventByIdAsync(eventId)
                ?? throw new Exception("Event not found.");

            await _priceWatcherService.AddWatchAsync(new PriceWatcher
            {
                EventId = eventId,
                EventTitle = movieEvent.Title,
                TargetPrice = movieEvent.TicketPrice,
            });

            TempData["Success"] = "Event added to your watchlist.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Detail", "Events", new { id = eventId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unwatch(int eventId)
    {
        await _priceWatcherService.RemoveWatchAsync(eventId);
        TempData["Success"] = "Removed from your watchlist.";
        return RedirectToAction(nameof(MyWatchers));
    }
}


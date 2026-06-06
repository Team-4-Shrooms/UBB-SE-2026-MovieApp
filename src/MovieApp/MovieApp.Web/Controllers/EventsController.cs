namespace MovieApp.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MovieApp.Logic.Interfaces.Services;
    using MovieApp.DataLayer.Models;
    using System.Threading.Tasks;
    using System.Linq;

    public sealed class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPriceWatcherService _priceWatcherService;

        public EventsController(IEventService eventService, ICurrentUserService currentUserService, IPriceWatcherService priceWatcherService)
        {
            _eventService = eventService;
            _currentUserService = currentUserService;
            _priceWatcherService = priceWatcherService;
        }

        public async Task<IActionResult> Index(string? search, int? movieId)
        {
            var events = movieId.HasValue
                ? await _eventService.GetEventsByMovieIdAsync(movieId.Value)
                : await _eventService.GetAvailableEventsAsync();

            if (!string.IsNullOrEmpty(search))
            {
                events = events.Where(e => e.Title.Contains(search, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.MovieId = movieId;
            return View(events);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var movieEvent = await _eventService.GetEventByIdAsync(id);
            if (movieEvent == null) return NotFound();

            var userId = _currentUserService.UserId;
            ViewBag.HasTicket = await _eventService.UserHasTicketAsync(userId, id);
            ViewBag.IsWatching = await _priceWatcherService.IsWatchingAsync(id);

            return View(movieEvent);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyTicket(int eventId)
        {
            try
            {
                await _eventService.PurchaseTicketAsync(_currentUserService.UserId, eventId);
                return RedirectToAction("Index", "Inventory");
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Detail", new { id = eventId });
            }
        }
    }
}

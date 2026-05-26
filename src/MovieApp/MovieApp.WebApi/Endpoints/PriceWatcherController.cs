using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.WebApi.Endpoints
{
    [Authorize]
    [ApiController]
    [Route("api/price-watchers")]
    public sealed class PriceWatcherController : ControllerBase
    {
        private readonly IPriceWatcherService _priceWatcherService;

        public PriceWatcherController(IPriceWatcherService priceWatcherService)
        {
            _priceWatcherService = priceWatcherService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllWatchedEvents()
        {
            List<PriceWatcher> watchedEvents = await _priceWatcherService.GetAllWatchedEventsAsync();
            return Ok(watchedEvents);
        }

        [HttpGet("{eventId:int}")]
        public async Task<IActionResult> GetWatchedEvent(int eventId)
        {
            PriceWatcher? watchedEvent = await _priceWatcherService.GetWatchAsync(eventId);
            if (watchedEvent == null)
            {
                return NotFound();
            }

            return Ok(watchedEvent);
        }

        [HttpGet("check/{eventId:int}")]
        public async Task<IActionResult> IsWatching(int eventId)
        {
            bool isWatching = await _priceWatcherService.IsWatchingAsync(eventId);
            return Ok(isWatching);
        }

        [HttpPost("")]
        public async Task<IActionResult> AddWatch([FromBody] PriceWatcher watchedEvent)
        {
            bool added = await _priceWatcherService.AddWatchAsync(watchedEvent);
            return Ok(added);
        }

        [HttpDelete("{eventId:int}")]
        public async Task<IActionResult> RemoveWatch(int eventId)
        {
            await _priceWatcherService.RemoveWatchAsync(eventId);
            return NoContent();
        }
    }
}

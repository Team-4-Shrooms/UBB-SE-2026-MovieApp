namespace MovieApp.WebApi.Endpoints
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MovieApp.DataLayer.Interfaces.Repositories;
    using MovieApp.DataLayer.Models;

    [Authorize]
    [ApiController]
    [Route("api/PriceWatcher")]
    public sealed class PriceWatcherController : ControllerBase
    {
        private readonly IPriceWatcherRepository _priceWatcherRepository;

        public PriceWatcherController(IPriceWatcherRepository priceWatcherRepository)
        {
            _priceWatcherRepository = priceWatcherRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllWatchedEvents()
        {
            List<PriceWatcher> watchedEvents = await _priceWatcherRepository.GetAllWatchedEventsAsync();
            return Ok(watchedEvents);
        }

        [HttpGet("{eventId:int}")]
        public async Task<IActionResult> GetWatchedEvent(int eventId)
        {
            PriceWatcher? watchedEvent = await _priceWatcherRepository.GetWatchAsync(eventId);
            if (watchedEvent == null)
            {
                return NotFound();
            }

            return Ok(watchedEvent);
        }

        [HttpGet("check/{eventId:int}")]
        public async Task<IActionResult> IsWatching(int eventId)
        {
            bool isWatching = await _priceWatcherRepository.IsWatchingAsync(eventId);
            return Ok(isWatching);
        }

        [HttpPost("")]
        public async Task<IActionResult> AddWatch([FromBody] PriceWatcher watchedEvent)
        {
            bool added = await _priceWatcherRepository.AddWatchAsync(watchedEvent);
            return Ok(added);
        }

        [HttpDelete("{eventId:int}")]
        public async Task<IActionResult> RemoveWatch(int eventId)
        {
            await _priceWatcherRepository.RemoveWatchAsync(eventId);
            return NoContent();
        }
    }
}

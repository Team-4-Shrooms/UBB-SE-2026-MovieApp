using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.WebApi.Endpoints
{
    [Authorize]
    [ApiController]
    [Route("api/stats")]
    public sealed class StatsController : ControllerBase
    {
        private readonly IUserStatsService _userStatsService;

        public StatsController(IUserStatsService userStatsService)
        {
            _userStatsService = userStatsService;
        }

        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetUserStats(int userId, CancellationToken ct = default)
        {
            UserStats? stats = await _userStatsService.GetByUserIdAsync(userId, ct);
            if (stats == null)
            {
                return NotFound();
            }

            return Ok(stats);
        }
    }
}

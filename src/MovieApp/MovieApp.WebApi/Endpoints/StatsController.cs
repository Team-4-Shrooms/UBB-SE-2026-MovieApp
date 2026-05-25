namespace MovieApp.WebApi.Endpoints
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MovieApp.Logic.Interfaces.Services;

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
            DataLayer.Models.UserStats? stats = await _userStatsService.GetByUserIdAsync(userId, ct);
            if (stats == null)
            {
                return NotFound();
            }

            return Ok(stats);
        }
    }
}

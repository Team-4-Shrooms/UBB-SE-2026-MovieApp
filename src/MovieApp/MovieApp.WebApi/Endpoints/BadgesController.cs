using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.WebApi.Endpoints
{
    [Authorize]
    [ApiController]
    [Route("api/badges")]
    public sealed class BadgesController : ControllerBase
    {
        private readonly IBadgeService _badgeService;

        public BadgesController(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllBadges(CancellationToken ct = default)
        {
            var badges = await _badgeService.GetAllBadgesAsync(ct);
            return Ok(badges);
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard(CancellationToken ct = default)
        {
            var leaderboard = await _badgeService.GetLeaderboardAsync(ct);
            return Ok(leaderboard);
        }

        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetUserBadges(int userId, CancellationToken ct = default)
        {
            var userBadges = await _badgeService.GetUserBadgesAsync(userId, ct);
            return Ok(userBadges);
        }
    }
}

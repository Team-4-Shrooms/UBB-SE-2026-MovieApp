using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebApi.Mappings;

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
        public async Task<IActionResult> GetAllBadges(CancellationToken cancellationToken = default)
        {
            List<Badge> badges = await _badgeService.GetAllBadgesAsync(cancellationToken);
            return Ok(badges.Select(badge => badge.ToDto()).ToList());
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard(CancellationToken cancellationToken = default)
        {
            IList<UserStats> leaderboard = await _badgeService.GetLeaderboardAsync(cancellationToken);
            return Ok(leaderboard.Select(entry => entry.ToDto()).ToList());
        }

        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetUserBadges(int userId, CancellationToken cancellationToken = default)
        {
            List<UserBadge> userBadges = await _badgeService.GetUserBadgesAsync(userId, cancellationToken);
            return Ok(userBadges.Select(userBadge => userBadge.ToDto()).ToList());
        }

        [HttpPost("{userId:int}/award")]
        public async Task<IActionResult> CheckAndAwardBadges(int userId, CancellationToken cancellationToken = default)
        {
            await _badgeService.CheckAndAwardBadgesAsync(userId, cancellationToken);
            return NoContent();
        }
    }
}

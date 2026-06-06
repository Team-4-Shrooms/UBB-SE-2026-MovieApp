using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Web.Models;

namespace MovieApp.Web.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
public class ProfileController : Controller
    {
        private readonly IBadgeService _badgeService;
        private readonly IUserStatsService _userStatsService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;

        public ProfileController(
            IBadgeService badgeService,
            IUserStatsService userStatsService,
            ICurrentUserService currentUserService,
            IUserService userService)
        {
            _badgeService = badgeService;
            _userStatsService = userStatsService;
            _currentUserService = currentUserService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            int userId = _currentUserService.UserId;

            // Evaluate and award any newly earned badges before reading the results.
            await _badgeService.CheckAndAwardBadgesAsync(userId, ct);

            // Parallel calls â€” no sequential waiting
            Task<UserStats?> statsTask = _userStatsService.GetByUserIdAsync(userId, ct);
            Task<List<UserBadge>> earnedTask = _badgeService.GetUserBadgesAsync(userId, ct);
            Task<List<Badge>> allTask = _badgeService.GetAllBadgesAsync(ct);
            Task<User?> userTask = _userService.GetUserByIdAsync(userId);

            await Task.WhenAll(statsTask, earnedTask, allTask, userTask);

            List<Badge> earnedBadges = earnedTask.Result
                .Where(static ub => ub.Badge is not null)
                .Select(static ub => ub.Badge!)
                .ToList();

            ProfileViewModel vm = new ProfileViewModel
            {
                Username = userTask.Result?.Username ?? string.Empty,
                Stats = statsTask.Result,
                EarnedBadges = earnedBadges,
                AllBadges = allTask.Result,
            };

            return View(vm);
        }
    }
}


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services
{
    public sealed class BadgeService : IBadgeService
    {
        private readonly IBadgeRepository _badgeRepository;
        private readonly IUserStatsRepository _userStatsRepository;

        public BadgeService(IBadgeRepository badgeRepository, IUserStatsRepository userStatsRepository)
        {
            _badgeRepository = badgeRepository;
            _userStatsRepository = userStatsRepository;
        }

        public Task<List<UserBadge>> GetUserBadgesAsync(int userId, CancellationToken ct = default)
        {
            return Task.FromResult(new List<UserBadge>());
        }

        public Task<List<Badge>> GetAllBadgesAsync(CancellationToken ct = default)
        {
            return _badgeRepository.GetAllAsync(ct);
        }

        public async Task<IList<UserStats>> GetLeaderboardAsync(CancellationToken ct = default)
        {
            return await _userStatsRepository.GetLeaderboardAsync(ct);
        }

        public Task CheckAndAwardBadgesAsync(int userId, CancellationToken ct = default)
        {
            // Server-side stub: badge evaluation logic to be implemented in a future ticket.
            return Task.CompletedTask;
        }
    }
}

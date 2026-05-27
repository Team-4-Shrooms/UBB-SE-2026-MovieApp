using System;
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

        public Task<List<UserBadge>> GetUserBadgesAsync(int userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new List<UserBadge>());
        }

        public Task<List<Badge>> GetAllBadgesAsync(CancellationToken cancellationToken = default)
        {
            return _badgeRepository.GetAllAsync(cancellationToken);
        }

        public async Task<IList<UserStats>> GetLeaderboardAsync(CancellationToken cancellationToken = default)
        {
            return await _userStatsRepository.GetLeaderboardAsync(cancellationToken);
        }

        public Task CheckAndAwardBadgesAsync(int userId, CancellationToken cancellationToken = default)
        {
            // Server-side stub: badge evaluation logic to be implemented in a future ticket.
            return Task.CompletedTask;
        }
    }
}

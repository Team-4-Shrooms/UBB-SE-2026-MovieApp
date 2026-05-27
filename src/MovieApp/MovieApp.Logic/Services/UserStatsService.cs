using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services
{
    public sealed class UserStatsService : IUserStatsService
    {
        private readonly IUserStatsRepository _userStatsRepository;

        public UserStatsService(IUserStatsRepository userStatsRepository)
        {
            _userStatsRepository = userStatsRepository;
        }

        public async Task<UserStats?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _userStatsRepository.GetByUserIdAsync(userId, ct);
        }

        public async Task<List<UserStats>> GetAllAsync(CancellationToken ct = default)
        {
            return await _userStatsRepository.GetAllAsync(ct);
        }

        public async Task<int> InsertAsync(UserStats userStats, CancellationToken ct = default)
        {
            return await _userStatsRepository.InsertAsync(userStats, ct);
        }

        public async Task<bool> UpdateAsync(UserStats userStats, CancellationToken ct = default)
        {
            return await _userStatsRepository.UpdateAsync(userStats, ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            return await _userStatsRepository.DeleteAsync(id, ct);
        }

        /// <summary>
        /// Returns all user stats ordered by TotalPoints descending.
        /// Uses a single OrderByDescending — no N+1 queries.
        /// </summary>
        public async Task<IList<UserStats>> GetLeaderboardAsync(CancellationToken ct = default)
        {
            return await _userStatsRepository.GetLeaderboardAsync(ct);
        }
    }
}

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

        public Task<UserStats?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return _userStatsRepository.GetByUserIdAsync(userId, ct);
        }
    }
}

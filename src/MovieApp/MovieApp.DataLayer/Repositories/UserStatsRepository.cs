using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class UserStatsRepository : IUserStatsRepository
    {
        public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserStats>> GetAllAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserStats?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserStats?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserStats>> GetLeaderboardAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertAsync(UserStats userStats, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(UserStats userStats, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}

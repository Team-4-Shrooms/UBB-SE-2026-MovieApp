using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class BadgeRepository : IBadgeRepository
    {
        public Task<List<Badge>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new List<Badge>());
        }

        public Task<Badge?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Badge?>(null);
        }

        public Task<int> InsertAsync(Badge badge, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        public Task<bool> UpdateAsync(Badge badge, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<List<Badge>> GetBadgesForUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new List<Badge>());
        }
    }
}

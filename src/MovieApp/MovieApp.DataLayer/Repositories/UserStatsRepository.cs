using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class UserStatsRepository : IUserStatsRepository
    {
        private readonly IMovieAppDbContext _context;

        public UserStatsRepository(IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserStats>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.UserStats
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<UserStats?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.UserStats
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.UserStatsId == id, ct);
        }

        public async Task<UserStats?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _context.UserStats
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.UserId == userId, ct);
        }

        public async Task<int> InsertAsync(UserStats userStats, CancellationToken ct = default)
        {
            await _context.UserStats.AddAsync(userStats, ct);
            await _context.SaveChangesAsync(ct);
            return userStats.UserStatsId;
        }

        public async Task<bool> UpdateAsync(UserStats userStats, CancellationToken ct = default)
        {
            _context.UserStats.Update(userStats);
            int rows = await _context.SaveChangesAsync(ct);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            UserStats? userStats = await _context.UserStats
                .FirstOrDefaultAsync(us => us.UserStatsId == id, ct);
            if (userStats is null)
            {
                return false;
            }

            _context.UserStats.Remove(userStats);
            int rows = await _context.SaveChangesAsync(ct);
            return rows > 0;
        }

        public async Task<IList<UserStats>> GetLeaderboardAsync(CancellationToken ct = default)
        {
            return await _context.UserStats
                .AsNoTracking()
                .OrderByDescending(us => us.TotalPoints)
                .ToListAsync(ct);
        }
    }
}

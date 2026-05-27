using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class UserBadgeRepository : IUserBadgeRepository
    {
        private readonly IMovieAppDbContext _context;

        public UserBadgeRepository(IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserBadge>> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _context.UserBadges
                .AsNoTracking()
                .Include(ub => ub.Badge)
                .Where(ub => ub.User != null && ub.User.Id == userId)
                .ToListAsync(ct);
        }

        public async Task<bool> ExistsAsync(int userId, int badgeId, CancellationToken ct = default)
        {
            return await _context.UserBadges
                .AnyAsync(
                    ub => ub.User != null && ub.User.Id == userId
                        && ub.Badge != null && ub.Badge.BadgeId == badgeId,
                    ct);
        }

        public async Task<int> InsertAsync(UserBadge userBadge, CancellationToken ct = default)
        {
            // Load tracked references so EF does not attempt to re-insert existing entities.
            if (userBadge.User is not null)
            {
                userBadge.User = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userBadge.User.Id, ct);
            }

            if (userBadge.Badge is not null)
            {
                userBadge.Badge = await _context.Badges
                    .FirstOrDefaultAsync(b => b.BadgeId == userBadge.Badge.BadgeId, ct);
            }

            await _context.UserBadges.AddAsync(userBadge, ct);
            await _context.SaveChangesAsync(ct);
            return userBadge.UserBadgeId;
        }

        public async Task<bool> DeleteAsync(int userBadgeId, CancellationToken ct = default)
        {
            UserBadge? userBadge = await _context.UserBadges
                .FirstOrDefaultAsync(ub => ub.UserBadgeId == userBadgeId, ct);
            if (userBadge is null)
            {
                return false;
            }

            _context.UserBadges.Remove(userBadge);
            int rows = await _context.SaveChangesAsync(ct);
            return rows > 0;
        }
    }
}

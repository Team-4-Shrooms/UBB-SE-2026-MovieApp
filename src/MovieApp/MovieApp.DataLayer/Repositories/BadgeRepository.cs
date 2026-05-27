using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class BadgeRepository : IBadgeRepository
    {
        private readonly IMovieAppDbContext _context;

        public BadgeRepository(IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Badge>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Badges
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<Badge?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Badges
                .AsNoTracking()
                .FirstOrDefaultAsync(badge => badge.BadgeId == id, ct);
        }

        public async Task<int> InsertAsync(Badge badge, CancellationToken ct = default)
        {
            await _context.Badges.AddAsync(badge, ct);
            await _context.SaveChangesAsync(ct);
            return badge.BadgeId;
        }

        public async Task<bool> UpdateAsync(Badge badge, CancellationToken ct = default)
        {
            _context.Badges.Update(badge);
            int rows = await _context.SaveChangesAsync(ct);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            Badge? badge = await _context.Badges
                .FirstOrDefaultAsync(b => b.BadgeId == id, ct);
            if (badge is null)
            {
                return false;
            }

            _context.Badges.Remove(badge);
            int rows = await _context.SaveChangesAsync(ct);
            return rows > 0;
        }

        public async Task<List<Badge>> GetBadgesForUserAsync(int userId, CancellationToken ct = default)
        {
            return await _context.UserBadges
                .AsNoTracking()
                .Include(ub => ub.Badge)
                .Where(ub => ub.User != null && ub.User.Id == userId && ub.Badge != null)
                .Select(ub => ub.Badge!)
                .ToListAsync(ct);
        }
    }
}

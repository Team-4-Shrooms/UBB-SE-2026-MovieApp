using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class FavoriteEventRepository : IFavoriteEventRepository
    {
        private readonly IMovieAppDbContext _context;

        public FavoriteEventRepository(IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(int userId, int eventId, CancellationToken cancellationToken = default)
        {
            FavoriteEvent favoriteEvent = new FavoriteEvent
            {
                UserId = userId,
                EventId = eventId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.FavoriteEvents.AddAsync(favoriteEvent, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveAsync(int userId, int eventId, CancellationToken cancellationToken = default)
        {
            FavoriteEvent? favoriteEvent = await _context.FavoriteEvents
                .FirstOrDefaultAsync(entry => entry.UserId == userId && entry.EventId == eventId, cancellationToken);

            if (favoriteEvent != null)
            {
                _context.FavoriteEvents.Remove(favoriteEvent);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IReadOnlyList<FavoriteEvent>> FindByUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            List<FavoriteEvent> favorites = await _context.FavoriteEvents
                .Where(entry => entry.UserId == userId)
                .ToListAsync(cancellationToken);
            return favorites;
        }

        public async Task<bool> ExistsAsync(int userId, int eventId, CancellationToken cancellationToken = default)
        {
            bool exists = await _context.FavoriteEvents
                .AnyAsync(entry => entry.UserId == userId && entry.EventId == eventId, cancellationToken);
            return exists;
        }

        public async Task<IReadOnlyList<int>> GetUsersByFavoriteEventAsync(int eventId, CancellationToken cancellationToken = default)
        {
            List<int> userIds = await _context.FavoriteEvents
                .Where(entry => entry.EventId == eventId)
                .Select(entry => entry.UserId)
                .ToListAsync(cancellationToken);
            return userIds;
        }

        public async Task<IReadOnlyList<FavoriteEvent>> FindByEventAsync(int eventId, CancellationToken cancellationToken = default)
        {
            List<FavoriteEvent> favorites = await _context.FavoriteEvents
                .Where(entry => entry.EventId == eventId)
                .ToListAsync(cancellationToken);
            return favorites;
        }
    }
}

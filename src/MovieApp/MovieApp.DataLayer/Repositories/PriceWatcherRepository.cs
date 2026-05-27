using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class PriceWatcherRepository : IPriceWatcherRepository
    {
        private readonly IMovieAppDbContext _context;

        public PriceWatcherRepository(IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PriceWatcher>> GetAllWatchedEventsAsync()
        {
            return await _context.PriceWatchers
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PriceWatcher?> GetWatchAsync(int eventIdentifier)
        {
            return await _context.PriceWatchers
                .AsNoTracking()
                .FirstOrDefaultAsync(pw => pw.EventId == eventIdentifier);
        }

        public async Task<bool> IsWatchingAsync(int eventIdentifier)
        {
            return await _context.PriceWatchers
                .AnyAsync(pw => pw.EventId == eventIdentifier);
        }

        public async Task<bool> AddWatchAsync(PriceWatcher watchedEvent)
        {
            bool alreadyExists = await _context.PriceWatchers
                .AnyAsync(pw => pw.EventId == watchedEvent.EventId);

            if (alreadyExists)
            {
                return false;
            }

            _context.PriceWatchers.Add(watchedEvent);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task RemoveWatchAsync(int eventIdentifier)
        {
            PriceWatcher? watch = await _context.PriceWatchers
                .FirstOrDefaultAsync(pw => pw.EventId == eventIdentifier);

            if (watch is null)
            {
                return;
            }

            _context.PriceWatchers.Remove(watch);
            await _context.SaveChangesAsync();
        }
    }
}

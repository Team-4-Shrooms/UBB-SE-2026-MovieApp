using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                .FirstOrDefaultAsync(priceWatcher => priceWatcher.EventId == eventIdentifier);
        }

        public async Task<bool> IsWatchingAsync(int eventIdentifier)
        {
            return await _context.PriceWatchers
                .AnyAsync(priceWatcher => priceWatcher.EventId == eventIdentifier);
        }

        public async Task<bool> AddWatchAsync(PriceWatcher watchedEvent)
        {
            bool alreadyWatching = await IsWatchingAsync(watchedEvent.EventId);
            if (alreadyWatching)
            {
                return false;
            }

            _context.PriceWatchers.Add(watchedEvent);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task RemoveWatchAsync(int eventIdentifier)
        {
            var priceWatcher = await _context.PriceWatchers
                .FirstOrDefaultAsync(priceWatcher => priceWatcher.EventId == eventIdentifier);

            if (priceWatcher is not null)
            {
                _context.PriceWatchers.Remove(priceWatcher);
                await _context.SaveChangesAsync();
            }
        }
    }
}

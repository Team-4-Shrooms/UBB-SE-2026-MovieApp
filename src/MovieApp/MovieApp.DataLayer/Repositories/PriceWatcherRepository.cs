using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories;

public sealed class PriceWatcherRepository : IPriceWatcherRepository
{
    private readonly IMovieAppDbContext _context;

    public PriceWatcherRepository(IMovieAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PriceWatcher>> GetAllWatchedEventsAsync()
        => await _context.PriceWatchers.ToListAsync();

    public async Task<PriceWatcher?> GetWatchAsync(int eventIdentifier)
        => await _context.PriceWatchers.FindAsync(eventIdentifier);

    public async Task<bool> IsWatchingAsync(int eventIdentifier)
        => await _context.PriceWatchers.AnyAsync(pw => pw.EventId == eventIdentifier);

    public async Task<bool> AddWatchAsync(PriceWatcher watchedEvent)
    {
        _context.PriceWatchers.Add(watchedEvent);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task RemoveWatchAsync(int eventIdentifier)
    {
        var watch = await _context.PriceWatchers.FindAsync(eventIdentifier);
        if (watch is not null)
        {
            _context.PriceWatchers.Remove(watch);
            await _context.SaveChangesAsync();
        }
    }
}

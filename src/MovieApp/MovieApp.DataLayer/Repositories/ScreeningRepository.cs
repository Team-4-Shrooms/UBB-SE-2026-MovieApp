using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories;

public sealed class ScreeningRepository : IScreeningRepository
{
    private readonly IMovieAppDbContext _context;

    public ScreeningRepository(IMovieAppDbContext context)
    {
        _context = context;
    }

    public async Task<Screening?> GetByIdAsync(int screeningId, CancellationToken cancellationToken = default)
    {
        return await _context.Screenings
            .FirstOrDefaultAsync(screening => screening.Id == screeningId, cancellationToken);
    }

    public async Task<IReadOnlyList<Screening>> GetByEventIdAsync(int eventIdentifier, CancellationToken cancellationToken = default)
    {
        return await _context.Screenings
            .Where(screening => screening.EventId == eventIdentifier)
            .OrderBy(screening => screening.ScreeningTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Screening>> GetByMovieIdAsync(int movieIdentifier, CancellationToken cancellationToken = default)
    {
        return await _context.Screenings
            .Where(screening => screening.MovieId == movieIdentifier)
            .OrderBy(screening => screening.ScreeningTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<Room?> GetRoomAsync(int roomId, CancellationToken cancellationToken = default)
    {
        return await _context.Rooms
            .FirstOrDefaultAsync(room => room.Id == roomId, cancellationToken);
    }

    public async Task<Event?> GetCinemaEventAsync(int eventId, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .FirstOrDefaultAsync(cinemaEvent => cinemaEvent.Id == eventId, cancellationToken);
    }

    public async Task AddAsync(Screening screening, CancellationToken cancellationToken = default)
    {
        await _context.Screenings.AddAsync(screening, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

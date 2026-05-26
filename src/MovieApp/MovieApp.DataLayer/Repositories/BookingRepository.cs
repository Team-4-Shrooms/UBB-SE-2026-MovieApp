using System.Data;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories;

public sealed class BookingRepository : IBookingRepository
{
    private readonly IMovieAppDbContext _context;

    public BookingRepository(IMovieAppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Booking>> GetByScreeningAsync(int screeningId, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(booking => booking.ScreeningId == screeningId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Booking>> GetByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(booking => booking.UserId == userId)
            .OrderByDescending(booking => booking.BookedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ReserveAsync(int screeningId, int userId, IReadOnlyList<(int Row, int Column)> seats, CancellationToken cancellationToken = default)
    {
        if (seats.Count == 0)
        {
            return false;
        }

        var dbContext = (DbContext)_context;
        await using var transaction = await dbContext.Database
            .BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        try
        {
            var existing = await _context.Bookings
                .Where(booking => booking.ScreeningId == screeningId)
                .Select(booking => new { booking.Row, booking.Column })
                .ToListAsync(cancellationToken);

            var taken = existing
                .Select(seat => (seat.Row, seat.Column))
                .ToHashSet();

            if (seats.Any(seat => taken.Contains(seat)))
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }

            var bookings = seats.Select(seat => new Booking
            {
                ScreeningId = screeningId,
                UserId = userId,
                Row = seat.Row,
                Column = seat.Column,
                BookedAt = DateTime.UtcNow,
            });

            await _context.Bookings.AddRangeAsync(bookings, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }
    }

    public async Task<bool> CancelAsync(int bookingId, int userId, CancellationToken cancellationToken = default)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(item => item.Id == bookingId && item.UserId == userId, cancellationToken);

        if (booking is null)
        {
            return false;
        }

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

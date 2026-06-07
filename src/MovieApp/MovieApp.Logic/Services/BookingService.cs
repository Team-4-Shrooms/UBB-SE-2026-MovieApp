using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services;

public sealed class BookingService : IBookingService
{
    private readonly IBookingRepository _bookings;
    private readonly IScreeningRepository _screenings;
    private readonly IUserRepository _users;

    public BookingService(IBookingRepository bookings, IScreeningRepository screenings, IUserRepository users)
    {
        _bookings = bookings;
        _screenings = screenings;
        _users = users;
    }

    public Task<IReadOnlyList<Booking>> GetBookingsForScreeningAsync(int screeningId, CancellationToken cancellationToken = default)
        => _bookings.GetByScreeningAsync(screeningId, cancellationToken);

    public Task<IReadOnlyList<Booking>> GetBookingsForUserAsync(int userId, CancellationToken cancellationToken = default)
        => _bookings.GetByUserAsync(userId, cancellationToken);

    public async Task<bool> BookSeatsAsync(int screeningId, int userId, IReadOnlyList<(int Row, int Column)> seats, CancellationToken cancellationToken = default)
    {
        if (seats is null || seats.Count == 0)
        {
            return false;
        }

        if (seats.Distinct().Count() != seats.Count)
        {
            return false;
        }

        var screening = await _screenings.GetByIdAsync(screeningId, cancellationToken)
            ?? throw new InvalidOperationException($"Screening {screeningId} not found.");

        var room = await _screenings.GetRoomAsync(screening.RoomId, cancellationToken)
            ?? throw new InvalidOperationException($"Room {screening.RoomId} not found.");

        if (seats.Any(seat => seat.Row < 1 || seat.Row > room.Rows || seat.Column < 1 || seat.Column > room.Columns))
        {
            return false;
        }

        decimal totalCost = screening.TicketPrice * seats.Count;
        decimal balance = 0;

        if (totalCost > 0)
        {
            balance = await _users.GetBalanceAsync(userId);
            if (balance < totalCost)
                throw new InvalidOperationException("Insufficient wallet balance.");
        }

        bool reserved = await _bookings.ReserveAsync(screeningId, userId, seats, cancellationToken);

        if (reserved && totalCost > 0)
        {
            await _users.UpdateBalanceAsync(userId, balance - totalCost);
        }

        return reserved;
    }

    public Task<bool> CancelBookingAsync(int bookingId, int userId, CancellationToken cancellationToken = default)
        => _bookings.CancelAsync(bookingId, userId, cancellationToken);
}

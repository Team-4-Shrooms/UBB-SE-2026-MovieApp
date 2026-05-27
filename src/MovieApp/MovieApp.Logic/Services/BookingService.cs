using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services;

public sealed class BookingService : IBookingService
{
    private readonly IBookingRepository _bookings;
    private readonly IScreeningRepository _screenings;

    public BookingService(IBookingRepository bookings, IScreeningRepository screenings)
    {
        _bookings = bookings;
        _screenings = screenings;
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

        return await _bookings.ReserveAsync(screeningId, userId, seats, cancellationToken);
    }

    public Task<bool> CancelBookingAsync(int bookingId, int userId, CancellationToken cancellationToken = default)
        => _bookings.CancelAsync(bookingId, userId, cancellationToken);
}

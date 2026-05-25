using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Models;
using MovieApp.WebDTOs.DTOs;

namespace MovieApp.Logic.Services;

public sealed class ScreeningService : IScreeningService
{
    private readonly IScreeningRepository _screenings;
    private readonly IBookingRepository _bookings;
    private readonly IMovieRepository _movies;

    public ScreeningService(
        IScreeningRepository screenings,
        IBookingRepository bookings,
        IMovieRepository movies)
    {
        _screenings = screenings;
        _bookings = bookings;
        _movies = movies;
    }

    public Task<Screening?> GetScreeningAsync(int screeningId, CancellationToken cancellationToken = default)
        => _screenings.GetByIdAsync(screeningId, cancellationToken);

    public Task<IReadOnlyList<Screening>> GetScreeningsByEventAsync(int eventId, CancellationToken cancellationToken = default)
        => _screenings.GetByEventIdAsync(eventId, cancellationToken);

    public Task<IReadOnlyList<Screening>> GetScreeningsByMovieAsync(int movieId, CancellationToken cancellationToken = default)
        => _screenings.GetByMovieIdAsync(movieId, cancellationToken);

    public async Task<IReadOnlyList<Seat>> GetAvailableSeatsAsync(int screeningId, CancellationToken cancellationToken = default)
    {
        var screening = await _screenings.GetByIdAsync(screeningId, cancellationToken)
            ?? throw new InvalidOperationException($"Screening {screeningId} not found.");

        var room = await _screenings.GetRoomAsync(screening.RoomId, cancellationToken)
            ?? throw new InvalidOperationException($"Room {screening.RoomId} not found.");

        var bookings = await _bookings.GetByScreeningAsync(screeningId, cancellationToken);
        var taken = bookings.Select(booking => (booking.Row, booking.Column)).ToHashSet();

        var seats = new List<Seat>(room.Rows * room.Columns);
        for (int row = 1; row <= room.Rows; row++)
        {
            for (int column = 1; column <= room.Columns; column++)
            {
                seats.Add(new Seat
                {
                    Row = row,
                    Column = column,
                    Quality = SeatQuality.Standard,
                    IsAvailable = !taken.Contains((row, column)),
                });
            }
        }

        return seats;
    }

    public async Task<ScreeningDetailsDto?> GetScreeningDetailsAsync(int screeningId, CancellationToken cancellationToken = default)
    {
        var screening = await _screenings.GetByIdAsync(screeningId, cancellationToken);
        if (screening is null)
        {
            return null;
        }

        var room = await _screenings.GetRoomAsync(screening.RoomId, cancellationToken);
        var cinemaEvent = await _screenings.GetCinemaEventAsync(screening.EventId, cancellationToken);
        var movie = await _movies.GetMovieByIdAsync(screening.MovieId);
        var bookings = await _bookings.GetByScreeningAsync(screeningId, cancellationToken);
        var taken = bookings.Select(booking => (booking.Row, booking.Column)).ToHashSet();

        int rows = room?.Rows ?? 0;
        int columns = room?.Columns ?? 0;
        var seatStatuses = new List<SeatStatusDto>(rows * columns);
        for (int row = 1; row <= rows; row++)
        {
            for (int column = 1; column <= columns; column++)
            {
                seatStatuses.Add(new SeatStatusDto
                {
                    Row = row,
                    Column = column,
                    IsAvailable = !taken.Contains((row, column)),
                });
            }
        }

        return new ScreeningDetailsDto
        {
            ScreeningId = screening.Id,
            ScreeningTime = screening.ScreeningTime,
            MovieId = screening.MovieId,
            MovieTitle = movie?.Title ?? $"Movie #{screening.MovieId}",
            MoviePosterUrl = movie?.PosterUrl ?? string.Empty,
            EventId = screening.EventId,
            EventTitle = cinemaEvent?.Title ?? $"Event #{screening.EventId}",
            EventLocation = cinemaEvent?.LocationReference ?? string.Empty,
            TicketPrice = cinemaEvent?.TicketPrice ?? 0m,
            RoomId = screening.RoomId,
            RoomName = room?.Name ?? $"Room #{screening.RoomId}",
            Rows = rows,
            Columns = columns,
            Seats = seatStatuses,
        };
    }

    public async Task AddScreeningAsync(Screening screening, CancellationToken cancellationToken = default)
    {
        var room = await _screenings.GetRoomAsync(screening.RoomId, cancellationToken)
            ?? throw new InvalidOperationException($"Room {screening.RoomId} not found.");

        if (room.EventId != screening.EventId)
        {
            throw new InvalidOperationException("Room does not belong to the screening's event.");
        }

        await _screenings.AddAsync(screening, cancellationToken);
    }
}

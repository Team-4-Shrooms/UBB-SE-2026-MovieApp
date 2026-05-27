using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;
using MovieApp.Logic.Services;

namespace MovieApp.Tests.Repositories;

public sealed class BookingFlowTests
{
    private const int RoomRows = 5;
    private const int RoomColumns = 8;

    [Fact]
    public async Task BookSeat_SecondBookingOfSameSeat_Fails()
    {
        using AppDbContext context = TestDbContextFactory.Create();
        var (screening, _) = await SeedAsync(context);

        var screeningRepo = new ScreeningRepository(context);
        var bookingRepo = new BookingRepository(context);
        var bookingService = new BookingService(bookingRepo, screeningRepo);

        var seat = new List<(int Row, int Column)> { (2, 3) };

        bool first = await bookingService.BookSeatsAsync(screening.Id, userId: 1, seat);
        bool second = await bookingService.BookSeatsAsync(screening.Id, userId: 2, seat);

        Assert.True(first);
        Assert.False(second);
    }

    [Fact]
    public async Task GetSeatMap_AfterBooking_MarksBookedSeatUnavailable()
    {
        using AppDbContext context = TestDbContextFactory.Create();
        var (screening, _) = await SeedAsync(context);

        var screeningRepo = new ScreeningRepository(context);
        var bookingRepo = new BookingRepository(context);
        var movieRepo = new MovieRepository(context);
        var screeningService = new ScreeningService(screeningRepo, bookingRepo, movieRepo);
        var bookingService = new BookingService(bookingRepo, screeningRepo);

        await bookingService.BookSeatsAsync(screening.Id, userId: 1, new List<(int, int)> { (1, 1) });

        var seats = await screeningService.GetAvailableSeatsAsync(screening.Id);

        Assert.Equal(RoomRows * RoomColumns, seats.Count);
        Assert.False(seats.Single(s => s.Row == 1 && s.Column == 1).IsAvailable);
        Assert.True(seats.Single(s => s.Row == 1 && s.Column == 2).IsAvailable);
    }

    [Fact]
    public async Task BookSeat_DuplicateInSameRequest_Fails()
    {
        using AppDbContext context = TestDbContextFactory.Create();
        var (screening, _) = await SeedAsync(context);

        var screeningRepo = new ScreeningRepository(context);
        var bookingRepo = new BookingRepository(context);
        var bookingService = new BookingService(bookingRepo, screeningRepo);

        var duplicates = new List<(int Row, int Column)> { (1, 1), (1, 1) };

        bool result = await bookingService.BookSeatsAsync(screening.Id, userId: 1, duplicates);

        Assert.False(result);
    }

    [Fact]
    public async Task BookSeat_OutsideRoomBounds_Fails()
    {
        using AppDbContext context = TestDbContextFactory.Create();
        var (screening, _) = await SeedAsync(context);

        var screeningRepo = new ScreeningRepository(context);
        var bookingRepo = new BookingRepository(context);
        var bookingService = new BookingService(bookingRepo, screeningRepo);

        bool result = await bookingService.BookSeatsAsync(
            screening.Id,
            userId: 1,
            new List<(int, int)> { (RoomRows + 1, 1) });

        Assert.False(result);
    }

    [Fact]
    public async Task CancelBooking_RemovesSeatAndAllowsRebook()
    {
        using AppDbContext context = TestDbContextFactory.Create();
        var (screening, _) = await SeedAsync(context);

        var screeningRepo = new ScreeningRepository(context);
        var bookingRepo = new BookingRepository(context);
        var bookingService = new BookingService(bookingRepo, screeningRepo);

        await bookingService.BookSeatsAsync(screening.Id, userId: 1, new List<(int, int)> { (3, 3) });
        var booking = context.Bookings.Single(b => b.Row == 3 && b.Column == 3);

        bool cancelled = await bookingService.CancelBookingAsync(booking.Id, userId: 1);
        bool rebook = await bookingService.BookSeatsAsync(screening.Id, userId: 2, new List<(int, int)> { (3, 3) });

        Assert.True(cancelled);
        Assert.True(rebook);
    }

    private static async Task<(Screening Screening, Room Room)> SeedAsync(AppDbContext context)
    {
        var cinemaEvent = new Event
        {
            Id = 1,
            Title = "Test",
            EventDateTime = DateTime.UtcNow.AddDays(1),
            LocationReference = "Hall T",
            TicketPrice = 10m,
            CreatorUserId = 1,
        };
        context.Events.Add(cinemaEvent);

        var room = new Room
        {
            Id = 1,
            EventId = cinemaEvent.Id,
            Name = "Hall T",
            Rows = RoomRows,
            Columns = RoomColumns,
        };
        context.Rooms.Add(room);

        var screening = new Screening
        {
            Id = 1,
            EventId = cinemaEvent.Id,
            MovieId = 1,
            RoomId = room.Id,
            ScreeningTime = DateTime.UtcNow.AddDays(1),
        };
        context.Screenings.Add(screening);

        await context.SaveChangesAsync();
        return (screening, room);
    }
}

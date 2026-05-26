namespace MovieApp.Logic.Interfaces.Services;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

public interface IBookingService
{
    Task<IReadOnlyList<Booking>> GetBookingsForScreeningAsync(int screeningId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetBookingsForUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> BookSeatsAsync(int screeningId, int userId, IReadOnlyList<(int Row, int Column)> seats, CancellationToken cancellationToken = default);
    Task<bool> CancelBookingAsync(int bookingId, int userId, CancellationToken cancellationToken = default);
}

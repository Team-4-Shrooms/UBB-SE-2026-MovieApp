

namespace MovieApp.DataLayer.Interfaces.Repositories;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

public interface IBookingRepository
{
    Task<IReadOnlyList<Booking>> GetByScreeningAsync(int screeningId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Booking>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);

    Task<bool> ReserveAsync(int screeningId, int userId, IReadOnlyList<(int Row, int Column)> seats, CancellationToken cancellationToken = default);

    Task<bool> CancelAsync(int bookingId, int userId, CancellationToken cancellationToken = default);
}

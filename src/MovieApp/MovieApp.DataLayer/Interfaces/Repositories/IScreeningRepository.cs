namespace MovieApp.DataLayer.Repositories;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

public interface IScreeningRepository
{
    Task<Screening?> GetByIdAsync(int screeningId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Screening>> GetByEventIdAsync(int eventIdentifier, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Screening>> GetByMovieIdAsync(int movieIdentifier, CancellationToken cancellationToken = default);

    Task<Room?> GetRoomAsync(int roomId, CancellationToken cancellationToken = default);

    Task<Event?> GetCinemaEventAsync(int eventId, CancellationToken cancellationToken = default);

    Task AddAsync(Screening screening, CancellationToken cancellationToken = default);
}

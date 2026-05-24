namespace MovieApp.Logic.Interfaces.Services;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Models;

public interface IScreeningService
{
    Task<Screening?> GetScreeningAsync(int screeningId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Screening>> GetScreeningsByEventAsync(int eventId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Screening>> GetScreeningsByMovieAsync(int movieId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Seat>> GetAvailableSeatsAsync(int screeningId, CancellationToken cancellationToken = default);
    Task AddScreeningAsync(Screening screening, CancellationToken cancellationToken = default);
}

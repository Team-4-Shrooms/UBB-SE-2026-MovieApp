using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.DataLayer.Interfaces.Repositories;

namespace MovieApp.Logic.Services
{
    public class ScreeningService : IScreeningService
    {
        private readonly IScreeningRepository _screeningRepo;
        
        public ScreeningService(IScreeningRepository screeningRepo)
        {
            _screeningRepo = screeningRepo;
        }

        public Task<Screening?> GetScreeningAsync(int screeningId, CancellationToken cancellationToken = default)
        {
            return _screeningRepo.GetByIdAsync(screeningId, cancellationToken);
        }

        public Task<IReadOnlyList<Screening>> GetScreeningsByEventAsync(int eventId, CancellationToken cancellationToken = default)
        {
            return _screeningRepo.GetByEventIdAsync(eventId, cancellationToken);
        }

        public Task<IReadOnlyList<Screening>> GetScreeningsByMovieAsync(int movieId, CancellationToken cancellationToken = default)
        {
            return _screeningRepo.GetByMovieIdAsync(movieId, cancellationToken);
        }

        public Task<IReadOnlyList<Seat>> GetAvailableSeatsAsync(int screeningId, CancellationToken cancellationToken = default)
        {
            // Placeholder - actual seat logic would query room size and booked seats
            return Task.FromResult((IReadOnlyList<Seat>)new List<Seat>());
        }

        public Task AddScreeningAsync(Screening screening, CancellationToken cancellationToken = default)
        {
            return _screeningRepo.AddAsync(screening, cancellationToken);
        }
    }
}

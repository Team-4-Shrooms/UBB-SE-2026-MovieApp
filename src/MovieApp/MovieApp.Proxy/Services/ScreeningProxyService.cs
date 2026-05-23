using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services
{
    public class ScreeningProxyService : IScreeningService
    {
        private readonly ApiClient _apiClient;
        private readonly string _baseEndpoint = "api/screenings";

        public ScreeningProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<Screening?> GetScreeningAsync(int screeningId, CancellationToken cancellationToken = default)
        {
            return await _apiClient.GetAsync<Screening>($"{_baseEndpoint}/{screeningId}");
        }

        public async Task<IReadOnlyList<Screening>> GetScreeningsByEventAsync(int eventId, CancellationToken cancellationToken = default)
        {
            var result = await _apiClient.GetAsync<List<Screening>>($"{_baseEndpoint}?eventId={eventId}");
            return result ?? new List<Screening>();
        }

        public async Task<IReadOnlyList<Screening>> GetScreeningsByMovieAsync(int movieId, CancellationToken cancellationToken = default)
        {
            var result = await _apiClient.GetAsync<List<Screening>>($"{_baseEndpoint}?movieId={movieId}");
            return result ?? new List<Screening>();
        }

        public async Task<IReadOnlyList<Seat>> GetAvailableSeatsAsync(int screeningId, CancellationToken cancellationToken = default)
        {
            var result = await _apiClient.GetAsync<List<Seat>>($"{_baseEndpoint}/{screeningId}/seats");
            return result ?? new List<Seat>();
        }

        public async Task AddScreeningAsync(Screening screening, CancellationToken cancellationToken = default)
        {
            await _apiClient.PostAsync($"{_baseEndpoint}", screening);
        }
    }
}

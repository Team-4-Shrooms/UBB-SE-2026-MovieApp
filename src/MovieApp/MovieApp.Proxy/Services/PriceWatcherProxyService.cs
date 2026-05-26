namespace MovieApp.Proxy.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MovieApp.DataLayer.Models;
    using MovieApp.Logic.Interfaces.Services;

    public sealed class PriceWatcherProxyService : IPriceWatcherService
    {
        private readonly ApiClient _apiClient;
        private readonly string _baseEndpoint = "/api/price-watchers";

        public PriceWatcherProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<PriceWatcher>> GetAllWatchedEventsAsync()
        {
            List<PriceWatcher>? result = await _apiClient.GetAsync<List<PriceWatcher>>($"{_baseEndpoint}");
            return result ?? new List<PriceWatcher>();
        }

        public async Task<PriceWatcher?> GetWatchAsync(int eventIdentifier)
        {
            return await _apiClient.GetAsync<PriceWatcher?>($"{_baseEndpoint}/{eventIdentifier}");
        }

        public async Task<bool> IsWatchingAsync(int eventIdentifier)
        {
            bool? result = await _apiClient.GetAsync<bool?>($"{_baseEndpoint}/check/{eventIdentifier}");
            return result ?? false;
        }

        public async Task<bool> AddWatchAsync(PriceWatcher watchedEvent)
        {
            bool? result = await _apiClient.PostAsync<PriceWatcher, bool>($"{_baseEndpoint}", watchedEvent);
            return result ?? false;
        }

        public async Task RemoveWatchAsync(int eventIdentifier)
        {
            await _apiClient.DeleteAsync($"{_baseEndpoint}/{eventIdentifier}");
        }
    }
}

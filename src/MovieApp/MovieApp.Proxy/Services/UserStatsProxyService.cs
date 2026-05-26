namespace MovieApp.Proxy.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using MovieApp.DataLayer.Models;
    using MovieApp.Logic.Interfaces.Services;

    public sealed class UserStatsProxyService : IUserStatsService
    {
        private readonly ApiClient _apiClient;
        private readonly string _baseEndpoint = "/api/stats";

        public UserStatsProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<UserStats?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _apiClient.GetAsync<UserStats?>($"{_baseEndpoint}/{userId}", cancellationToken);
        }
    }
}

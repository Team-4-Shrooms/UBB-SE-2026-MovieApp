namespace MovieApp.Proxy.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MovieApp.DataLayer.Models;
    using MovieApp.Logic.Interfaces.Services;

    public sealed class BadgeProxyService : IBadgeService
    {
        private readonly ApiClient _apiClient;
        private readonly string _baseEndpoint = "/api/badges";

        public BadgeProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<UserBadge>> GetUserBadgesAsync(int userId, CancellationToken cancellationToken = default)
        {
            List<UserBadge>? result = await _apiClient.GetAsync<List<UserBadge>>($"{_baseEndpoint}/{userId}", cancellationToken);
            return result ?? new List<UserBadge>();
        }

        public async Task<List<Badge>> GetAllBadgesAsync(CancellationToken cancellationToken = default)
        {
            List<Badge>? result = await _apiClient.GetAsync<List<Badge>>($"{_baseEndpoint}", cancellationToken);
            return result ?? new List<Badge>();
        }

        public async Task<IList<UserStats>> GetLeaderboardAsync(CancellationToken cancellationToken = default)
        {
            IList<UserStats>? result = await _apiClient.GetAsync<List<UserStats>>($"{_baseEndpoint}/leaderboard", cancellationToken);
            return result ?? new List<UserStats>();
        }

        public Task CheckAndAwardBadgesAsync(int userId, CancellationToken cancellationToken = default)
        {
            return _apiClient.PostAsync($"{_baseEndpoint}/{userId}/award", new { }, cancellationToken);
        }
    }
}

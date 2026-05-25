namespace MovieApp.Proxy.Services
{
    using System.Collections.Generic;
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

        public async Task<UserStats?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _apiClient.GetAsync<UserStats?>($"{_baseEndpoint}/{userId}", ct);
        }

        public async Task<List<UserStats>> GetAllAsync(CancellationToken ct = default)
        {
            List<UserStats>? result = await _apiClient.GetAsync<List<UserStats>>($"{_baseEndpoint}", ct);
            return result ?? new List<UserStats>();
        }

        public async Task<int> InsertAsync(UserStats userStats, CancellationToken ct = default)
        {
            int result = await _apiClient.PostAsync<UserStats, int>($"{_baseEndpoint}", userStats, ct);
            return result;
        }

        public async Task<bool> UpdateAsync(UserStats userStats, CancellationToken ct = default)
        {
            bool result = await _apiClient.PutAsync<UserStats, bool>($"{_baseEndpoint}/{userStats.UserStatsId}", userStats, ct);
            return result;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            await _apiClient.DeleteAsync($"{_baseEndpoint}/{id}", ct);
            return true;
        }

        public async Task<IList<UserStats>> GetLeaderboardAsync(CancellationToken ct = default)
        {
            List<UserStats>? result = await _apiClient.GetAsync<List<UserStats>>($"{_baseEndpoint}/leaderboard", ct);
            return result ?? new List<UserStats>();
        }
    }
}

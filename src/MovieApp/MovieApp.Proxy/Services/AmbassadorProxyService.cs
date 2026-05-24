using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services
{
    public class AmbassadorProxyService: IAmbassadorService
    {
        private readonly ApiClient _apiClient;
        private readonly string _baseEndpoint = "/api/ambassadors";

        public AmbassadorProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task CreateAmbassadorProfileAsync(int userId, string referralCode, CancellationToken ct = default)
        {
            await _apiClient.PostAsync($"{_baseEndpoint}/{userId}/profile", new { referralCode }, ct);
        }

        public async Task<IEnumerable<AmbassadorProfile>> GetAllAmbassadorsAsync(CancellationToken ct = default)
        {
            var result = await _apiClient.GetAsync<IEnumerable<AmbassadorProfile>>($"/api/ambassadors", ct);
            return result ?? Enumerable.Empty<AmbassadorProfile>();
        }

        public async Task<AmbassadorProfile?> GetAmbassadorByIdAsync(int id, CancellationToken ct = default)
        {
            return await _apiClient.GetAsync<AmbassadorProfile?>($"/api/ambassadors/{id}", ct);
        }

        public async Task<string?> GetReferralCodeAsync(int userId, CancellationToken ct = default)
        {
            return await _apiClient.GetAsync<string?>($"{_baseEndpoint}/{userId}/my-code", ct);
        }

        public async Task<IEnumerable<ReferralHistoryItem>> GetReferralHistoryAsync(int ambassadorId, CancellationToken ct = default)
        {
            var result = await _apiClient.GetAsync<IEnumerable<ReferralHistoryItem>>($"{_baseEndpoint}/history/{ambassadorId}", ct);
            return result ?? Enumerable.Empty<ReferralHistoryItem>();
        }

        public async Task<int> GetRewardBalanceAsync(int userId, CancellationToken ct = default)
        {
            var result = await _apiClient.GetAsync<int?>($"{_baseEndpoint}/{userId}/rewards/balance", ct);
            return result ?? 0;
        }

        public async Task<bool> IsReferralCodeValidAsync(string referralCode, CancellationToken ct = default)
        {
            var result = await _apiClient.GetAsync<bool?>($"{_baseEndpoint}/referral/validate?code={referralCode}", ct);
            return result ?? false;
        }

        public async Task ProcessReferralAsync(string referralCode, int friendId, int eventId, CancellationToken ct = default)
        {
            await _apiClient.PostAsync($"{_baseEndpoint}/referral/process", new { referralCode, friendId, eventId }, ct);
        }

        public async Task RedeemRewardAsync(int userId, CancellationToken ct = default)
        {
            await _apiClient.PostAsync($"{_baseEndpoint}/{userId}/rewards/redeem", new { }, ct);
        }
    }
}

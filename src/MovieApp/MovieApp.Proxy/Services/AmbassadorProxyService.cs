using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public async Task CreateAmbassadorProfileAsync(int userId, string referralCode, CancellationToken cancellationToken = default)
        {
            await _apiClient.PostAsync($"{_baseEndpoint}/{userId}/profile", new { code = referralCode }, cancellationToken);
        }

        public async Task<IEnumerable<AmbassadorProfile>> GetAllAmbassadorsAsync(CancellationToken cancellationToken = default)
        {
            var result = await _apiClient.GetAsync<IEnumerable<AmbassadorProfile>>($"{_baseEndpoint}", cancellationToken);
            return result ?? Enumerable.Empty<AmbassadorProfile>();
        }

        public async Task<AmbassadorProfile?> GetAmbassadorByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _apiClient.GetAsync<AmbassadorProfile?>($"{_baseEndpoint}/{id}", cancellationToken);
        }

        public async Task<string?> GetReferralCodeAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _apiClient.GetAsync<string?>($"{_baseEndpoint}/{userId}/my-code", cancellationToken);
        }

        public async Task<IEnumerable<ReferralHistoryItem>> GetReferralHistoryAsync(int ambassadorId, CancellationToken cancellationToken = default)
        {
            var result = await _apiClient.GetAsync<IEnumerable<ReferralHistoryItem>>($"{_baseEndpoint}/history/{ambassadorId}", cancellationToken);
            return result ?? Enumerable.Empty<ReferralHistoryItem>();
        }

        public async Task<int> GetRewardBalanceAsync(int userId, CancellationToken cancellationToken = default)
        {
            var result = await _apiClient.GetAsync<int?>($"{_baseEndpoint}/{userId}/rewards/balance", cancellationToken);
            return result ?? 0;
        }

        public async Task<bool> IsReferralCodeValidAsync(string referralCode, CancellationToken cancellationToken = default)
        {
            var result = await _apiClient.GetAsync<bool?>($"{_baseEndpoint}/referral/validate?code={referralCode}", cancellationToken);
            return result ?? false;
        }

        public async Task ProcessReferralAsync(string referralCode, int friendId, int eventId, CancellationToken cancellationToken = default)
        {
            await _apiClient.PostAsync($"{_baseEndpoint}/referral/process", new { referralCode, friendId, eventId }, cancellationToken);
        }

        public async Task RedeemRewardAsync(int userId, CancellationToken cancellationToken = default)
        {
            await _apiClient.PostAsync($"{_baseEndpoint}/{userId}/rewards/redeem", new { }, cancellationToken);
        }

        public async Task<int?> ResolveCodeToUserIdAsync(string referralCode, CancellationToken cancellationToken = default)
        {
            return await _apiClient.GetAsync<int?>($"{_baseEndpoint}/resolve?code={referralCode}", cancellationToken);
        }

        public async Task<bool> ReferralLogExistsAsync(int ambassadorId, int friendId, int eventId, CancellationToken cancellationToken = default)
        {
            var result = await _apiClient.GetAsync<bool?>($"{_baseEndpoint}/{ambassadorId}/referral-log/exists?friendId={friendId}&eventId={eventId}", cancellationToken);
            return result ?? false;
        }

        public async Task LogReferralByAmbassadorIdAsync(int ambassadorId, int friendId, int eventId, CancellationToken cancellationToken = default)
        {
            await _apiClient.PostAsync($"{_baseEndpoint}/referral-log", new { ambassadorId, friendId, eventId }, cancellationToken);
        }

        public async Task DecrementRewardBalanceAsync(int userId, CancellationToken cancellationToken = default)
        {
            await _apiClient.PostAsync($"{_baseEndpoint}/{userId}/rewards/decrement", new { }, cancellationToken);
        }
    }
}

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
        public AmbassadorProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public Task CreateAmbassadorProfileAsync(int userId, string referralCode, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetReferralCodeAsync(int userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReferralHistoryItem>> GetReferralHistoryAsync(int ambassadorId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetRewardBalanceAsync(int userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsReferralCodeValidAsync(string referralCode, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task ProcessReferralAsync(string referralCode, int friendId, int eventId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task RedeemRewardAsync(int userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}

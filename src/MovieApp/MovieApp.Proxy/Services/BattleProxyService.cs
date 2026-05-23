using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.Proxy.Services
{
    public class BattleProxyService : IBattleService
    {
        private readonly ApiClient _apiClient;

        public BattleProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IEnumerable<Battle>> GetBattlesAsync(CancellationToken cancellationToken = default)
        {
            return await _apiClient.GetAsync<IEnumerable<Battle>>("api/battles") ?? new List<Battle>();
        }

        public async Task<Battle?> GetBattleByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _apiClient.GetAsync<Battle>($"api/battles/{id}");
        }

        public async Task<Battle?> GetActiveBattleAsync(CancellationToken cancellationToken = default)
        {
            return await _apiClient.GetAsync<Battle>("api/battles/active");
        }

        public async Task<Battle> CreateBattleAsync(int firstMovieId, int secondMovieId, CancellationToken cancellationToken = default)
        {
            PlaceBattleRequest request = new PlaceBattleRequest { FirstMovieId = firstMovieId, SecondMovieId = secondMovieId };
            Battle? response = await _apiClient.PostAsync<object, Battle>("api/battles", request);
            if (response == null)
            {
                throw new Exception("Failed to create battle.");
            }
            return response;
        }

        public async Task<BattleBet> PlaceBetAsync(int userId, int battleId, int movieId, int amount, CancellationToken cancellationToken = default)
        {
            PlaceBetRequest request = new PlaceBetRequest { MovieId = movieId, Amount = amount };
            BattleBet? response = await _apiClient.PostAsync<PlaceBetRequest, BattleBet>($"api/battles/{battleId}/bet", request);
            if (response == null)
            {
                throw new Exception("Failed to place bet.");
            }
            return response;
        }

        public async Task<BattleBet?> GetBetAsync(int userId, int battleId, CancellationToken cancellationToken = default)
        {
            return await _apiClient.GetAsync<BattleBet>($"api/battles/{battleId}/bet?userId={userId}");
        }

        public async Task<int> DetermineWinnerAsync(int battleId, CancellationToken cancellationToken = default)
        {
            int response = await _apiClient.PostAsync<object, int>($"api/battles/{battleId}/determine-winner", new { });
            return response;
        }

        public async Task DistributePayoutsAsync(int battleId, CancellationToken cancellationToken = default)
        {
            await _apiClient.PostAsync($"api/battles/{battleId}/distribute-payouts", new { });
        }

        public async Task SettleExpiredBattlesAsync(CancellationToken cancellationToken = default)
        {
            await _apiClient.PostAsync("api/battles/settle-expired", new { });
        }

        public async Task<Battle?> GetCurrentBattleForUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _apiClient.GetAsync<Battle>($"api/battles/current?userId={userId}");
        }

        public async Task ForceSettleBattleAsync(int battleId, CancellationToken cancellationToken = default)
        {
            await _apiClient.PostAsync($"api/battles/{battleId}/force-settle", new { });
        }

        public async Task ResetAllBattlesForDemoAsync(CancellationToken cancellationToken = default)
        {
            await _apiClient.PostAsync("api/battles/reset-demo", new { });
        }

        public async Task<Battle> CreateDemoBattleAsync(CancellationToken cancellationToken = default)
        {
            Battle? response = await _apiClient.PostAsync<object, Battle>("api/battles/demo", new { });
            if (response == null)
            {
                throw new Exception("Failed to create demo battle.");
            }
            return response;
        }
    }
}

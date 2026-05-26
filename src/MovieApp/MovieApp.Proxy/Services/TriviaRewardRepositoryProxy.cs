using System;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy.Services;

public sealed class TriviaRewardRepositoryProxy : ITriviaRewardRepository
{
    private readonly ApiClient _apiClient;

    public TriviaRewardRepositoryProxy(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task AddAsync(TriviaReward reward, CancellationToken cancellationToken = default)
    {
        await _apiClient.PostAsync(
            "api/trivia/reward",
            new { reward.UserId },
            cancellationToken);
    }

    public async Task<TriviaReward?> GetUnredeemedByUserAsync(
        int userIdentifier,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.GetAsync<TriviaReward>(
            $"api/trivia/reward/{userIdentifier}",
            cancellationToken);
    }

    public Task MarkAsRedeemedAsync(int rewardIdentifier, CancellationToken cancellationToken = default)
    {
        return _apiClient.PutAsync(
            $"api/trivia/reward/{rewardIdentifier}/redeem",
            new { },
            cancellationToken);
    }
}

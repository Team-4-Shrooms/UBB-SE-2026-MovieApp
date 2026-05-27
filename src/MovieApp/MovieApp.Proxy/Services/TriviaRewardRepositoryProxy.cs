using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy.Services;

/// <summary>
/// Proxy implementation of <see cref="ITriviaRewardRepository"/> that delegates to the
/// WebApi service endpoints.
/// </summary>
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
            $"api/trivia/rewards/{reward.UserId}/award",
            new { },
            cancellationToken);
    }

    public async Task<TriviaReward?> GetUnredeemedByUserAsync(
        int userIdentifier,
        CancellationToken cancellationToken = default)
    {
        var rewards = await _apiClient.GetAsync<List<TriviaReward>>(
            $"api/trivia/rewards/{userIdentifier}",
            cancellationToken);
        return rewards?.FirstOrDefault(reward => !reward.IsRedeemed);
    }

    public async Task MarkAsRedeemedAsync(
        int rewardIdentifier,
        CancellationToken cancellationToken = default)
    {
        await _apiClient.PostAsync(
            $"api/trivia/rewards/{rewardIdentifier}/redeem",
            new { },
            cancellationToken);
    }
}

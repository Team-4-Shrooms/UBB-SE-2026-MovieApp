using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services;

public sealed class TriviaProxyService : ITriviaService
{
    private readonly ApiClient _apiClient;

    public TriviaProxyService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<TriviaQuestion>> GetAllQuestionsAsync(CancellationToken cancellationToken = default)
    {
        // Fetches questions from the "General" category as a representative set.
        var questions = await _apiClient.GetAsync<List<TriviaQuestion>>(
            "api/trivia/category/General", cancellationToken);
        return questions ?? new List<TriviaQuestion>();
    }

    public async Task<List<TriviaQuestion>> GetQuestionsByMovieIdAsync(int movieId, CancellationToken cancellationToken = default)
    {
        var questions = await _apiClient.GetAsync<List<TriviaQuestion>>(
            $"api/trivia/movie/{movieId}", cancellationToken);
        return questions ?? new List<TriviaQuestion>();
    }

    public Task<TriviaQuestion?> GetQuestionByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // No single-question-by-id endpoint exists; callers should use GetByCategoryAsync instead.
        return Task.FromResult<TriviaQuestion?>(null);
    }

    public async Task<List<TriviaReward>> GetRewardsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var reward = await _apiClient.GetAsync<TriviaReward>(
            $"api/trivia/reward/{userId}", cancellationToken);
        return reward is null ? new List<TriviaReward>() : new List<TriviaReward> { reward };
    }

    public async Task<int> AwardRewardAsync(int userId, CancellationToken cancellationToken = default)
    {
        var rewardId = await _apiClient.PostAsync<object, int>(
            "api/trivia/reward", new { UserId = userId }, cancellationToken);
        return rewardId;
    }

    public async Task<bool> RedeemRewardAsync(int rewardId, CancellationToken cancellationToken = default)
    {
        await _apiClient.PutAsync(
            $"api/trivia/reward/{rewardId}/redeem", new { }, cancellationToken);
        return true;
    }
}

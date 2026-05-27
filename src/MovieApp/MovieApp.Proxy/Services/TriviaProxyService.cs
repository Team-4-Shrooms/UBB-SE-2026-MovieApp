using System;
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

    public async Task<List<TriviaQuestion>> GetAllQuestionsAsync(
        CancellationToken cancellationToken = default)
    {
        var questions = await _apiClient.GetAsync<List<TriviaQuestion>>(
            "api/trivia/questions", cancellationToken);
        return questions ?? new List<TriviaQuestion>();
    }

    public async Task<List<TriviaQuestion>> GetQuestionsByCategoryAsync(
        string category,
        CancellationToken cancellationToken = default)
    {
        var questions = await _apiClient.GetAsync<List<TriviaQuestion>>(
            $"api/trivia/questions/category/{Uri.EscapeDataString(category)}",
            cancellationToken);
        return questions ?? new List<TriviaQuestion>();
    }

    public async Task<List<TriviaQuestion>> GetQuestionsByMovieIdAsync(
        int movieId,
        CancellationToken cancellationToken = default)
    {
        var questions = await _apiClient.GetAsync<List<TriviaQuestion>>(
            $"api/trivia/questions/movie/{movieId}",
            cancellationToken);
        return questions ?? new List<TriviaQuestion>();
    }

    public async Task<TriviaQuestion?> GetQuestionByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.GetAsync<TriviaQuestion>(
            $"api/trivia/questions/{id}",
            cancellationToken);
    }

    public async Task<List<TriviaReward>> GetRewardsByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var rewards = await _apiClient.GetAsync<List<TriviaReward>>(
            $"api/trivia/rewards/{userId}",
            cancellationToken);
        return rewards ?? new List<TriviaReward>();
    }

    public async Task<int> AwardRewardAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var rewardId = await _apiClient.PostAsync<object, int>(
            $"api/trivia/rewards/{userId}/award",
            new { },
            cancellationToken);
        return rewardId;
    }

    public async Task<bool> RedeemRewardAsync(
        int rewardId,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.PostAsync<object, bool>(
            $"api/trivia/rewards/{rewardId}/redeem",
            new { },
            cancellationToken);
    }
}

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

    public async Task<List<TriviaQuestion>> GetAllQuestionsAsync(CancellationToken ct = default)
    {
        var questions = await _apiClient.GetAsync<List<TriviaQuestion>>("api/trivia/questions", ct);
        return questions ?? new List<TriviaQuestion>();
    }

    public async Task<List<TriviaQuestion>> GetQuestionsByCategoryAsync(
        string category,
        CancellationToken ct = default)
    {
        var questions = await _apiClient.GetAsync<List<TriviaQuestion>>(
            $"api/trivia/questions/category/{Uri.EscapeDataString(category)}", ct);
        return questions ?? new List<TriviaQuestion>();
    }

    public async Task<List<TriviaQuestion>> GetQuestionsByMovieIdAsync(
        int movieId,
        CancellationToken ct = default)
    {
        var questions = await _apiClient.GetAsync<List<TriviaQuestion>>(
            $"api/trivia/questions/movie/{movieId}", ct);
        return questions ?? new List<TriviaQuestion>();
    }

    public async Task<TriviaQuestion?> GetQuestionByIdAsync(int id, CancellationToken ct = default)
    {
        return await _apiClient.GetAsync<TriviaQuestion>($"api/trivia/questions/{id}", ct);
    }

    public async Task<List<TriviaReward>> GetRewardsByUserIdAsync(int userId, CancellationToken ct = default)
    {
        var rewards = await _apiClient.GetAsync<List<TriviaReward>>($"api/trivia/rewards/{userId}", ct);
        return rewards ?? new List<TriviaReward>();
    }

    public async Task<int> AwardRewardAsync(int userId, CancellationToken ct = default)
    {
        var rewardId = await _apiClient.PostAsync<object, int>(
            $"api/trivia/rewards/{userId}/award", new { }, ct);
        return rewardId;
    }

    public async Task<bool> RedeemRewardAsync(int rewardId, CancellationToken ct = default)
    {
        return await _apiClient.PostAsync<object, bool>(
            $"api/trivia/rewards/{rewardId}/redeem", new { }, ct);
    }
}

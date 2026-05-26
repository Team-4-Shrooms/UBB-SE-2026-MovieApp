using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy.Services;

public sealed class TriviaRepositoryProxy : ITriviaRepository
{
    private readonly ApiClient _apiClient;

    public TriviaRepositoryProxy(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IEnumerable<TriviaQuestion>> GetByCategoryAsync(
        string categoryName,
        CancellationToken cancellationToken = default)
    {
        var questions = await _apiClient.GetAsync<List<TriviaQuestion>>(
            $"api/trivia/category/{Uri.EscapeDataString(categoryName)}",
            cancellationToken);
        return questions ?? new List<TriviaQuestion>();
    }

    public async Task<IEnumerable<TriviaQuestion>> GetByMovieIdAsync(
        int movieIdentifier,
        int questionCount = ITriviaRepository.DefaultQuestionCount,
        CancellationToken cancellationToken = default)
    {
        var questions = await _apiClient.GetAsync<List<TriviaQuestion>>(
            $"api/trivia/movie/{movieIdentifier}?count={questionCount}",
            cancellationToken);
        return questions ?? new List<TriviaQuestion>();
    }
}

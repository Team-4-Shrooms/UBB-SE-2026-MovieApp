using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.Proxy.Services;

/// <summary>
/// Proxy implementation of <see cref="ITriviaRepository"/> that delegates to the WebApi
/// service endpoints. Used by desktop ViewModels that depend on ITriviaRepository directly
/// (e.g. MarathonTriviaViewModel).
/// </summary>
public sealed class TriviaRepositoryProxy : ITriviaRepository
{
    private readonly ApiClient _apiClient;

    public TriviaRepositoryProxy(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IEnumerable<TriviaQuestion>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var questions = await _apiClient.GetAsync<List<TriviaQuestion>>(
            "api/trivia/questions", cancellationToken);
        return questions ?? new List<TriviaQuestion>();
    }

    public async Task<IEnumerable<TriviaQuestion>> GetByCategoryAsync(
        string categoryName,
        CancellationToken cancellationToken = default)
    {
        var questions = await _apiClient.GetAsync<List<TriviaQuestion>>(
            $"api/trivia/questions/category/{Uri.EscapeDataString(categoryName)}",
            cancellationToken);
        return questions ?? new List<TriviaQuestion>();
    }

    public async Task<IEnumerable<TriviaQuestion>> GetByMovieIdAsync(
        int movieIdentifier,
        int questionCount = ITriviaRepository.DefaultQuestionCount,
        CancellationToken cancellationToken = default)
    {
        var questions = await _apiClient.GetAsync<List<TriviaQuestion>>(
            $"api/trivia/questions/movie/{movieIdentifier}",
            cancellationToken);
        return questions ?? new List<TriviaQuestion>();
    }
}

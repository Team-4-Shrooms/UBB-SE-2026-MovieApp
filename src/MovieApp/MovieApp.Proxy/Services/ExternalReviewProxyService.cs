using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services;

public class ExternalReviewProxyService : IExternalReviewService
{
    private readonly ApiClient _apiClient;

    public ExternalReviewProxyService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<CriticReview>> GetExternalReviewsAsync(string movieTitle, int releaseYear, CancellationToken cancellationToken = default)
    {
        var reviews = await _apiClient.GetAsync<List<CriticReview>>(
            $"api/movies/external-reviews/by-title?title={Uri.EscapeDataString(movieTitle)}&year={releaseYear}",
            cancellationToken);

        return reviews ?? new List<CriticReview>();
    }

    public List<(string Word, int Count)> AnalyseLexicon(List<CriticReview> reviews)
    {
        throw new NotSupportedException("Lexicon analysis is not supported over the HTTP proxy.");
    }
}

using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Options;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Models.DTOs;

namespace MovieApp.Logic.Services;

public sealed class GuardianReviewProvider : IExternalReviewProvider
{
    private readonly HttpClient _httpClient;
    private readonly ICacheService _cacheService;
    private readonly string _apiKey;

    public GuardianReviewProvider(HttpClient httpClient, ICacheService cacheService, IOptions<ExternalReviewsOptions> options)
    {
        _httpClient = httpClient;
        _cacheService = cacheService;
        _apiKey = options.Value.Guardian.ApiKey ?? string.Empty;
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey) && !string.Equals(_apiKey, "placeholder", StringComparison.OrdinalIgnoreCase);

    public async Task<CriticReview?> GetReviewAsync(string movieTitle, int releaseYear, CancellationToken ct = default)
    {
        if (!IsConfigured || string.IsNullOrWhiteSpace(movieTitle))
        {
            return null;
        }

        var query = Uri.EscapeDataString(movieTitle);
        var url = $"https://content.guardianapis.com/search?q={query}&section=film&tag=tone/reviews&show-fields=trailText&page-size=10&from-date={releaseYear}-01-01&to-date={releaseYear + 1}-12-31&api-key={_apiKey}";
        var cacheKey = BuildCacheKey("guardian", movieTitle, releaseYear);

        try
        {
            var json = await _cacheService.FetchOrCacheAsync(cacheKey, url, _httpClient, ct);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            var dto = JsonSerializer.Deserialize<GuardianApiResponseDto>(json);

            var result = dto?.Response?.Results?
                .OrderByDescending(r => MatchScore(movieTitle, releaseYear, r.WebTitle, r.Fields?.TrailText))
                .FirstOrDefault();

            if (result is null || MatchScore(movieTitle, releaseYear, result.WebTitle, result.Fields?.TrailText) <= 0)
            {
                return null;
            }

            return new CriticReview
            {
                Source = "The Guardian",
                Score = 0,
                Headline = result.WebTitle,
                Snippet = BuildLongerSnippet(result.Fields?.TrailText, movieTitle, releaseYear),
                Url = result.WebUrl,
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static string BuildCacheKey(string provider, string movieTitle, int releaseYear)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(movieTitle.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
        return $"{provider}_{sanitized}_{releaseYear}_v2".Replace(' ', '_').ToLowerInvariant();
    }

    private static int MatchScore(string movieTitle, int releaseYear, string? headline, string? snippet)
    {
        var text = $"{headline} {snippet}";
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        var score = 0;
        if (text.Contains(movieTitle, StringComparison.OrdinalIgnoreCase))
        {
            score += 10;
        }

        var tokens = movieTitle.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(t => t.Length > 2);

        score += tokens.Count(t => text.Contains(t, StringComparison.OrdinalIgnoreCase));

        if (text.Contains(releaseYear.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            score += 2;
        }

        return score;
    }

    private static string BuildLongerSnippet(string? trailText, string movieTitle, int releaseYear)
    {
        var baseSnippet = string.IsNullOrWhiteSpace(trailText)
            ? "The Guardian returned a matching film review article."
            : trailText.Trim();

        return $"{baseSnippet} This result was selected for '{movieTitle}' within the {releaseYear}-{releaseYear + 1} film-review window.";
    }
}

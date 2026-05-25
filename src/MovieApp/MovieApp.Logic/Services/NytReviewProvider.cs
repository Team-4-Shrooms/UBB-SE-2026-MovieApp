using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Options;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Models.DTOs;

namespace MovieApp.Logic.Services;

public sealed class NytReviewProvider : IExternalReviewProvider
{
    private readonly HttpClient _httpClient;
    private readonly ICacheService _cacheService;
    private readonly string _apiKey;
    private readonly string _omdbApiKey;

    public NytReviewProvider(HttpClient httpClient, ICacheService cacheService, IOptions<ExternalReviewsOptions> options)
    {
        _httpClient = httpClient;
        _cacheService = cacheService;
        _apiKey = options.Value.Nyt.ApiKey ?? string.Empty;
        _omdbApiKey = options.Value.Omdb.ApiKey ?? string.Empty;
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey) && !string.Equals(_apiKey, "placeholder", StringComparison.OrdinalIgnoreCase);

    public async Task<CriticReview?> GetReviewAsync(string movieTitle, int releaseYear, CancellationToken ct = default)
    {
        if (!IsConfigured || string.IsNullOrWhiteSpace(movieTitle))
        {
            return null;
        }

        try
        {
            var context = await GetOmdbContextAsync(movieTitle, releaseYear, ct);
            var query = Uri.EscapeDataString($"\"{context.Title}\" \"{context.Year}\" {context.Director}");
            var fq = Uri.EscapeDataString("type_of_material:(\"Review\") AND subject:(\"Movies\")");

            var url = $"https://api.nytimes.com/svc/search/v2/articlesearch.json?q={query}&fq={fq}&sort=relevance&api-key={_apiKey}";
            var cacheKey = BuildCacheKey("nyt_full", movieTitle, releaseYear);

            var json = await _cacheService.FetchOrCacheAsync(cacheKey, url, _httpClient, ct);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            var dto = JsonSerializer.Deserialize<NytApiResponseDto>(json);
            var doc = dto?.Response?.Docs?
                .Where(d => IsSpecificMovieReview(context.Title, context.Year, d.Headline?.Main, d.Snippet))
                .OrderByDescending(d => MatchScore(movieTitle, releaseYear, d.Headline?.Main, d.Snippet))
                .FirstOrDefault();

            if (doc is null)
            {
                return null;
            }

            return new CriticReview
            {
                Source = "New York Times",
                Score = 0,
                Headline = doc.Headline?.Main ?? string.Empty,
                Snippet = BuildLongerSnippet(doc.Snippet, context.Title, context.Year),
                Url = doc.WebUrl,
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<(string Title, int Year, string Director)> GetOmdbContextAsync(string movieTitle, int releaseYear, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_omdbApiKey) || string.Equals(_omdbApiKey, "placeholder", StringComparison.OrdinalIgnoreCase))
        {
            return (movieTitle, releaseYear, string.Empty);
        }

        var omdbUrl = $"https://www.omdbapi.com/?apikey={_omdbApiKey}&t={Uri.EscapeDataString(movieTitle)}&y={releaseYear}";
        var cacheKey = BuildCacheKey("nyt_omdb", movieTitle, releaseYear);

        try
        {
            var json = await _cacheService.FetchOrCacheAsync(cacheKey, omdbUrl, _httpClient, ct);
            if (string.IsNullOrWhiteSpace(json))
            {
                return (movieTitle, releaseYear, string.Empty);
            }

            var dto = JsonSerializer.Deserialize<OmdbContextDto>(json);
            if (dto is null)
            {
                return (movieTitle, releaseYear, string.Empty);
            }

            var year = int.TryParse(dto.Year, out var y) ? y : releaseYear;
            var dir = string.IsNullOrWhiteSpace(dto.Director) ? string.Empty : dto.Director.Split(',')[0].Trim();

            return (string.IsNullOrWhiteSpace(dto.Title) ? movieTitle : dto.Title, year, dir);
        }
        catch (Exception)
        {
            return (movieTitle, releaseYear, string.Empty);
        }
    }

    private static string BuildCacheKey(string provider, string movieTitle, int releaseYear)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(movieTitle.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
        return $"{provider}_{sanitized}_{releaseYear}_v3".Replace(' ', '_').ToLowerInvariant();
    }

    private static int MatchScore(string movieTitle, int releaseYear, string? headline, string? snippet)
    {
        var text = $"{headline} {snippet}";
        var score = text.Contains(movieTitle, StringComparison.OrdinalIgnoreCase) ? 10 : 0;
        if (text.Contains(releaseYear.ToString()))
        {
            score += 2;
        }

        return score;
    }

    private static bool IsSpecificMovieReview(string movieTitle, int releaseYear, string? headline, string? snippet)
    {
        var combined = $"{headline} {snippet}";
        if (string.IsNullOrWhiteSpace(combined))
        {
            return false;
        }

        return combined.Contains(movieTitle, StringComparison.OrdinalIgnoreCase) &&
               combined.Contains("review", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildLongerSnippet(string? snippet, string title, int year)
    {
        return $"{(snippet ?? "Matching review found.").Trim()} (Source: NYT review for '{title}' {year})";
    }
}

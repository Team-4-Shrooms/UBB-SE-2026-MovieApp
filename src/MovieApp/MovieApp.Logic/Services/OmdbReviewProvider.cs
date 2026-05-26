using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Options;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Models.DTOs;

namespace MovieApp.Logic.Services;

public sealed class OmdbReviewProvider : IExternalReviewProvider
{
    private readonly HttpClient _httpClient;
    private readonly ICacheService _cacheService;
    private readonly string _apiKey;

    public OmdbReviewProvider(HttpClient httpClient, ICacheService cacheService, IOptions<ExternalReviewsOptions> options)
    {
        _httpClient = httpClient;
        _cacheService = cacheService;
        _apiKey = options.Value.Omdb.ApiKey ?? string.Empty;
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey) && !string.Equals(_apiKey, "placeholder", StringComparison.OrdinalIgnoreCase);

    public async Task<CriticReview?> GetReviewAsync(string movieTitle, int releaseYear, CancellationToken ct = default)
    {
        if (!IsConfigured || string.IsNullOrWhiteSpace(movieTitle))
        {
            return null;
        }

        var url = $"https://www.omdbapi.com/?apikey={_apiKey}&t={Uri.EscapeDataString(movieTitle)}&y={releaseYear}";
        var cacheKey = BuildCacheKey("omdb", movieTitle, releaseYear);

        try
        {
            var json = await _cacheService.FetchOrCacheAsync(cacheKey, url, _httpClient, ct);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            var dto = JsonSerializer.Deserialize<OmdbResponseDto>(json);
            var firstRating = dto?.Ratings?.FirstOrDefault();

            if (firstRating is null)
            {
                return null;
            }

            return new CriticReview
            {
                Source = firstRating.Source,
                Score = ParseScore(firstRating.Value),
                Headline = $"{movieTitle} — OMDb rating",
                Snippet = BuildLongerSnippet(firstRating.Source, firstRating.Value, movieTitle, releaseYear),
                Url = $"https://www.omdbapi.com/?t={Uri.EscapeDataString(movieTitle)}",
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
        return $"{provider}_{sanitized}_{releaseYear}".Replace(' ', '_').ToLowerInvariant();
    }

    private static double ParseScore(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0;
        }

        var trimmed = value.Trim();

        if (trimmed.EndsWith('%') && double.TryParse(trimmed.TrimEnd('%'), NumberStyles.Number, CultureInfo.InvariantCulture, out var percent))
        {
            return Math.Round(Math.Clamp(percent / 20.0, 0, 5), 1);
        }

        if (trimmed.Contains('/'))
        {
            var parts = trimmed.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 &&
                double.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var num) &&
                double.TryParse(parts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var den) && den > 0)
            {
                return Math.Round(Math.Clamp((num / den) * 5.0, 0, 5), 1);
            }
        }

        return 0;
    }

    private static string BuildLongerSnippet(string source, string value, string movieTitle, int releaseYear)
    {
        return $"OMDb aggregated rating from {source}: {value}. This normalized score is used as an external critic signal for '{movieTitle}' ({releaseYear}).";
    }
}

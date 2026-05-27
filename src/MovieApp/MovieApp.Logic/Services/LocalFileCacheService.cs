using System.IO;
using System.Net.Http;
using System.Threading;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services;

public sealed class LocalFileCacheService : ICacheService
{
    private readonly string _cacheDirectory;

    public LocalFileCacheService()
    {
        _cacheDirectory = Path.Combine(AppContext.BaseDirectory, "ApiCache");
        Directory.CreateDirectory(_cacheDirectory);
    }

    public async Task<string> FetchOrCacheAsync(string cacheKey, string url, HttpClient client, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(cacheKey) || string.IsNullOrWhiteSpace(url) || client is null)
        {
            return string.Empty;
        }

        var cachePath = Path.Combine(_cacheDirectory, $"{cacheKey}.json");

        if (File.Exists(cachePath))
        {
            var age = DateTime.UtcNow - File.GetLastWriteTimeUtc(cachePath);
            if (age < TimeSpan.FromHours(24))
            {
                return await File.ReadAllTextAsync(cachePath, ct);
            }
        }

        try
        {
            using var response = await client.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            await File.WriteAllTextAsync(cachePath, json, ct);
            return json;
        }
        catch (HttpRequestException)
        {
            return string.Empty;
        }
        catch (TaskCanceledException)
        {
            return string.Empty;
        }
    }
}

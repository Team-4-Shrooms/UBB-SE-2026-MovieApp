namespace MovieApp.Logic.Interfaces.Services;
    public interface ICacheService
    {
        Task<string> FetchOrCacheAsync(string cacheKey, string url, HttpClient client, CancellationToken ct = default);
    }


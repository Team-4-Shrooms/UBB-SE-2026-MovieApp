using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApp.Proxy.Services;

public sealed class MarathonProxyService : IMarathonService
{
    private readonly ApiClient _apiClient;

    public MarathonProxyService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IEnumerable<Marathon>> GetWeeklyMarathonsAsync(int userIdentifier)
    {
        var marathons = await _apiClient.GetAsync<IEnumerable<Marathon>>($"api/marathons/weekly/{userIdentifier}");
        return marathons ?? new List<Marathon>();
    }

    public async Task<IEnumerable<Movie>> GetMoviesForMarathonAsync(int marathonId)
    {
        var movies = await _apiClient.GetAsync<IEnumerable<Movie>>($"api/marathons/{marathonId}");
        return movies ?? new List<Movie>();
    }

    public async Task<bool> StartMarathonAsync(int marathonIdentifier)
    {
        var success = await _apiClient.PostAsync<object, bool>(
            $"api/marathons/{marathonIdentifier}/start",
            new { });
        return success;
    }

    public async Task<MarathonProgress?> GetUserProgressAsync(int userId, int marathonId)
    {
        return await _apiClient.GetAsync<MarathonProgress>($"api/marathons/{marathonId}/progress/{userId}");
    }

    public Task<MarathonProgress?> GetCurrentProgressAsync(int marathonIdentifier)
    {
        return Task.FromResult<MarathonProgress?>(null);
    }

    public async Task UpdateQuizResultAsync(int marathonIdentifier, int correctAnswersCount)
    {
        await _apiClient.PostAsync<int>($"api/marathons/{marathonIdentifier}/quiz", correctAnswersCount);
    }

    public async Task<bool> LogMovieAsync(int marathonIdentifier, int movieIdentifier, int correctAnswersCount)
    {
        var success = await _apiClient.PostAsync<int, bool>(
            $"api/marathons/{marathonIdentifier}/movies/{movieIdentifier}/log",
            correctAnswersCount);
        return success;
    }

    public async Task<int> GetParticipantCountAsync(int marathonId)
    {
        return await _apiClient.GetAsync<int>($"api/marathons/{marathonId}/participants/count");
    }

    public async Task<int> GetMarathonMovieCountAsync(int marathonId)
    {
        return await _apiClient.GetAsync<int>($"api/marathons/{marathonId}/movies/count");
    }

    public async Task<bool> IsPrerequisiteCompletedAsync(int userId, int marathonId)
    {
        return await _apiClient.GetAsync<bool>($"api/marathons/{marathonId}/prerequisite/{userId}");
    }

    public async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync(int marathonId)
    {
        var leaderboard = await _apiClient.GetAsync<IEnumerable<LeaderboardEntry>>($"api/marathons/{marathonId}/leaderboard");
        return leaderboard ?? new List<LeaderboardEntry>();
    }

    public async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardWithUsernamesAsync(int marathonId)
    {
        return await GetLeaderboardAsync(marathonId);
    }
}

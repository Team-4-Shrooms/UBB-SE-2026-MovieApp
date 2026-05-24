namespace MovieApp.Logic.Interfaces.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

public interface IMarathonService
{
    Task<IEnumerable<Marathon>> GetWeeklyMarathonsAsync(int userIdentifier);

    Task<MarathonProgress?> GetCurrentProgressAsync(int marathonIdentifier);

    Task<bool> StartMarathonAsync(int marathonIdentifier);

    Task UpdateQuizResultAsync(int marathonIdentifier, int correctAnswersCount);

    Task<bool> LogMovieAsync(int marathonIdentifier, int movieIdentifier, int correctAnswersCount);

    Task<int> GetParticipantCountAsync(int marathonId);

    Task<int> GetMarathonMovieCountAsync(int marathonId);

    Task<bool> IsPrerequisiteCompletedAsync(int userId, int marathonId);

    Task<IEnumerable<Movie>> GetMoviesForMarathonAsync(int marathonId);

    Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync(int marathonId);

    Task<MarathonProgress?> GetUserProgressAsync(int userId, int marathonId);

    Task<IEnumerable<LeaderboardEntry>> GetLeaderboardWithUsernamesAsync(int marathonId);
}

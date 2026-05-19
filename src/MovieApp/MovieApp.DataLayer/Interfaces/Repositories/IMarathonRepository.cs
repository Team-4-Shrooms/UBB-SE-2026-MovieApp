using MovieApp.Core.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories;

using MovieApp.DataLayer.Models;

public interface IMarathonRepository
{
    Task<IEnumerable<Marathon>> GetActiveMarathonsAsync();

    Task<MarathonProgress?> GetUserProgressAsync(int userId, int marathonId);

    Task<bool> JoinMarathonAsync(int userId, int marathonId);

    Task<bool> UpdateProgressAsync(MarathonProgress progress);

    Task<IEnumerable<MarathonProgress>> GetLeaderboardAsync(int marathonId);

    Task<bool> IsPrerequisiteCompletedAsync(int userId, int prerequisiteMarathonId);

    Task<int> GetMarathonMovieCountAsync(int marathonId);

    Task<IEnumerable<Marathon>> GetWeeklyMarathonsForUserAsync(int userId, string weekString);

    Task AssignWeeklyMarathonsAsync(int userId, string weekString, int count = 10);

    Task<IEnumerable<MovieApp.DataLayer.Models.Movie>> GetMoviesForMarathonAsync(int marathonId);

    Task<IEnumerable<LeaderboardEntry>> GetLeaderboardWithUsernamesAsync(int marathonId);

    Task<int> GetParticipantCountAsync(int marathonId);
}

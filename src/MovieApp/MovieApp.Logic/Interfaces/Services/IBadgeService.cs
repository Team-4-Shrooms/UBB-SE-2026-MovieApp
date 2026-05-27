namespace MovieApp.Logic.Interfaces.Services;

using MovieApp.DataLayer.Models;

/// <summary>
/// Defines business logic operations for badge/achievement management and awarding.
/// </summary>
public interface IBadgeService
{
    /// <summary>
    /// Retrieves all badges earned by a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of badges the user has earned.</returns>
    Task<List<UserBadge>> GetUserBadgesAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all available badges defined in the system.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of all badges.</returns>
    Task<List<Badge>> GetAllBadgesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the leaderboard, containing user stats sorted by score.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of <see cref="UserStats"/> representing the leaderboard entries.</returns>
    Task<IList<UserStats>> GetLeaderboardAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Evaluates badge criteria for a user and persists any newly earned achievements.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to evaluate.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CheckAndAwardBadgesAsync(int userId, CancellationToken cancellationToken = default);
}

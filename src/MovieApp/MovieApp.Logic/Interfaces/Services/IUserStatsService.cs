namespace MovieApp.Logic.Interfaces.Services;

using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

/// <summary>
/// Defines business logic operations for retrieving user statistics.
/// </summary>
public interface IUserStatsService
{
    /// <summary>
    /// Retrieves statistics associated with a specific user identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>The user's stats, or <c>null</c> if no record exists.</returns>
    Task<UserStats?> GetByUserIdAsync(int userId, CancellationToken ct = default);
}

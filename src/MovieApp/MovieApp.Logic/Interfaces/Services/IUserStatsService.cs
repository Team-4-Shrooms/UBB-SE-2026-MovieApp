using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Interfaces.Services;

/// <summary>
/// Defines business logic operations for retrieving and managing user statistics.
/// </summary>
public interface IUserStatsService
{
    /// <summary>Retrieves statistics associated with a specific user identifier.</summary>
    Task<UserStats?> GetByUserIdAsync(int userId, CancellationToken ct = default);

    /// <summary>Retrieves all user statistics records.</summary>
    Task<List<UserStats>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Persists a new user stats record and returns the new identifier.</summary>
    Task<int> InsertAsync(UserStats userStats, CancellationToken ct = default);

    /// <summary>Updates an existing user stats record.</summary>
    Task<bool> UpdateAsync(UserStats userStats, CancellationToken ct = default);

    /// <summary>Removes a user stats record from the system.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    /// <summary>Retrieves all user statistics ordered by TotalPoints descending for leaderboard display.</summary>
    Task<IList<UserStats>> GetLeaderboardAsync(CancellationToken ct = default);
}

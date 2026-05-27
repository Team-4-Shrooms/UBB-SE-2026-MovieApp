using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// Defines a contract for accessing and managing user-badge assignments.
    /// </summary>
    public interface IUserBadgeRepository
    {
        /// <summary>Retrieves all badge assignments for a specific user.</summary>
        Task<List<UserBadge>> GetByUserIdAsync(int userId, CancellationToken ct = default);

        /// <summary>Returns true if the user already holds the specified badge.</summary>
        Task<bool> ExistsAsync(int userId, int badgeId, CancellationToken ct = default);

        /// <summary>Persists a new user-badge assignment and returns its identifier.</summary>
        Task<int> InsertAsync(UserBadge userBadge, CancellationToken ct = default);

        /// <summary>Removes a user-badge assignment by its identifier.</summary>
        Task<bool> DeleteAsync(int userBadgeId, CancellationToken ct = default);
    }
}

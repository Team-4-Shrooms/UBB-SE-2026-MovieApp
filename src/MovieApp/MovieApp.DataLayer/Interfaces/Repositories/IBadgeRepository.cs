using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories;
public interface IBadgeRepository
{
    /// <summary>Retrieves all available badges.</summary>
    Task<List<Badge>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Retrieves a badge by its unique identifier.</summary>
    Task<Badge?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Creates a new badge and returns its identifier.</summary>
    Task<int> InsertAsync(Badge badge, CancellationToken ct = default);

    /// <summary>Updates an existing badge definition.</summary>
    Task<bool> UpdateAsync(Badge badge, CancellationToken ct = default);

    /// <summary>Deletes a badge definition from the system.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    Task<List<Badge>> GetBadgesForUserAsync(int userId, CancellationToken ct = default);
}

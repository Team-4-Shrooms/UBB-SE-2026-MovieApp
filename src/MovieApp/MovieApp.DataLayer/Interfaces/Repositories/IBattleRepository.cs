using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories;
public interface IBattleRepository
{
    Task<List<Battle>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Battle?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> InsertAsync(Battle battle, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Battle battle, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories;
public interface IBattleRepository
{
    Task<List<Battle>> GetAllAsync(CancellationToken ct = default);
    Task<Battle?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> InsertAsync(Battle battle, CancellationToken ct = default);
    Task<bool> UpdateAsync(Battle battle, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

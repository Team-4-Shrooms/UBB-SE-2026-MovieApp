namespace MovieApp.DataLayer.Interfaces.Repositories;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

public interface IFavoriteEventRepository
{
    Task AddAsync(int userId, int eventId, CancellationToken cancellationToken = default);

    Task RemoveAsync(int userId, int eventId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FavoriteEvent>> FindByUserAsync(int userId, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int userId, int eventId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> GetUsersByFavoriteEventAsync(int eventId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FavoriteEvent>> FindByEventAsync(int eventId, CancellationToken cancellationToken = default);
}

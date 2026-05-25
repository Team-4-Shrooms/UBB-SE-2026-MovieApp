using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Features.Battles
{
    public interface IPointService
    {
        Task<UserStats> GetUserStatsAsync(int userId, CancellationToken cancellationToken = default);

        Task AddPointsAsync(int userId, int movieId, bool isBattleMovie, CancellationToken cancellationToken = default);

        Task DeductPointsAsync(int userId, int points, CancellationToken cancellationToken = default);

        Task FreezePointsAsync(int userId, int amount, CancellationToken cancellationToken = default);

        Task RefundPointsAsync(int userId, int amount, CancellationToken cancellationToken = default);

        Task UpdateWeeklyScoreAsync(int userId, CancellationToken cancellationToken = default);
    }
}

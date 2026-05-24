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
        Task<UserStats> GetUserStatsAsync(int userId, CancellationToken ct = default);

        Task AddPointsAsync(int userId, int movieId, bool isBattleMovie, CancellationToken ct = default);

        Task DeductPointsAsync(int userId, int points, CancellationToken ct = default);

        Task FreezePointsAsync(int userId, int amount, CancellationToken ct = default);

        Task RefundPointsAsync(int userId, int amount, CancellationToken ct = default);

        Task UpdateWeeklyScoreAsync(int userId, CancellationToken ct = default);
    }
}

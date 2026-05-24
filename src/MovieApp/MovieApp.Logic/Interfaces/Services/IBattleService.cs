using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Interfaces.Services;
public interface IBattleService
{
    Task<IEnumerable<Battle>> GetBattlesAsync(CancellationToken cancellationToken = default);
    Task<Battle?> GetBattleByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Battle?> GetActiveBattleAsync(CancellationToken cancellationToken = default);
    Task<Battle> CreateBattleAsync(int firstMovieId, int secondMovieId, CancellationToken cancellationToken = default);
    Task<BattleBet> PlaceBetAsync(int userId, int battleId, int movieId, int amount, CancellationToken cancellationToken = default);
    Task<BattleBet?> GetBetAsync(int userId, int battleId, CancellationToken cancellationToken = default);
    Task<int> DetermineWinnerAsync(int battleId, CancellationToken cancellationToken = default);
    Task DistributePayoutsAsync(int battleId, CancellationToken cancellationToken = default);
    Task SettleExpiredBattlesAsync(CancellationToken cancellationToken = default);
    Task<Battle?> GetCurrentBattleForUserAsync(int userId, CancellationToken cancellationToken = default);
    Task ForceSettleBattleAsync(int battleId, CancellationToken cancellationToken = default);
    Task ResetAllBattlesForDemoAsync(CancellationToken cancellationToken = default);
    Task<Battle> CreateDemoBattleAsync(CancellationToken cancellationToken = default);
}

using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Interfaces.Services;
public interface IBattleService
{
    Task<Battle?> GetActiveBattleAsync(CancellationToken ct = default);
    Task<Battle> CreateBattleAsync(int firstMovieId, int secondMovieId, CancellationToken ct = default);
    Task<BattleBet> PlaceBetAsync(int userId, int battleId, int movieId, int amount, CancellationToken ct = default);
    Task<BattleBet?> GetBetAsync(int userId, int battleId, CancellationToken ct = default);
    Task<int> DetermineWinnerAsync(int battleId, CancellationToken ct = default);
    Task DistributePayoutsAsync(int battleId, CancellationToken ct = default);
    Task SettleExpiredBattlesAsync(CancellationToken ct = default);
    Task<Battle?> GetCurrentBattleForUserAsync(int userId, CancellationToken ct = default);
    Task ForceSettleBattleAsync(int battleId, CancellationToken ct = default);
    Task ResetAllBattlesForDemoAsync(CancellationToken ct = default);
    Task<Battle> CreateDemoBattleAsync(CancellationToken ct = default);
}

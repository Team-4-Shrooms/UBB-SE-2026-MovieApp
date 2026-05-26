using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    public interface IBetRepository
    {
        Task<List<BattleBet>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<BattleBet?> GetByIdAsync(int userId, int battleId, CancellationToken cancellationToken = default);

        Task<bool> InsertAsync(BattleBet bet, CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(BattleBet bet, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(int userId, int battleId, CancellationToken cancellationToken = default);

        Task<bool> DeleteByBattleIdAsync(int battleId, CancellationToken cancellationToken = default);
    }
}

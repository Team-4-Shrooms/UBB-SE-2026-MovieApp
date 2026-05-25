using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class BetRepository : IBetRepository
    {
        private readonly IMovieAppDbContext _context;

        public BetRepository(IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BattleBet>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.BattleBets
                    .Include(battle => battle.User)
                    .Include(battle => battle.Battle)
                    .Include(battle => battle.Movie)
                    .ToListAsync(cancellationToken);
        }

        public async Task<BattleBet?> GetByIdAsync(int userId, int battleId, CancellationToken cancellationToken = default)
        {
            return await _context.BattleBets
                    .Include(battle => battle.User)
                    .Include(battle => battle.Battle)
                    .Include(battle => battle.Movie)
                    .FirstOrDefaultAsync(battle => battle.User != null && battle.User.Id == userId && battle.Battle != null && battle.Battle.BattleId == battleId, cancellationToken);
        }

        public async Task<bool> InsertAsync(BattleBet bet, CancellationToken cancellationToken = default)
        {
            _context.BattleBets.Add(bet);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> UpdateAsync(BattleBet bet, CancellationToken cancellationToken = default)
        {
            _context.BattleBets.Update(bet);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> DeleteAsync(int userId, int battleId, CancellationToken cancellationToken = default)
        {
            BattleBet? bet = await this.GetByIdAsync(userId, battleId, cancellationToken);
            if (bet == null)
            {
                return false;
            }
            _context.BattleBets.Remove(bet);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> DeleteByBattleIdAsync(int battleId, CancellationToken cancellationToken = default)
        {
            var battleBets = _context.BattleBets.Where(battle => battle.Battle != null && battle.Battle.BattleId == battleId);
            _context.BattleBets.RemoveRange(battleBets);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

    }
}

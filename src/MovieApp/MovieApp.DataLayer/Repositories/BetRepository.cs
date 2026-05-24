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

        public async Task<List<BattleBet>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.BattleBets
                    .Include(b => b.User)
                    .Include(b => b.Battle)
                    .Include(b => b.Movie)
                    .ToListAsync(ct);
        }

        public async Task<BattleBet?> GetByIdAsync(int userId, int battleId, CancellationToken ct = default)
        {
            return await _context.BattleBets
                    .Include(b => b.User)
                    .Include(b => b.Battle)
                    .Include(b => b.Movie)
                    .FirstOrDefaultAsync(b => b.User != null && b.User.Id == userId && b.Battle != null && b.Battle.BattleId == battleId, ct);
        }

        public async Task<bool> InsertAsync(BattleBet bet, CancellationToken ct = default)
        {
            _context.BattleBets.Add(bet);
            return await _context.SaveChangesAsync(ct) > 0;
        }

        public async Task<bool> UpdateAsync(BattleBet bet, CancellationToken ct = default)
        {
            _context.BattleBets.Update(bet);
            return await _context.SaveChangesAsync(ct) > 0;
        }

        public async Task<bool> DeleteAsync(int userId, int battleId, CancellationToken ct = default)
        {
            BattleBet? bet = await this.GetByIdAsync(userId, battleId, ct);
            if (bet == null)
            {
                return false;
            }
            _context.BattleBets.Remove(bet);
            return await _context.SaveChangesAsync(ct) > 0;
        }

        public async Task<bool> DeleteByBattleIdAsync(int battleId, CancellationToken ct = default)
        {
            var battleBets = _context.BattleBets.Where(b => b.Battle != null && b.Battle.BattleId == battleId);
            _context.BattleBets.RemoveRange(battleBets);
            return await _context.SaveChangesAsync(ct) > 0;
        }

    }
}

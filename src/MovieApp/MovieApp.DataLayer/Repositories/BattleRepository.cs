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
    public sealed class BattleRepository: IBattleRepository
    {
        private readonly IMovieAppDbContext _context;

        public BattleRepository(IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Battle>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Battles
                    .Include(b => b.FirstMovie)
                    .Include(b => b.SecondMovie)
                    .Include(b => b.Bets)
                        .ThenInclude(bet => bet.User)
                    .Include(b => b.Bets)
                        .ThenInclude(bet => bet.Movie)
                    .ToListAsync(ct);
        }

        public async Task<Battle?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Battles
                    .Include(b => b.FirstMovie)
                    .Include(b => b.SecondMovie)
                    .Include(b => b.Bets)
                        .ThenInclude(bet => bet.User)
                    .Include(b => b.Bets)
                        .ThenInclude(bet => bet.Movie)
                    .FirstOrDefaultAsync(b => b.BattleId == id, ct);
        }

        public async Task<int> InsertAsync(Battle battle, CancellationToken ct = default)
        {
            _context.Battles.Add(battle);
            await _context.SaveChangesAsync(ct);
            return battle.BattleId;
        }

        public async Task<bool> UpdateAsync(Battle battle, CancellationToken ct = default)
        {
            _context.Battles.Update(battle);
            return await _context.SaveChangesAsync(ct) > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            Battle? battle = await this.GetByIdAsync(id, ct);
            if (battle == null)
            {
                return false;
            }
            _context.Battles.Remove(battle);
            return await _context.SaveChangesAsync(ct) > 0;
        }
    }
}

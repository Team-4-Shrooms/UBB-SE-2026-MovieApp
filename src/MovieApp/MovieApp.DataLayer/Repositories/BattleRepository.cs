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

        public async Task<List<Battle>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Battles
                    .Include(battle => battle.FirstMovie)
                    .Include(battle => battle.SecondMovie)
                    .Include(battle => battle.Bets)
                        .ThenInclude(bet => bet.User)
                    .Include(battle => battle.Bets)
                        .ThenInclude(bet => bet.Movie)
                    .ToListAsync(cancellationToken);
        }

        public async Task<Battle?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Battles
                    .Include(battle => battle.FirstMovie)
                    .Include(battle => battle.SecondMovie)
                    .Include(battle => battle.Bets)
                        .ThenInclude(bet => bet.User)
                    .Include(battle => battle.Bets)
                        .ThenInclude(bet => bet.Movie)
                    .FirstOrDefaultAsync(battle => battle.BattleId == id, cancellationToken);
        }

        public async Task<int> InsertAsync(Battle battle, CancellationToken cancellationToken = default)
        {
            _context.Battles.Add(battle);
            await _context.SaveChangesAsync(cancellationToken);
            return battle.BattleId;
        }

        public async Task<bool> UpdateAsync(Battle battle, CancellationToken cancellationToken = default)
        {
            _context.Battles.Update(battle);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            Battle? battle = await this.GetByIdAsync(id, cancellationToken);
            if (battle == null)
            {
                return false;
            }
            _context.Battles.Remove(battle);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}

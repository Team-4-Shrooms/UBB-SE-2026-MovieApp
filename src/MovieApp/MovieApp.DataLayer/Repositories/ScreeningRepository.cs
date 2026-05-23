using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces;

namespace MovieApp.DataLayer.Repositories
{
    public class ScreeningRepository : IScreeningRepository
    {
        private readonly IMovieAppDbContext _context;

        public ScreeningRepository(IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task<Screening?> GetByIdAsync(int screeningId, CancellationToken cancellationToken = default)
        {
            return await ((DbContext)_context).Set<Screening>().FindAsync(new object[] { screeningId }, cancellationToken);
        }

        public async Task<IReadOnlyList<Screening>> GetByEventIdAsync(int eventIdentifier, CancellationToken cancellationToken = default)
        {
            return await ((DbContext)_context).Set<Screening>()
                .Where(s => s.EventId == eventIdentifier)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Screening>> GetByMovieIdAsync(int movieIdentifier, CancellationToken cancellationToken = default)
        {
            return await ((DbContext)_context).Set<Screening>()
                .Where(s => s.MovieId == movieIdentifier)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Screening screening, CancellationToken cancellationToken = default)
        {
            await ((DbContext)_context).Set<Screening>().AddAsync(screening, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

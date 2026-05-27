using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class TriviaRepository : ITriviaRepository
    {
        private readonly IMovieAppDbContext _context;

        public TriviaRepository(IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TriviaQuestion>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.TriviaQuestions
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<TriviaQuestion?> GetRandomAsync(CancellationToken cancellationToken = default)
        {
            int count = await _context.TriviaQuestions.CountAsync(cancellationToken);
            if (count == 0)
            {
                return null;
            }

            int skip = Random.Shared.Next(count);
            return await _context.TriviaQuestions
                .AsNoTracking()
                .Skip(skip)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TriviaQuestion?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.TriviaQuestions
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<TriviaQuestion>> GetByCategoryAsync(string categoryName, CancellationToken cancellationToken = default)
        {
            return await _context.TriviaQuestions
                .AsNoTracking()
                .Where(q => q.Category == categoryName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TriviaQuestion>> GetByMovieIdAsync(int movieIdentifier, int questionCount = ITriviaRepository.DefaultQuestionCount, CancellationToken cancellationToken = default)
        {
            return await _context.TriviaQuestions
                .AsNoTracking()
                .Where(q => q.MovieId == movieIdentifier)
                .Take(questionCount)
                .ToListAsync(cancellationToken);
        }
    }
}

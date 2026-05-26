using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<TriviaQuestion>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.TriviaQuestions
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TriviaQuestion>> GetByCategoryAsync(
            string categoryName,
            CancellationToken cancellationToken = default)
        {
            return await _context.TriviaQuestions
                .AsNoTracking()
                .Where(question => question.Category == categoryName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TriviaQuestion>> GetByMovieIdAsync(
            int movieIdentifier,
            int questionCount = ITriviaRepository.DefaultQuestionCount,
            CancellationToken cancellationToken = default)
        {
            return await _context.TriviaQuestions
                .AsNoTracking()
                .Where(question => question.MovieId == movieIdentifier)
                .Take(questionCount)
                .ToListAsync(cancellationToken);
        }
    }
}

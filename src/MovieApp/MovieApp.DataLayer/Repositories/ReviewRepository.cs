using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class ReviewRepository : IReviewRepository
    {
        private readonly MovieApp.DataLayer.Interfaces.IMovieAppDbContext _context;

        public ReviewRepository(MovieApp.DataLayer.Interfaces.IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MovieReview>> GetReviewsForMovieAsync(int movieId)
        {
            return await _context.MovieReviews
                .AsNoTracking()
                .Include(review => review.Movie)
                .Include(review => review.User)
                .Where(review => review.Movie.Id == movieId)
                .OrderByDescending(review => review.CreatedAt)
                .ThenByDescending(review => review.Id)
                .ToListAsync();
        }

        public async Task<List<decimal>> GetRawRatingsForMovieAsync(int movieId) =>
            await _context.MovieReviews.Where(r => r.Movie.Id == movieId).Select(r => r.StarRating).ToListAsync();

        public async Task<Dictionary<int, int>> GetReviewCountsAsync(IEnumerable<int> movieIds) =>
            await _context.MovieReviews.Where(r => movieIds.Contains(r.Movie.Id))
                .GroupBy(r => r.Movie.Id).Select(g => new { Id = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.Count);

        public async Task AddReviewAsync(MovieReview review) => await _context.MovieReviews.AddAsync(review);
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<int> GetReviewCountByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _context.Reviews
                .AsNoTracking()
                .CountAsync(review => review.User != null && review.User.Id == userId, ct);
        }

        public async Task<List<Review>> GetReviewsByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _context.Reviews
                .AsNoTracking()
                .Include(review => review.Movie)
                    .ThenInclude(movie => movie.Genres)
                .Include(review => review.User)
                .Where(review => review.User != null && review.User.Id == userId)
                .ToListAsync(ct);
        }
    }
}


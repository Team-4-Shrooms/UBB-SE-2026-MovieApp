using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// A repository interface for managing movie reviews.
    /// </summary>
    public interface IReviewRepository
    {
        Task<List<MovieReview>> GetReviewsForMovieAsync(int movieId);
        Task<List<decimal>> GetRawRatingsForMovieAsync(int movieId);
        Task<Dictionary<int, int>> GetReviewCountsAsync(IEnumerable<int> movieIds);
        Task AddReviewAsync(MovieReview review);
        Task<int> SaveChangesAsync();

        /// <summary>Returns the number of reviews written by a specific user.</summary>
        Task<int> GetReviewCountByUserIdAsync(int userId, CancellationToken ct = default);

        /// <summary>Returns all reviews written by a specific user, with Movie and Genres loaded.</summary>
        Task<List<Review>> GetReviewsByUserIdAsync(int userId, CancellationToken ct = default);
    }
}


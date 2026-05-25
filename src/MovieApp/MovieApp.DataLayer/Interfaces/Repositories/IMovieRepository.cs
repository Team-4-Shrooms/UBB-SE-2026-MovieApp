using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    /// <summary>
    /// Defines a contract for accessing and managing movie data, including retrieval, search, ownership checks, and
    /// purchase operations.
    /// </summary>
    public interface IMovieRepository
    {
        Task<Movie?> GetMovieByIdAsync(int movieId);
        Task<List<Movie>> GetAllMoviesAsync();
        Task<List<Movie>> SearchMoviesAsync(string query, int limit);
        Task<bool> UserOwnsMovieAsync(int userId, int movieId);
        Task AddOwnedMovieAsync(OwnedMovie ownership);
        Task AddTransactionAsync(Transaction transaction);
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Retrieves all available genres.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A read-only list of genre entities.</returns>
        Task<IReadOnlyList<Genre>> GetGenresAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all available actors.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A read-only list of actor entities.</returns>
        Task<IReadOnlyList<Actor>> GetActorsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all available directors.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A read-only list of director entities.</returns>
        Task<IReadOnlyList<Director>> GetDirectorsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns movies that match all of the given criteria (AND logic).
        /// </summary>
        /// <param name="genreIdentifier">The unique identifier of the genre.</param>
        /// <param name="actorIdentifier">The unique identifier of the actor.</param>
        /// <param name="directorIdentifier">The unique identifier of the director.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A read-only list of movies matching the criteria.</returns>
        Task<IReadOnlyList<Movie>> FindMoviesByCriteriaAsync(int genreIdentifier, int actorIdentifier, int directorIdentifier, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns movies that match any of the given criteria (OR logic).
        /// </summary>
        /// <param name="genreIdentifier">The unique identifier of the genre.</param>
        /// <param name="actorIdentifier">The unique identifier of the actor.</param>
        /// <param name="directorIdentifier">The unique identifier of the director.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A read-only list of movies matching any of the criteria.</returns>
        Task<IReadOnlyList<Movie>> FindMoviesByAnyCriteriaAsync(int genreIdentifier, int actorIdentifier, int directorIdentifier, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the identifiers of events where a specific movie is being screened.
        /// </summary>
        /// <param name="movieIdentifier">The unique identifier of the movie.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A read-only list of event identifiers.</returns>
        Task<IReadOnlyList<int>> FindScreeningEventIdsForMovieAsync(int movieIdentifier, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns all valid (Genre, Actor, Director) combinations from movies
        /// that have at least one screening in a future event.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A read-only list of valid reel combinations.</returns>
        Task<IReadOnlyList<ReelCombination>> GetValidReelCombinationsAsync(CancellationToken cancellationToken = default);
    }
}


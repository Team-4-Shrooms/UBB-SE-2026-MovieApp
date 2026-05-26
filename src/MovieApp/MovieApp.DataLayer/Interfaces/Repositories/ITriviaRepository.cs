using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories;
public interface ITriviaRepository
{
    /// <summary>
    /// The default number of questions to retrieve for a movie-specific quiz.
    /// </summary>
    public const int DefaultQuestionCount = 3;

    /// <summary>
    /// Retrieves all trivia questions.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>All trivia questions in the store.</returns>
    Task<IEnumerable<TriviaQuestion>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single trivia question at random.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A randomly selected trivia question, or null if the store is empty.</returns>
    Task<TriviaQuestion?> GetRandomAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single trivia question by its identifier.
    /// </summary>
    /// <param name="id">The question identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The matching trivia question, or null if not found.</returns>
    Task<TriviaQuestion?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves trivia questions belonging to a specific category.
    /// </summary>
    /// <param name="categoryName">The name of the trivia category.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of trivia questions matching the category.</returns>
    Task<IEnumerable<TriviaQuestion>> GetByCategoryAsync(string categoryName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific number of trivia questions linked to a movie.
    /// </summary>
    /// <param name="movieIdentifier">The unique identifier of the movie.</param>
    /// <param name="questionCount">The number of questions to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of movie-specific trivia questions.</returns>
    Task<IEnumerable<TriviaQuestion>> GetByMovieIdAsync(int movieIdentifier, int questionCount = DefaultQuestionCount, CancellationToken cancellationToken = default);
}

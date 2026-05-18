using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Interfaces.Services;

/// <summary>
/// Defines business logic operations for trivia questions and rewards.
/// </summary>
public interface ITriviaService
{
    /// <summary>
    /// Retrieves all trivia questions.
    /// </summary>
    Task<List<TriviaQuestion>> GetAllQuestionsAsync(CancellationToken ct = default);

    /// <summary>
    /// Retrieves trivia questions for a specific movie.
    /// </summary>
    Task<List<TriviaQuestion>> GetQuestionsByMovieIdAsync(int movieId, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a trivia question by its unique identifier.
    /// </summary>
    Task<TriviaQuestion?> GetQuestionByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Retrieves all trivia rewards for a specific user.
    /// </summary>
    Task<List<TriviaReward>> GetRewardsByUserIdAsync(int userId, CancellationToken ct = default);

    /// <summary>
    /// Awards a new trivia reward to a user and returns its identifier.
    /// </summary>
    Task<int> AwardRewardAsync(int userId, CancellationToken ct = default);

    /// <summary>
    /// Redeems a trivia reward for a user.
    /// </summary>
    Task<bool> RedeemRewardAsync(int rewardId, CancellationToken ct = default);
}

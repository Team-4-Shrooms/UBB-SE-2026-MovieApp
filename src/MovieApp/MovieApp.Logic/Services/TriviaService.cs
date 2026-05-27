using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services
{
    public class TriviaService : ITriviaService
    {
        private readonly ITriviaRepository _triviaRepository;
        private readonly ITriviaRewardRepository _triviaRewardRepository;

        public TriviaService(
            ITriviaRepository triviaRepository,
            ITriviaRewardRepository triviaRewardRepository)
        {
            _triviaRepository = triviaRepository;
            _triviaRewardRepository = triviaRewardRepository;
        }

        /// <inheritdoc />
        public async Task<List<TriviaQuestion>> GetAllQuestionsAsync(
            CancellationToken cancellationToken = default)
        {
            var questions = await _triviaRepository.GetAllAsync(cancellationToken);
            return questions.ToList();
        }

        /// <inheritdoc />
        public async Task<List<TriviaQuestion>> GetQuestionsByCategoryAsync(
            string category,
            CancellationToken cancellationToken = default)
        {
            var questions = await _triviaRepository.GetByCategoryAsync(category, cancellationToken);
            return questions.ToList();
        }

        /// <inheritdoc />
        public async Task<List<TriviaQuestion>> GetQuestionsByMovieIdAsync(
            int movieId,
            CancellationToken cancellationToken = default)
        {
            var questions = await _triviaRepository.GetByMovieIdAsync(
                movieId,
                cancellationToken: cancellationToken);
            return questions.ToList();
        }

        /// <inheritdoc />
        public async Task<TriviaQuestion?> GetQuestionByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _triviaRepository.GetByIdAsync(id, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<List<TriviaReward>> GetRewardsByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            var reward = await _triviaRewardRepository.GetUnredeemedByUserAsync(userId, cancellationToken);
            return reward is null
                ? new List<TriviaReward>()
                : new List<TriviaReward> { reward };
        }

        /// <inheritdoc />
        public async Task<int> AwardRewardAsync(int userId, CancellationToken cancellationToken = default)
        {
            var reward = new TriviaReward
            {
                UserId = userId,
                IsRedeemed = false,
                CreatedAt = DateTime.UtcNow,
            };
            await _triviaRewardRepository.AddAsync(reward, cancellationToken);
            return reward.Id;
        }

        /// <inheritdoc />
        public async Task<bool> RedeemRewardAsync(
            int rewardId,
            CancellationToken cancellationToken = default)
        {
            await _triviaRewardRepository.MarkAsRedeemedAsync(rewardId, cancellationToken);
            return true;
        }
    }
}

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
        public async Task<List<TriviaQuestion>> GetAllQuestionsAsync(CancellationToken ct = default)
        {
            var questions = await _triviaRepository.GetAllAsync(ct);
            return questions.ToList();
        }

        /// <inheritdoc />
        public async Task<List<TriviaQuestion>> GetQuestionsByCategoryAsync(
            string category,
            CancellationToken ct = default)
        {
            var questions = await _triviaRepository.GetByCategoryAsync(category, ct);
            return questions.ToList();
        }

        /// <inheritdoc />
        public async Task<List<TriviaQuestion>> GetQuestionsByMovieIdAsync(
            int movieId,
            CancellationToken ct = default)
        {
            var questions = await _triviaRepository.GetByMovieIdAsync(movieId, cancellationToken: ct);
            return questions.ToList();
        }

        /// <inheritdoc />
        public async Task<TriviaQuestion?> GetQuestionByIdAsync(int id, CancellationToken ct = default)
        {
            var all = await _triviaRepository.GetAllAsync(ct);
            return all.FirstOrDefault(question => question.Id == id);
        }

        /// <inheritdoc />
        public async Task<List<TriviaReward>> GetRewardsByUserIdAsync(int userId, CancellationToken ct = default)
        {
            var reward = await _triviaRewardRepository.GetUnredeemedByUserAsync(userId, ct);
            return reward is null
                ? new List<TriviaReward>()
                : new List<TriviaReward> { reward };
        }

        /// <inheritdoc />
        public async Task<int> AwardRewardAsync(int userId, CancellationToken ct = default)
        {
            var reward = new TriviaReward
            {
                UserId = userId,
                IsRedeemed = false,
                CreatedAt = DateTime.UtcNow,
            };
            await _triviaRewardRepository.AddAsync(reward, ct);
            return reward.Id;
        }

        /// <inheritdoc />
        public async Task<bool> RedeemRewardAsync(int rewardId, CancellationToken ct = default)
        {
            await _triviaRewardRepository.MarkAsRedeemedAsync(rewardId, ct);
            return true;
        }
    }
}

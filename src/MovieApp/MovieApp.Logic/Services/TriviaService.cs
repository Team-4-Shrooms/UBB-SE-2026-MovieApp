using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services
{
    public sealed class TriviaService : ITriviaService
    {
        private readonly ITriviaRepository _triviaRepository;
        private readonly ITriviaRewardRepository _rewardRepository;

        public TriviaService(ITriviaRepository triviaRepository, ITriviaRewardRepository rewardRepository)
        {
            _triviaRepository = triviaRepository;
            _rewardRepository = rewardRepository;
        }

        public async Task<List<TriviaQuestion>> GetAllQuestionsAsync(CancellationToken ct = default)
        {
            IEnumerable<TriviaQuestion> questions = await _triviaRepository.GetAllAsync(ct);
            return new List<TriviaQuestion>(questions);
        }

        public async Task<List<TriviaQuestion>> GetQuestionsByMovieIdAsync(int movieId, CancellationToken ct = default)
        {
            IEnumerable<TriviaQuestion> questions = await _triviaRepository.GetByMovieIdAsync(movieId, cancellationToken: ct);
            return new List<TriviaQuestion>(questions);
        }

        public async Task<TriviaQuestion?> GetQuestionByIdAsync(int id, CancellationToken ct = default)
        {
            return await _triviaRepository.GetByIdAsync(id, ct);
        }

        public async Task<List<TriviaReward>> GetRewardsByUserIdAsync(int userId, CancellationToken ct = default)
        {
            TriviaReward? reward = await _rewardRepository.GetUnredeemedByUserAsync(userId, ct);
            if (reward is null)
            {
                return new List<TriviaReward>();
            }

            return new List<TriviaReward> { reward };
        }

        public async Task<int> AwardRewardAsync(int userId, CancellationToken ct = default)
        {
            TriviaReward reward = new TriviaReward
            {
                UserId = userId,
                IsRedeemed = false,
                CreatedAt = DateTime.UtcNow,
            };

            await _rewardRepository.AddAsync(reward, ct);
            return reward.Id;
        }

        public async Task<bool> RedeemRewardAsync(int rewardId, CancellationToken ct = default)
        {
            await _rewardRepository.MarkAsRedeemedAsync(rewardId, ct);
            return true;
        }
    }
}

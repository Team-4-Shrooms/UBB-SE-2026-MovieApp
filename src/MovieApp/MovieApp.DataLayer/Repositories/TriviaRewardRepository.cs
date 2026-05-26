using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class TriviaRewardRepository : ITriviaRewardRepository
    {
        private readonly IMovieAppDbContext _context;

        public TriviaRewardRepository(IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TriviaReward reward, CancellationToken cancellationToken = default)
        {
            _context.TriviaRewards.Add(reward);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<TriviaReward?> GetUnredeemedByUserAsync(int userIdentifier, CancellationToken cancellationToken = default)
        {
            return await _context.TriviaRewards
                .AsNoTracking()
                .Where(r => r.UserId == userIdentifier && !r.IsRedeemed)
                .OrderBy(r => r.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task MarkAsRedeemedAsync(int rewardIdentifier, CancellationToken cancellationToken = default)
        {
            TriviaReward? reward = await _context.TriviaRewards
                .FirstOrDefaultAsync(r => r.Id == rewardIdentifier, cancellationToken);

            if (reward is null)
            {
                return;
            }

            reward.Redeem();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

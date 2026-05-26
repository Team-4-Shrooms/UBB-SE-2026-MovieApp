using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<TriviaReward?> GetUnredeemedByUserAsync(
            int userIdentifier,
            CancellationToken cancellationToken = default)
        {
            return await _context.TriviaRewards
                .Where(reward => reward.UserId == userIdentifier && !reward.IsRedeemed)
                .OrderBy(reward => reward.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task MarkAsRedeemedAsync(
            int rewardIdentifier,
            CancellationToken cancellationToken = default)
        {
            var reward = await _context.TriviaRewards
                .FirstOrDefaultAsync(reward => reward.Id == rewardIdentifier, cancellationToken);

            if (reward is not null)
            {
                reward.Redeem();
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}

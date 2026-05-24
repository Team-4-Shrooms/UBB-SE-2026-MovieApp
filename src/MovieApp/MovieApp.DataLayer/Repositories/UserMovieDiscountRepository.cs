using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class UserMovieDiscountRepository: IUserMovieDiscountRepository
    {
        private readonly IMovieAppDbContext context;

        public UserMovieDiscountRepository(IMovieAppDbContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task AddAsync(Reward reward, CancellationToken ct = default)
        {
            this.context.Rewards.Add(reward);
            await this.context.SaveChangesAsync(ct);
        }

        /// <inheritdoc/>
        public async Task<List<Reward>> GetDiscountsForUserAsync(int userIdentifier, CancellationToken ct = default)
        {
            return await this.context.Rewards
                .Where(r => r.OwnerUserId == userIdentifier)
                .ToListAsync(ct);
        }

        /// <inheritdoc/>
        public async Task MarkRedeemedAsync(int rewardIdentifier, CancellationToken ct = default)
        {
            var reward = await this.context.Rewards.FindAsync(new object[] { rewardIdentifier }, ct);
            if (reward != null)
            {
                reward.Redeem();
                await this.context.SaveChangesAsync(ct);
            }
        }
    }
}

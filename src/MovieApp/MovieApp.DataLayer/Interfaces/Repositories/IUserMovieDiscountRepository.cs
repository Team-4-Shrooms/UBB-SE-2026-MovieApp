using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories
{
    public interface IUserMovieDiscountRepository
    {
        Task AddAsync(Reward reward, CancellationToken cancellationToken = default);

        Task<List<Reward>> GetDiscountsForUserAsync(int userIdentifier, CancellationToken cancellationToken = default);

        Task MarkRedeemedAsync(int rewardIdentifier, CancellationToken cancellationToken = default);
    }
}

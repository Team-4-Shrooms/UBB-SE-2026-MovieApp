using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Services
{
    public class BadgeService : IBadgeService
    {
        public Task CheckAndAwardBadgesAsync(int userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<Badge>> GetAllBadgesAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserBadge>> GetUserBadgesAsync(int userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}

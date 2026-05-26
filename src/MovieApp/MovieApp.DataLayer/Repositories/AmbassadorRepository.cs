namespace MovieApp.DataLayer.Repositories
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using MovieApp.DataLayer.Interfaces;
    using MovieApp.DataLayer.Interfaces.Repositories;
    using MovieApp.DataLayer.Models;

    public sealed class AmbassadorRepository : IAmbassadorRepository
    {
        private readonly IMovieAppDbContext _context;

        public AmbassadorRepository(IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsReferralCodeValidAsync(string referralCode, CancellationToken cancellationToken = default)
        {
            return await _context.AmbassadorProfiles
                .AnyAsync(ambassadorProfile => ambassadorProfile.PermanentCode == referralCode, cancellationToken);
        }

        public async Task<string?> GetReferralCodeAsync(int userId, CancellationToken cancellationToken = default)
        {
            AmbassadorProfile? profile = await _context.AmbassadorProfiles
                .FirstOrDefaultAsync(ambassadorProfile => ambassadorProfile.UserId == userId, cancellationToken);
            return profile?.PermanentCode;
        }

        public async Task CreateAmbassadorProfileAsync(int userId, string referralCode, CancellationToken cancellationToken = default)
        {
            AmbassadorProfile profile = new AmbassadorProfile
            {
                UserId = userId,
                PermanentCode = referralCode,
                RewardBalance = 0
            };
            await _context.AmbassadorProfiles.AddAsync(profile, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int?> GetUserIdByReferralCodeAsync(string referralCode, CancellationToken cancellationToken = default)
        {
            AmbassadorProfile? profile = await _context.AmbassadorProfiles
                .FirstOrDefaultAsync(ambassadorProfile => ambassadorProfile.PermanentCode == referralCode, cancellationToken);
            return profile?.UserId;
        }

        public async Task AddReferralLogAsync(int ambassadorId, int friendId, int eventId, CancellationToken cancellationToken = default)
        {
            ReferralLog log = new ReferralLog
            {
                AmbassadorId = ambassadorId,
                ReferredUserId = friendId,
                EventId = eventId
            };
            await _context.ReferralLogs.AddAsync(log, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> HasReferralLogAsync(int ambassadorId, int friendId, int eventId, CancellationToken cancellationToken = default)
        {
            return await _context.ReferralLogs
                .AnyAsync(
                    referralLog => referralLog.AmbassadorId == ambassadorId &&
                                   referralLog.ReferredUserId == friendId &&
                                   referralLog.EventId == eventId,
                    cancellationToken);
        }

        public async Task<IEnumerable<AmbassadorProfile>> GetAllAmbassadorsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.AmbassadorProfiles
                .ToListAsync(cancellationToken);
        }

        public async Task<AmbassadorProfile?> GetAmbassadorByIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.AmbassadorProfiles
                .FirstOrDefaultAsync(ambassadorProfile => ambassadorProfile.UserId == userId, cancellationToken);
        }

        public async Task<int> GetRewardBalanceAsync(int userId, CancellationToken cancellationToken = default)
        {
            AmbassadorProfile? profile = await _context.AmbassadorProfiles
                .FirstOrDefaultAsync(ambassadorProfile => ambassadorProfile.UserId == userId, cancellationToken);
            return profile?.RewardBalance ?? 0;
        }

        public async Task<IEnumerable<ReferralHistoryItem>> GetReferralHistoryAsync(int ambassadorId, CancellationToken cancellationToken = default)
        {
            return await _context.ReferralLogs
                .Where(referralLog => referralLog.AmbassadorId == ambassadorId)
                .Include(referralLog => referralLog.ReferredUser)
                .Include(referralLog => referralLog.Event)
                .Select(referralLog => new ReferralHistoryItem
                {
                    FriendName = referralLog.ReferredUser != null ? referralLog.ReferredUser.Username : "Unknown",
                    EventTitle = referralLog.Event != null ? referralLog.Event.Title : "Deleted Event",
                    UsedAt = referralLog.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task IncrementRewardBalanceAsync(int ambassadorId, CancellationToken cancellationToken = default)
        {
            AmbassadorProfile? profile = await _context.AmbassadorProfiles
                .FirstOrDefaultAsync(ambassadorProfile => ambassadorProfile.UserId == ambassadorId, cancellationToken);
            if (profile != null)
            {
                profile.RewardBalance++;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DecrementRewardBalanceAsync(int userId, CancellationToken cancellationToken = default)
        {
            AmbassadorProfile? profile = await _context.AmbassadorProfiles
                .FirstOrDefaultAsync(ambassadorProfile => ambassadorProfile.UserId == userId, cancellationToken);
            if (profile != null && profile.RewardBalance > 0)
            {
                profile.RewardBalance--;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}

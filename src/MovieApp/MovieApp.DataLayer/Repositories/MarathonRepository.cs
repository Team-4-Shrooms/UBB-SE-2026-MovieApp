using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class MarathonRepository : IMarathonRepository
    {
        private readonly IMovieAppDbContext _context;

        public MarathonRepository(IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Marathon>> GetActiveMarathonsAsync()
        {
            List<Marathon> activeMarathons = await _context.Marathons
                .Where(marathon => marathon.IsActive)
                .ToListAsync();
            return activeMarathons;
        }

        public async Task<MarathonProgress?> GetUserProgressAsync(int userId, int marathonId)
        {
            MarathonProgress? progress = await _context.MarathonProgressions
                .FirstOrDefaultAsync(entry => entry.UserId == userId && entry.MarathonId == marathonId);
            return progress;
        }

        public async Task<bool> JoinMarathonAsync(int userId, int marathonId)
        {
            bool alreadyJoined = await _context.MarathonProgressions
                .AnyAsync(entry => entry.UserId == userId && entry.MarathonId == marathonId);

            if (alreadyJoined)
            {
                return false;
            }

            MarathonProgress newProgress = new MarathonProgress
            {
                UserId = userId,
                MarathonId = marathonId,
                JoinedAt = DateTime.UtcNow,
                TriviaAccuracy = 0,
                CompletedMoviesCount = 0
            };

            await _context.MarathonProgressions.AddAsync(newProgress);
            int savedCount = await _context.SaveChangesAsync();
            return savedCount > 0;
        }

        public async Task<bool> UpdateProgressAsync(MarathonProgress progress)
        {
            _context.MarathonProgressions.Update(progress);
            int savedCount = await _context.SaveChangesAsync();
            return savedCount > 0;
        }

        public async Task<IEnumerable<MarathonProgress>> GetLeaderboardAsync(int marathonId)
        {
            List<MarathonProgress> leaderboard = await _context.MarathonProgressions
                .Where(entry => entry.MarathonId == marathonId)
                .OrderByDescending(entry => entry.TriviaAccuracy)
                .ThenByDescending(entry => entry.CompletedMoviesCount)
                .ToListAsync();
            return leaderboard;
        }

        public async Task<bool> IsPrerequisiteCompletedAsync(int userId, int prerequisiteMarathonId)
        {
            bool isCompleted = await _context.MarathonProgressions
                .AnyAsync(entry => entry.UserId == userId
                    && entry.MarathonId == prerequisiteMarathonId
                    && entry.FinishedAt != null);
            return isCompleted;
        }

        public async Task<int> GetMarathonMovieCountAsync(int marathonId)
        {
            Marathon? marathon = await _context.Marathons
                .Include(m => m.Movies)
                .FirstOrDefaultAsync(m => m.Id == marathonId);

            return marathon?.Movies?.Count ?? 0;
        }

        public async Task<IEnumerable<Marathon>> GetWeeklyMarathonsForUserAsync(int userId, string weekString)
        {
            List<int> joinedIds = await _context.MarathonProgressions
                .Where(p => p.UserId == userId)
                .Select(p => p.MarathonId)
                .ToListAsync();

            if (joinedIds.Count == 0)
                return Enumerable.Empty<Marathon>();

            return await _context.Marathons
                .Where(m => joinedIds.Contains(m.Id))
                .ToListAsync();
        }

        public async Task AssignWeeklyMarathonsAsync(int userId, string weekString, int count = 10)
        {
            List<int> joinedIds = await _context.MarathonProgressions
                .Where(p => p.UserId == userId)
                .Select(p => p.MarathonId)
                .ToListAsync();

            List<Marathon> availableMarathons = await _context.Marathons
                .Where(m => m.IsActive && !joinedIds.Contains(m.Id))
                .Take(count)
                .ToListAsync();

            foreach (Marathon marathon in availableMarathons)
            {
                await JoinMarathonAsync(userId, marathon.Id);
            }
        }

        public async Task<IEnumerable<Movie>> GetMoviesForMarathonAsync(int marathonId)
        {
            Marathon? marathon = await _context.Marathons
                .Include(m => m.Movies)
                .FirstOrDefaultAsync(m => m.Id == marathonId);

            return marathon?.Movies ?? Enumerable.Empty<Movie>();
        }

        public async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardWithUsernamesAsync(int marathonId)
        {
            List<LeaderboardEntry> leaderboard = await _context.MarathonProgressions
                .Where(entry => entry.MarathonId == marathonId)
                .Include(entry => entry.User)
                .OrderByDescending(entry => entry.CompletedMoviesCount)
                .ThenByDescending(entry => entry.TriviaAccuracy)
                .Select(entry => new LeaderboardEntry
                {
                    UserId = entry.UserId,
                    Username = entry.User != null ? entry.User.Username : "Unknown User",
                    CompletedMoviesCount = entry.CompletedMoviesCount,
                    TriviaAccuracy = entry.TriviaAccuracy,
                    FinishedAt = entry.FinishedAt
                })
                .ToListAsync();
            return leaderboard;
        }

        public async Task<int> GetParticipantCountAsync(int marathonId)
        {
            int participantCount = await _context.MarathonProgressions
                .CountAsync(entry => entry.MarathonId == marathonId);
            return participantCount;
        }
    }
}

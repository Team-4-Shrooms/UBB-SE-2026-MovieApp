using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Features.Battles
{
    public class PointService: IPointService
    {
        private readonly IUserStatsRepository _userStatsRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IBadgeService _badgeService;

        public PointService(
        IUserStatsRepository userStatsRepository,
        IUserRepository userRepository,
        IMovieRepository movieRepository,
        IBadgeService badgeService)
        {
            _userStatsRepository = userStatsRepository;
            _userRepository = userRepository;
            _movieRepository = movieRepository;
            _badgeService = badgeService;
        }

        /// <inheritdoc/>
        public async Task<UserStats> GetUserStatsAsync(int userId, CancellationToken cancellationToken = default)
        {
            var stats = await _userStatsRepository.GetByUserIdAsync(userId, cancellationToken);

            if (stats == null)
            {
                var user = await _userRepository.GetUserByIdAsync(userId)
                    ?? throw new InvalidOperationException("User not found.");

                stats = new UserStats { User = user, TotalPoints = 0, WeeklyScore = 0 };
                await _userStatsRepository.InsertAsync(stats, cancellationToken);
            }

            return stats;
        }

        /// <inheritdoc/>
        public async Task AddPointsAsync(int userId, int movieId, bool isBattleMovie, CancellationToken cancellationToken = default)
        {
            var stats = await this.GetUserStatsAsync(userId, cancellationToken);
            var movie = await _movieRepository.GetMovieByIdAsync(movieId);

            if (movie == null)
            {
                return;
            }

            int pointsToAdd = isBattleMovie ? 5 : 0;
            if (movie.Rating > 3.5m)
            {
                pointsToAdd += 2;
            }
            if (movie.Rating > 3.5m)
            {
                pointsToAdd += 2;
            }
            else if (movie.Rating < 2.0m)
            {
                pointsToAdd += 1;
            }

            stats.TotalPoints = Math.Max(0, stats.TotalPoints + pointsToAdd);
            await _userStatsRepository.UpdateAsync(stats, cancellationToken);

            await _badgeService.CheckAndAwardBadgesAsync(userId, cancellationToken);
        }

        public async Task DeductPointsAsync(int userId, int points, CancellationToken cancellationToken = default)
        {
            var stats = await this.GetUserStatsAsync(userId, cancellationToken);
            stats.TotalPoints = Math.Max(0, stats.TotalPoints - points);
            await _userStatsRepository.UpdateAsync(stats, cancellationToken);
        }

        public async Task FreezePointsAsync(int userId, int amount, CancellationToken cancellationToken = default)
        {
            var stats = await this.GetUserStatsAsync(userId, cancellationToken);
            if (stats.TotalPoints < amount)
            {
                throw new InvalidOperationException($"Insufficient points. Required: {amount}, available: {stats.TotalPoints}.");
            }

            stats.TotalPoints -= amount;
            await _userStatsRepository.UpdateAsync(stats, cancellationToken);
        }

        public async Task RefundPointsAsync(int userId, int amount, CancellationToken cancellationToken = default)
        {
            var stats = await this.GetUserStatsAsync(userId, cancellationToken);
            stats.TotalPoints += amount;
            await _userStatsRepository.UpdateAsync(stats, cancellationToken);
        }

        public async Task UpdateWeeklyScoreAsync(int userId, CancellationToken cancellationToken = default)
        {
            var stats = await this.GetUserStatsAsync(userId, cancellationToken);
            stats.WeeklyScore = stats.TotalPoints;
            await _userStatsRepository.UpdateAsync(stats, cancellationToken);
        }
    }
}

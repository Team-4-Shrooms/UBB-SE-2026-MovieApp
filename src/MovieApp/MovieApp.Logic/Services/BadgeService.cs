using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services
{
    /// <summary>
    /// Implements badge-awarding business logic.
    /// Badge criteria evaluation lives here — not in controllers or repositories.
    /// Ported from 925-1 Core/Services/BadgeService.cs.
    /// </summary>
    public sealed class BadgeService : IBadgeService
    {
        private readonly IBadgeRepository _badgeRepository;
        private readonly IUserBadgeRepository _userBadgeRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IUserStatsRepository _userStatsRepository;

        public BadgeService(
            IBadgeRepository badgeRepository,
            IUserBadgeRepository userBadgeRepository,
            IReviewRepository reviewRepository,
            IMovieRepository movieRepository,
            IUserStatsRepository userStatsRepository)
        {
            _badgeRepository = badgeRepository;
            _userBadgeRepository = userBadgeRepository;
            _reviewRepository = reviewRepository;
            _movieRepository = movieRepository;
            _userStatsRepository = userStatsRepository;
        }

        public async Task<List<UserBadge>> GetUserBadgesAsync(int userId, CancellationToken ct = default)
        {
            return await _userBadgeRepository.GetByUserIdAsync(userId, ct);
        }

        public async Task<List<Badge>> GetAllBadgesAsync(CancellationToken ct = default)
        {
            return await _badgeRepository.GetAllAsync(ct);
        }

        public async Task<IList<UserStats>> GetLeaderboardAsync(CancellationToken ct = default)
        {
            return await _userStatsRepository.GetLeaderboardAsync(ct);
        }

        /// <summary>
        /// Evaluates all badge criteria for a user and awards any newly earned badges.
        /// Loads all data upfront — no N+1 queries.
        /// Badge criteria (ported from 925-1):
        ///   "The Snob"          — 10+ extra reviews
        ///   "Why so serious?"   — 50+ fully-completed extra reviews
        ///   "The Joker"         — comedy genre makes up more than 70% of reviews
        ///   "The Godfather I"   — 100+ total reviews
        ///   "The Godfather II"  — 200+ total reviews
        ///   "The Godfather III" — 300+ total reviews
        /// </summary>
        public async Task CheckAndAwardBadgesAsync(int userId, CancellationToken ct = default)
        {
            List<Badge> allBadges = await _badgeRepository.GetAllAsync(ct);
            List<UserBadge> earnedBadges = await _userBadgeRepository.GetByUserIdAsync(userId, ct);

            HashSet<int> earnedBadgeIds = new HashSet<int>();
            foreach (UserBadge earned in earnedBadges)
            {
                if (earned.Badge is not null)
                {
                    earnedBadgeIds.Add(earned.Badge.BadgeId);
                }
            }

            // Load reviews with Movie + Genres so all criteria can be evaluated without extra queries.
            List<Review> userReviews = await _reviewRepository.GetReviewsByUserIdAsync(userId, ct);

            int totalReviews = userReviews.Count;

            int extraReviews = userReviews.Count(review => review.IsExtraReview);

            int fullyCompletedExtraReviews = userReviews.Count(review =>
                review.IsExtraReview &&
                !string.IsNullOrEmpty(review.CinematographyText) &&
                !string.IsNullOrEmpty(review.ActingText) &&
                !string.IsNullOrEmpty(review.CgiText) &&
                !string.IsNullOrEmpty(review.PlotText) &&
                !string.IsNullOrEmpty(review.SoundText));

            int comedyReviews = userReviews.Count(review =>
            {
                if (review.Movie is null)
                {
                    return false;
                }

                // Prefer the normalised Genres collection; fall back to PrimaryGenre string
                // because seeded movies only populate PrimaryGenre, not the join table.
                if (review.Movie.Genres != null && review.Movie.Genres.Count > 0)
                {
                    return review.Movie.Genres.Any(genre =>
                        genre.Name.Equals("Comedy", StringComparison.OrdinalIgnoreCase));
                }

                return review.Movie.PrimaryGenre.Equals("Comedy", StringComparison.OrdinalIgnoreCase);
            });

            double comedyPercentage = totalReviews > 0
                ? (double)comedyReviews / totalReviews * 100
                : 0;

            foreach (Badge badge in allBadges)
            {
                if (earnedBadgeIds.Contains(badge.BadgeId))
                {
                    continue;
                }

                bool shouldAward = badge.Name switch
                {
                    "The Snob" => extraReviews >= 10,
                    "Why so serious?" => fullyCompletedExtraReviews >= 50,
                    "The Joker" => comedyPercentage > 70,
                    "The Godfather I" => totalReviews >= 100,
                    "The Godfather II" => totalReviews >= 200,
                    "The Godfather III" => totalReviews >= 300,
                    _ => false
                };

                if (shouldAward)
                {
                    UserBadge newUserBadge = new UserBadge
                    {
                        User = new User { Id = userId },
                        Badge = badge,
                    };

                    await _userBadgeRepository.InsertAsync(newUserBadge, ct);
                }
            }
        }
    }
}

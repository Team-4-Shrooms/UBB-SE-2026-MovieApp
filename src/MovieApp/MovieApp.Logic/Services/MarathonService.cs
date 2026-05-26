using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services
{
    public sealed class MarathonService : IMarathonService
    {
        private const int PerfectQuizScore = 3;

        private readonly IMarathonRepository _marathonRepo;
        private readonly ICurrentUserService _currentUserService;

        public MarathonService(IMarathonRepository marathonRepo, ICurrentUserService currentUserService)
        {
            _marathonRepo = marathonRepo;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<Marathon>> GetWeeklyMarathonsAsync(int userId)
        {
            DateTime currentDateTime = DateTime.UtcNow;
            string weekIdentifier = $"{currentDateTime.Year}-W" +
                ISOWeek.GetWeekOfYear(currentDateTime).ToString("D2");

            IEnumerable<Marathon> existingMarathons = await _marathonRepo
                .GetWeeklyMarathonsForUserAsync(userId, weekIdentifier);

            List<Marathon> marathonList = existingMarathons.ToList();

            if (marathonList.Count == 0)
            {
                await _marathonRepo.AssignWeeklyMarathonsAsync(userId, weekIdentifier, 10);
                marathonList = (await _marathonRepo
                    .GetWeeklyMarathonsForUserAsync(userId, weekIdentifier)).ToList();
            }

            return marathonList;
        }

        public async Task<MarathonProgress?> GetCurrentProgressAsync(int marathonId)
        {
            int currentUserId = _currentUserService.UserId;
            return await _marathonRepo.GetUserProgressAsync(currentUserId, marathonId);
        }

        public async Task<bool> StartMarathonAsync(int marathonId)
        {
            int currentUserId = _currentUserService.UserId;

            MarathonProgress? existingProgress = await _marathonRepo.GetUserProgressAsync(currentUserId, marathonId);
            if (existingProgress != null)
            {
                return true;
            }

            IEnumerable<Marathon> activeMarathons = await _marathonRepo.GetActiveMarathonsAsync();
            Marathon? selectedMarathon = activeMarathons.FirstOrDefault(marathon => marathon.Id == marathonId);

            if (selectedMarathon?.PrerequisiteMarathonId is int prerequisiteMarathonId)
            {
                bool isPrerequisiteComplete = await _marathonRepo
                    .IsPrerequisiteCompletedAsync(currentUserId, prerequisiteMarathonId);

                if (!isPrerequisiteComplete)
                {
                    return false;
                }
            }

            return await _marathonRepo.JoinMarathonAsync(currentUserId, marathonId);
        }

        public async Task UpdateQuizResultAsync(int marathonId, int correctAnswers)
        {
            int currentUserId = _currentUserService.UserId;
            MarathonProgress? marathonProgress = await _marathonRepo.GetUserProgressAsync(currentUserId, marathonId);

            if (marathonProgress != null)
            {
                double newQuizScore = (correctAnswers / (double)PerfectQuizScore) * 100;
                marathonProgress.TriviaAccuracy = (marathonProgress.TriviaAccuracy + newQuizScore) / 2;
                marathonProgress.CompletedMoviesCount++;

                int totalMovies = await _marathonRepo.GetMarathonMovieCountAsync(marathonId);
                if (totalMovies > 0 && marathonProgress.CompletedMoviesCount >= totalMovies && !marathonProgress.IsCompleted)
                {
                    marathonProgress.FinishedAt = DateTime.UtcNow;
                }

                await _marathonRepo.UpdateProgressAsync(marathonProgress);
            }
        }

        public async Task<bool> LogMovieAsync(int marathonId, int movieId, int correctAnswers)
        {
            if (correctAnswers < PerfectQuizScore)
            {
                return false;
            }

            int currentUserId = _currentUserService.UserId;
            MarathonProgress? marathonProgress = await _marathonRepo.GetUserProgressAsync(currentUserId, marathonId);
            if (marathonProgress is null)
            {
                return false;
            }

            double newScore = (correctAnswers / (double)PerfectQuizScore) * 100;
            marathonProgress.TriviaAccuracy = marathonProgress.CompletedMoviesCount == 0
                ? newScore
                : (marathonProgress.TriviaAccuracy + newScore) / 2;

            marathonProgress.CompletedMoviesCount++;

            int totalMovies = await _marathonRepo.GetMarathonMovieCountAsync(marathonId);
            if (totalMovies > 0 && marathonProgress.CompletedMoviesCount >= totalMovies && !marathonProgress.IsCompleted)
            {
                marathonProgress.FinishedAt = DateTime.UtcNow;
            }

            await _marathonRepo.UpdateProgressAsync(marathonProgress);
            return true;
        }

        public Task<int> GetParticipantCountAsync(int marathonId)
        {
            return _marathonRepo.GetParticipantCountAsync(marathonId);
        }

        public Task<int> GetMarathonMovieCountAsync(int marathonId)
        {
            return _marathonRepo.GetMarathonMovieCountAsync(marathonId);
        }

        public Task<bool> IsPrerequisiteCompletedAsync(int userId, int marathonId)
        {
            return _marathonRepo.IsPrerequisiteCompletedAsync(userId, marathonId);
        }

        public Task<IEnumerable<Movie>> GetMoviesForMarathonAsync(int marathonId)
        {
            return _marathonRepo.GetMoviesForMarathonAsync(marathonId);
        }

        public Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync(int marathonId)
        {
            return _marathonRepo.GetLeaderboardWithUsernamesAsync(marathonId);
        }

        public Task<MarathonProgress?> GetUserProgressAsync(int userId, int marathonId)
        {
            return _marathonRepo.GetUserProgressAsync(userId, marathonId);
        }

        public Task<IEnumerable<LeaderboardEntry>> GetLeaderboardWithUsernamesAsync(int marathonId)
        {
            return _marathonRepo.GetLeaderboardWithUsernamesAsync(marathonId);
        }
    }
}

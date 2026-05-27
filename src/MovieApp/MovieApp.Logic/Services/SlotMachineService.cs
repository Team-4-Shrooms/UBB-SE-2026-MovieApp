using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Models;

namespace MovieApp.Logic.Services
{
    public sealed class SlotMachineService : ISlotMachineService
    {
        private const int ResetSpinsCount = 5;
        private const int NoSpinsAvailable = 0;
        private const int DiscountPercentage = 70;
        private const double DiscountPercentageDouble = 70.0;
        private const int RequiredLoginStreak = 3;
        private const int MaximumEventSpinsPerDay = 2;

        private readonly IUserSlotMachineStateRepository _stateRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IUserMovieDiscountRepository _discountRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly Random _random = new();

        public SlotMachineService(
            IUserSlotMachineStateRepository stateRepository,
            IMovieRepository movieRepository,
            IEventRepository eventRepository,
            IUserMovieDiscountRepository discountRepository,
            INotificationRepository notificationRepository)
        {
            _stateRepository = stateRepository;
            _movieRepository = movieRepository;
            _eventRepository = eventRepository;
            _discountRepository = discountRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<SlotMachineResult> SpinAsync(int userIdentifier)
        {
            UserSpinData? state = await GetOrCreateUserStateAsync(userIdentifier);

            DateTime currentUtcDate = DateTime.UtcNow.Date;
            if (state.LastSlotSpinReset.Date < currentUtcDate)
            {
                state.ResetDailySpins(ResetSpinsCount);
            }

            int totalSpinsCount = state.DailySpinsRemaining + state.BonusSpins;
            if (totalSpinsCount <= NoSpinsAvailable)
            {
                throw new InvalidOperationException("No available spins");
            }

            if (state.DailySpinsRemaining > NoSpinsAvailable)
            {
                state.DailySpinsRemaining--;
            }
            else
            {
                state.BonusSpins--;
            }

            List<Genre> distinctGenres = (await _movieRepository.GetGenresAsync()).DistinctBy(g => g.Id).ToList();
            List<Actor> distinctActors = (await _movieRepository.GetActorsAsync()).DistinctBy(a => a.Id).ToList();
            List<Director> distinctDirectors = (await _movieRepository.GetDirectorsAsync()).DistinctBy(d => d.Id).ToList();

            if (distinctGenres.Count == 0 || distinctActors.Count == 0 || distinctDirectors.Count == 0)
            {
                throw new InvalidOperationException("No movies with active screenings available");
            }

            Genre selectedGenre = distinctGenres[_random.Next(distinctGenres.Count)];
            Actor selectedActor = distinctActors[_random.Next(distinctActors.Count)];
            Director selectedDirector = distinctDirectors[_random.Next(distinctDirectors.Count)];

            IReadOnlyList<MovieEvent> matchingEvents =
                await GetMatchingEventsAsync(selectedGenre.Id, selectedActor.Id, selectedDirector.Id);

            Movie? jackpotMovie =
                await FindJackpotMovieAsync(selectedGenre.Id, selectedActor.Id, selectedDirector.Id);

            HashSet<int> jackpotEventIds = jackpotMovie is not null
                ? matchingEvents
                    .Where(e => e.Movie?.Id == jackpotMovie.Id)
                    .Select(e => e.Id)
                    .ToHashSet()
                : new HashSet<int>();

            SlotMachineResult result = new()
            {
                Genre = selectedGenre,
                Actor = selectedActor,
                Director = selectedDirector,
                MatchingEvents = matchingEvents.ToList(),
                JackpotEventIds = jackpotEventIds,
                JackpotMovie = jackpotMovie,
                JackpotDiscountApplied = false,
                DiscountPercentage = 0
            };

            if (jackpotMovie is not null)
            {
                await GrantJackpotDiscountAsync(userIdentifier, jackpotMovie.Id);
                result.JackpotDiscountApplied = true;
                result.DiscountPercentage = DiscountPercentage;
            }

            await _stateRepository.UpdateAsync(state);
            return result;
        }

        public async Task<int> GetAvailableSpinsAsync(int userIdentifier)
        {
            UserSpinData state = await GetOrCreateUserStateAsync(userIdentifier);

            DateTime currentUtcDate = DateTime.UtcNow.Date;
            if (state.LastSlotSpinReset.Date < currentUtcDate)
            {
                state.ResetDailySpins(ResetSpinsCount);
                await _stateRepository.UpdateAsync(state);
            }

            return state.DailySpinsRemaining + state.BonusSpins;
        }

        public async Task<UserSpinData> GetUserSpinStateAsync(int userIdentifier)
        {
            UserSpinData state = await GetOrCreateUserStateAsync(userIdentifier);

            DateTime currentUtcDate = DateTime.UtcNow.Date;
            if (state.LastSlotSpinReset.Date < currentUtcDate)
            {
                state.ResetDailySpins(ResetSpinsCount);
                await _stateRepository.UpdateAsync(state);
            }

            return state;
        }

        public async Task<bool> GrantBonusSpinForEventParticipationAsync(int userIdentifier)
        {
            UserSpinData state = await GetOrCreateUserStateAsync(userIdentifier);

            if (state.EventSpinRewardsToday < MaximumEventSpinsPerDay)
            {
                state.BonusSpins++;
                state.EventSpinRewardsToday++;
                await _stateRepository.UpdateAsync(state);
                return true;
            }

            return false;
        }

        public async Task<bool> RecordLoginAndCheckStreakAsync(int userIdentifier)
        {
            UserSpinData state = await GetOrCreateUserStateAsync(userIdentifier);

            state.UpdateLoginStreak();

            bool granted = false;

            if (state.LoginStreak >= RequiredLoginStreak)
            {
                state.BonusSpins++;
                state.LoginStreak = 0;
                granted = true;
            }

            await _stateRepository.UpdateAsync(state);
            return granted;
        }

        public async Task<bool> GrantStreakSpinAsync(int userIdentifier)
        {
            UserSpinData state = await GetOrCreateUserStateAsync(userIdentifier);

            if (state.LoginStreak >= RequiredLoginStreak)
            {
                state.BonusSpins++;
                state.LoginStreak = 0;
                await _stateRepository.UpdateAsync(state);
                return true;
            }

            return false;
        }

        public async Task<Genre> GetRandomGenreAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Genre> genres = await _movieRepository.GetGenresAsync(cancellationToken);
            if (genres.Count == 0)
            {
                return new Genre();
            }

            return genres[_random.Next(genres.Count)];
        }

        public async Task<Actor> GetRandomActorAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Actor> actors = await _movieRepository.GetActorsAsync(cancellationToken);
            if (actors.Count == 0)
            {
                return new Actor();
            }

            return actors[_random.Next(actors.Count)];
        }

        public async Task<Director> GetRandomDirectorAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Director> directors = await _movieRepository.GetDirectorsAsync(cancellationToken);
            if (directors.Count == 0)
            {
                return new Director();
            }

            return directors[_random.Next(directors.Count)];
        }

        public Task<IReadOnlyList<Genre>> GetGenresAsync(CancellationToken cancellationToken = default) =>
            _movieRepository.GetGenresAsync(cancellationToken);

        public Task<IReadOnlyList<Actor>> GetActorsAsync(CancellationToken cancellationToken = default) =>
            _movieRepository.GetActorsAsync(cancellationToken);

        public Task<IReadOnlyList<Director>> GetDirectorsAsync(CancellationToken cancellationToken = default) =>
            _movieRepository.GetDirectorsAsync(cancellationToken);

        public async Task<IReadOnlyList<MovieEvent>> GetMatchingEventsAsync(
            int genreIdentifier,
            int actorIdentifier,
            int directorIdentifier)
        {
            IReadOnlyList<Movie> movies =
                await _movieRepository.FindMoviesByAnyCriteriaAsync(
                    genreIdentifier, actorIdentifier, directorIdentifier);

            if (movies.Count == 0)
            {
                return new List<MovieEvent>();
            }

            HashSet<int> movieIds = movies.Select(m => m.Id).ToHashSet();

            List<MovieEvent> allMovieEvents = await _eventRepository.GetAllEventsAsync();

            return allMovieEvents
                .Where(e => e.Movie != null && movieIds.Contains(e.Movie.Id) && e.Date > DateTime.UtcNow)
                .DistinctBy(e => e.Id)
                .ToList();
        }

        public async Task<Movie?> FindJackpotMovieAsync(
            int genreIdentifier,
            int actorIdentifier,
            int directorIdentifier)
        {
            IReadOnlyList<Movie> movies =
                await _movieRepository.FindMoviesByCriteriaAsync(
                    genreIdentifier, actorIdentifier, directorIdentifier);

            return movies.FirstOrDefault();
        }

        public async Task GrantJackpotDiscountAsync(int userIdentifier, int movieIdentifier)
        {
            Movie? movie = await _movieRepository.GetMovieByIdAsync(movieIdentifier);
            string title = movie?.Title ?? "a movie";

            Reward reward = new()
            {
                RewardId = 0,
                RewardType = "MovieDiscount",
                RedemptionStatus = false,
                ApplicabilityScope = title,
                DiscountValue = DiscountPercentageDouble,
                OwnerUserId = userIdentifier,
                EventId = movieIdentifier
            };

            await _discountRepository.AddAsync(reward);

            Notification notification = new()
            {
                Id = 0,
                UserId = userIdentifier,
                EventId = 0,
                Type = "Jackpot Win",
                Message = $"Congratulations! You won a {DiscountPercentage}% discount for '{title}'!",
                CreatedAt = DateTime.UtcNow,
                State = NotificationState.Unread
            };

            await _notificationRepository.AddAsync(notification);
        }

        private async Task<UserSpinData> GetOrCreateUserStateAsync(int userIdentifier)
        {
            UserSpinData? state = await _stateRepository.GetByUserIdAsync(userIdentifier);

            if (state is not null)
            {
                return state;
            }

            state = new UserSpinData
            {
                UserId = userIdentifier,
                DailySpinsRemaining = ResetSpinsCount,
                LastSlotSpinReset = DateTime.UtcNow.Date,
                BonusSpins = 0,
                EventSpinRewardsToday = 0,
                LoginStreak = 0,
                LastLoginDate = DateTime.MinValue
            };

            await _stateRepository.CreateAsync(state);
            return state;
        }

    }
}

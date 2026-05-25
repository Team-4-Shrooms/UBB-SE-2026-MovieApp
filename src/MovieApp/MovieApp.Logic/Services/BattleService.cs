using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.Battles;

namespace MovieApp.Logic.Services
{
    public sealed class BattleService : IBattleService
    {
        private readonly IBattleRepository _battleRepository;
        private readonly IBetRepository _betRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPointService _pointService;

        public BattleService(
        IBattleRepository battleRepository,
        IBetRepository betRepository,
        IMovieRepository movieRepository,
        IUserRepository userRepository,
        IPointService pointService)
        {
            _battleRepository = battleRepository;
            _betRepository = betRepository;
            _movieRepository = movieRepository;
            _userRepository = userRepository;
            _pointService = pointService;
        }


        public async Task<Battle> CreateBattleAsync(int firstMovieId, int secondMovieId, CancellationToken cancellationToken = default)
        {
            var allBattles = await _battleRepository.GetAllAsync(cancellationToken);
            if (allBattles.Any(b => b.Status == "Active"))
            {
                throw new InvalidOperationException("An active battle already exists.");
            }

            var first = await _movieRepository.GetMovieByIdAsync(firstMovieId) ?? throw new InvalidOperationException("First movie not found.");
            var second = await _movieRepository.GetMovieByIdAsync(secondMovieId) ?? throw new InvalidOperationException("Second movie not found.");

            var startDate = DateTime.UtcNow.Date; 
            var battle = new Battle
            {
                FirstMovie = first,
                SecondMovie = second,
                InitialRatingFirstMovie = (double)first.Rating,
                InitialRatingSecondMovie = (double)second.Rating,
                StartDate = startDate,
                EndDate = startDate.AddDays(6),
                Status = "Active"
            };

            await _battleRepository.InsertAsync(battle, cancellationToken);
            return battle;
        }

        public async Task<Battle> CreateDemoBattleAsync(CancellationToken cancellationToken = default)
        {
            var movies = await _movieRepository.GetAllMoviesAsync();
            if (movies.Count < 2)
            {
                throw new InvalidOperationException("Not enough movies for a battle.");
            }

            var sortedMovies = movies.OrderBy(movie => movie.Rating).ToList();
            Movie bestMovie1 = sortedMovies[0];
            Movie bestMovie2 = sortedMovies[1];
            double minDiff = (double)Math.Abs(bestMovie1.Rating - bestMovie2.Rating);

            for (int i = 0; i < sortedMovies.Count - 1; i++)
            {
                double diff = (double)Math.Abs(sortedMovies[i].Rating - sortedMovies[i + 1].Rating);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    bestMovie1 = sortedMovies[i];
                    bestMovie2 = sortedMovies[i + 1];
                }
            }

            return await this.CreateBattleAsync(bestMovie1.Id, bestMovie2.Id, cancellationToken);
        }

        public async Task<int> DetermineWinnerAsync(int battleId, CancellationToken cancellationToken = default)
        {
            var battle = await _battleRepository.GetByIdAsync(battleId, cancellationToken) ?? throw new InvalidOperationException("Battle not found.");

            var movie1 = await _movieRepository.GetMovieByIdAsync(battle.FirstMovie?.Id ?? 0);
            var movie2 = await _movieRepository.GetMovieByIdAsync(battle.SecondMovie?.Id ?? 0);

            double growth1 = (double)((movie1?.Rating ?? 0) - (decimal)battle.InitialRatingFirstMovie);
            double growth2 = (double)((movie2?.Rating ?? 0) - (decimal)battle.InitialRatingSecondMovie);

            return growth1 >= growth2 ? (movie1?.Id ?? 0) : (movie2?.Id ?? 0);
        }

        public async Task DistributePayoutsAsync(int battleId, CancellationToken cancellationToken = default)
        {
            int winnerId = await this.DetermineWinnerAsync(battleId, cancellationToken);
            var bets = await _betRepository.GetAllAsync(cancellationToken);
            var battleBets = bets.Where(battle => battle.Battle?.BattleId == battleId).ToList();

            foreach (var bet in battleBets)
            {
                if (bet.Movie?.Id == winnerId)
                {
                    await _pointService.RefundPointsAsync(bet.User?.Id ?? 0, bet.Amount * 2, cancellationToken);
                }
            }

            var battle = await _battleRepository.GetByIdAsync(battleId, cancellationToken);
            if (battle != null)
            {
                battle.Status = "Finished";
                await _battleRepository.UpdateAsync(battle, cancellationToken);
            }
        }

        public async Task<Battle?> GetActiveBattleAsync(CancellationToken cancellationToken = default)
        {
            var battles = await _battleRepository.GetAllAsync(cancellationToken);
            var active = battles.FirstOrDefault(b => b.Status == "Active");

            if (active != null)
            {
                active.FirstMovie = await _movieRepository.GetMovieByIdAsync(active.FirstMovie?.Id ?? 0) ?? active.FirstMovie;
                active.SecondMovie = await _movieRepository.GetMovieByIdAsync(active.SecondMovie?.Id ?? 0) ?? active.SecondMovie;
            }

            return active;
        }

        public async Task SettleExpiredBattlesAsync(CancellationToken cancellationToken = default)
        {
            var battles = await _battleRepository.GetAllAsync(cancellationToken);
            var expired = battles.Where(battle => battle.Status == "Active" && battle.EndDate < DateTime.UtcNow.Date);

            foreach (var battle in expired)
            {
                await this.DistributePayoutsAsync(battle.BattleId, cancellationToken);
            }
        }

        public async Task<Battle?> GetCurrentBattleForUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            var active = await this.GetActiveBattleAsync(cancellationToken);
            if (active != null)
            {
                return active;
            }

            var battles = await _battleRepository.GetAllAsync(cancellationToken);
            return battles
                .Where(battle => battle.Bets.Any(bet => bet.User?.Id == userId))
                .OrderByDescending(battle => battle.EndDate)
                .ThenByDescending(battle => battle.BattleId)
                .FirstOrDefault();
        }

        public async Task<BattleBet> PlaceBetAsync(int userId, int battleId, int movieId, int amount, CancellationToken cancellationToken = default)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("Amount must be positive.");
            }

            var existingBet = await _betRepository.GetByIdAsync(userId, battleId, cancellationToken);
            if (existingBet != null)
            {
                throw new InvalidOperationException("User has already bet.");
            }

            var user = await _userRepository.GetUserByIdAsync(userId) ?? throw new InvalidOperationException("User not found.");
            var battle = await _battleRepository.GetByIdAsync(battleId, cancellationToken) ?? throw new InvalidOperationException("Battle not found.");
            var movie = await _movieRepository.GetMovieByIdAsync(movieId) ?? throw new InvalidOperationException("Movie not found.");

            if (!string.Equals(battle.Status, "Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("This battle is not accepting bets.");
            }

            if (movie.Id != battle.FirstMovie?.Id && movie.Id != battle.SecondMovie?.Id)
            {
                throw new InvalidOperationException("Selected movie is not part of this battle.");
            }

            await _pointService.FreezePointsAsync(userId, amount, cancellationToken);

            var bet = new BattleBet { User = user, Battle = battle, Movie = movie, Amount = amount };
            await _betRepository.InsertAsync(bet, cancellationToken);
            return bet;
        }

        public async Task ResetAllBattlesForDemoAsync(CancellationToken cancellationToken = default)
        {
            var battles = await _battleRepository.GetAllAsync(cancellationToken);
            foreach (var battle in battles)
            {
                await _battleRepository.DeleteAsync(battle.BattleId, cancellationToken);
            }
        }

        public async Task ForceSettleBattleAsync(int battleId, CancellationToken cancellationToken = default)
        {
            await this.DistributePayoutsAsync(battleId, cancellationToken);
        }

        public async Task<BattleBet?> GetBetAsync(int userId, int battleId, CancellationToken cancellationToken = default)
        {
            return await _betRepository.GetByIdAsync(userId, battleId, cancellationToken);
        }

        public async Task<IEnumerable<Battle>> GetBattlesAsync(CancellationToken cancellationToken = default)
        {
            var battles = await _battleRepository.GetAllAsync(cancellationToken);

            foreach (var battle in battles)
            {
                battle.FirstMovie =
                    await _movieRepository.GetMovieByIdAsync(battle.FirstMovie?.Id ?? 0)
                    ?? battle.FirstMovie;

                battle.SecondMovie =
                    await _movieRepository.GetMovieByIdAsync(battle.SecondMovie?.Id ?? 0)
                    ?? battle.SecondMovie;
            }

            return battles;
        }

        public async Task<Battle?> GetBattleByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var battle = await _battleRepository.GetByIdAsync(id, cancellationToken);

            if (battle != null)
            {
                battle.FirstMovie =
                    await _movieRepository.GetMovieByIdAsync(battle.FirstMovie?.Id ?? 0)
                    ?? battle.FirstMovie;

                battle.SecondMovie =
                    await _movieRepository.GetMovieByIdAsync(battle.SecondMovie?.Id ?? 0)
                    ?? battle.SecondMovie;
            }

            return battle;
        }
    }
}

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


        public async Task<Battle> CreateBattleAsync(int firstMovieId, int secondMovieId, CancellationToken ct = default)
        {
            var allBattles = await _battleRepository.GetAllAsync(ct);
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

            await _battleRepository.InsertAsync(battle, ct);
            return battle;
        }

        public async Task<Battle> CreateDemoBattleAsync(CancellationToken ct = default)
        {
            var movies = await _movieRepository.GetAllMoviesAsync();
            if (movies.Count < 2)
            {
                throw new InvalidOperationException("Not enough movies for a battle.");
            }

            var sortedMovies = movies.OrderBy(m => m.Rating).ToList();
            Movie bestM1 = sortedMovies[0];
            Movie bestM2 = sortedMovies[1];
            double minDiff = (double)Math.Abs(bestM1.Rating - bestM2.Rating);

            for (int i = 0; i < sortedMovies.Count - 1; i++)
            {
                double diff = (double)Math.Abs(sortedMovies[i].Rating - sortedMovies[i + 1].Rating);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    bestM1 = sortedMovies[i];
                    bestM2 = sortedMovies[i + 1];
                }
            }

            return await this.CreateBattleAsync(bestM1.Id, bestM2.Id, ct);
        }

        public async Task<int> DetermineWinnerAsync(int battleId, CancellationToken ct = default)
        {
            var battle = await _battleRepository.GetByIdAsync(battleId, ct) ?? throw new InvalidOperationException("Battle not found.");

            var m1 = await _movieRepository.GetMovieByIdAsync(battle.FirstMovie?.Id ?? 0);
            var m2 = await _movieRepository.GetMovieByIdAsync(battle.SecondMovie?.Id ?? 0);

            double growth1 = (double)((m1?.Rating ?? 0) - (decimal)battle.InitialRatingFirstMovie);
            double growth2 = (double)((m2?.Rating ?? 0) - (decimal)battle.InitialRatingSecondMovie);

            return growth1 >= growth2 ? (m1?.Id ?? 0) : (m2?.Id ?? 0);
        }

        public async Task DistributePayoutsAsync(int battleId, CancellationToken ct = default)
        {
            int winnerId = await this.DetermineWinnerAsync(battleId, ct);
            var bets = await _betRepository.GetAllAsync(ct);
            var battleBets = bets.Where(b => b.Battle?.BattleId == battleId).ToList();

            foreach (var bet in battleBets)
            {
                if (bet.Movie?.Id == winnerId)
                {
                    await _pointService.RefundPointsAsync(bet.User?.Id ?? 0, bet.Amount * 2, ct);
                }
            }

            var battle = await _battleRepository.GetByIdAsync(battleId, ct);
            if (battle != null)
            {
                battle.Status = "Finished";
                await _battleRepository.UpdateAsync(battle, ct);
            }
        }

        public async Task<Battle?> GetActiveBattleAsync(CancellationToken ct = default)
        {
            var battles = await _battleRepository.GetAllAsync(ct);
            var active = battles.FirstOrDefault(b => b.Status == "Active");

            if (active != null)
            {
                active.FirstMovie = await _movieRepository.GetMovieByIdAsync(active.FirstMovie?.Id ?? 0) ?? active.FirstMovie;
                active.SecondMovie = await _movieRepository.GetMovieByIdAsync(active.SecondMovie?.Id ?? 0) ?? active.SecondMovie;
            }

            return active;
        }

        public async Task SettleExpiredBattlesAsync(CancellationToken ct = default)
        {
            var battles = await _battleRepository.GetAllAsync(ct);
            var expired = battles.Where(b => b.Status == "Active" && b.EndDate < DateTime.UtcNow.Date);

            foreach (var battle in expired)
            {
                await this.DistributePayoutsAsync(battle.BattleId, ct);
            }
        }

        public async Task<Battle?> GetCurrentBattleForUserAsync(int userId, CancellationToken ct = default)
        {
            var active = await this.GetActiveBattleAsync(ct);
            if (active != null)
            {
                return active;
            }

            var battles = await _battleRepository.GetAllAsync(ct);
            return battles
                .Where(b => b.Bets.Any(bet => bet.User?.Id == userId))
                .OrderByDescending(b => b.EndDate)
                .ThenByDescending(b => b.BattleId)
                .FirstOrDefault();
        }

        public async Task<BattleBet> PlaceBetAsync(int userId, int battleId, int movieId, int amount, CancellationToken ct = default)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("Amount must be positive.");
            }

            var existingBet = await _betRepository.GetByIdAsync(userId, battleId, ct);
            if (existingBet != null)
            {
                throw new InvalidOperationException("User has already bet.");
            }

            var user = await _userRepository.GetUserByIdAsync(userId) ?? throw new InvalidOperationException("User not found.");
            var battle = await _battleRepository.GetByIdAsync(battleId, ct) ?? throw new InvalidOperationException("Battle not found.");
            var movie = await _movieRepository.GetMovieByIdAsync(movieId) ?? throw new InvalidOperationException("Movie not found.");

            if (!string.Equals(battle.Status, "Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("This battle is not accepting bets.");
            }

            if (movie.Id != battle.FirstMovie?.Id && movie.Id != battle.SecondMovie?.Id)
            {
                throw new InvalidOperationException("Selected movie is not part of this battle.");
            }

            await _pointService.FreezePointsAsync(userId, amount, ct);

            var bet = new BattleBet { User = user, Battle = battle, Movie = movie, Amount = amount };
            await _betRepository.InsertAsync(bet, ct);
            return bet;
        }

        public async Task ResetAllBattlesForDemoAsync(CancellationToken ct = default)
        {
            var battles = await _battleRepository.GetAllAsync(ct);
            foreach (var b in battles)
            {
                await _battleRepository.DeleteAsync(b.BattleId, ct);
            }
        }

        public async Task ForceSettleBattleAsync(int battleId, CancellationToken ct = default)
        {
            await this.DistributePayoutsAsync(battleId, ct);
        }

        public async Task<BattleBet?> GetBetAsync(int userId, int battleId, CancellationToken ct = default)
        {
            return await _betRepository.GetByIdAsync(userId, battleId, ct);
        }
    }
}

namespace MovieApp.Web.Controllers
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using MovieApp.DataLayer.Models;
    using MovieApp.Logic.Features.Battles;
    using MovieApp.Logic.Interfaces.Services;
    using MovieApp.Web.ViewModels.Battles;

    public class BattleController : Controller
    {
        private readonly IBattleService _battleService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPointService _pointService;

        public BattleController(
            IBattleService battleService,
            ICurrentUserService currentUserService,
            IPointService pointService)
        {
            this._battleService = battleService;
            this._currentUserService = currentUserService;
            this._pointService = pointService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //await this._battleService.SettleExpiredBattlesAsync();

            int currentUserId = this._currentUserService.UserId;

            Task<UserStats> userStatsTask =
                this._pointService.GetUserStatsAsync(currentUserId);

            Task<Battle?> battleTask =
                this._battleService.GetCurrentBattleForUserAsync(currentUserId);

            await Task.WhenAll(userStatsTask, battleTask);

            UserStats userStats = await userStatsTask;
            Battle? battle = await battleTask;

            BattleBet? userBet = battle == null
                ? null
                : await this._battleService.GetBetAsync(
                    currentUserId,
                    battle.BattleId);

            int? winnerMovieId = null;

            if (battle?.Status == "Finished")
            {
                winnerMovieId = await this._battleService
                    .DetermineWinnerAsync(battle.BattleId);
            }

            Debug.WriteLine("------------------"+ userStats.TotalPoints+ "--------------------------");

            BattleViewModel viewModel = new BattleViewModel
            {
                Battle = battle,
                UserBet = userBet,
                CurrentUserPoints = userStats.TotalPoints,
                WinnerMovieId = winnerMovieId,
                StatusMessage = TempData["StatusMessage"] as string
            };

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceBet(
            PlaceBattleBetInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                TempData["StatusMessage"] =
                    "Choose a movie and enter a valid whole-number amount.";

                return this.RedirectToAction(nameof(this.Index));
            }

            try
            {
                await this._battleService.PlaceBetAsync(
                    this._currentUserService.UserId,
                    model.BattleId,
                    model.MovieId,
                    model.Amount);

                TempData["StatusMessage"] =
                    "Your bet has been placed.";
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] =
                    $"Could not place bet: {ex.Message}";
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDemo()
        {
            try
            {
                await this._battleService.CreateDemoBattleAsync();

                TempData["StatusMessage"] =
                    "A demo battle has been started.";
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] =
                    $"Could not start a demo battle: {ex.Message}";
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForceSettle(int battleId)
        {
            try
            {
                await this._battleService
                    .ForceSettleBattleAsync(battleId);

                TempData["StatusMessage"] =
                    "Battle settled. Points have been distributed.";
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] =
                    $"Could not settle the battle: {ex.Message}";
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetDemo()
        {
            try
            {
                await this._battleService.ResetAllBattlesForDemoAsync();

                TempData["StatusMessage"] =
                    "Demo reset. A new battle has been created.";
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] =
                    $"Could not reset the demo: {ex.Message}";
            }

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}

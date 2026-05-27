using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Web.Models;

namespace MovieApp.Web.Controllers
{
    public class SlotMachineController : Controller
    {
        private readonly ISlotMachineService _slotMachineService;
        private readonly ICurrentUserService _currentUserService;

        public SlotMachineController(
            ISlotMachineService slotMachineService,
            ICurrentUserService currentUserService)
        {
            _slotMachineService = slotMachineService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _currentUserService.UserId;

            var viewModel = await BuildViewModelAsync(userId);
            viewModel.StatusMessage = TempData["StatusMessage"] as string;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Spin()
        {
            var userId = _currentUserService.UserId;

            var viewModel = await BuildViewModelAsync(userId);

            if (!viewModel.CanSpin)
            {
                viewModel.StatusMessage = "No spins remaining - come back tomorrow!";
                return View(nameof(Index), viewModel);
            }

            try
            {
                var result = await _slotMachineService.SpinAsync(userId);

                // Re-fetch state so the displayed spin counter reflects the post-spin total.
                viewModel = await BuildViewModelAsync(userId);
                viewModel.LastResult = result;

                viewModel.StatusMessage = result.JackpotDiscountApplied
                    ? $"JACKPOT! You won a {result.DiscountPercentage}% discount" +
                      $" on \"{result.JackpotMovie?.Title}\"."
                    : result.MatchingEvents.Count > 0
                        ? $"Nice spin - {result.MatchingEvents.Count} matching event(s) found."
                        : "No matching events this time. Try again!";
            }
            catch (Exception ex)
            {
                viewModel.StatusMessage = $"Could not spin: {ex.Message}";
            }

            return View(nameof(Index), viewModel);
        }

        private async Task<SlotMachineIndexViewModel> BuildViewModelAsync(int userId)
        {
            var state = await _slotMachineService.GetUserSpinStateAsync(userId);
            var available = await _slotMachineService.GetAvailableSpinsAsync(userId);

            return new SlotMachineIndexViewModel
            {
                AvailableSpins = available,
                DailySpinsRemaining = state.DailySpinsRemaining,
                BonusSpins = state.BonusSpins,
                LoginStreak = state.LoginStreak,
            };
        }
    }
}

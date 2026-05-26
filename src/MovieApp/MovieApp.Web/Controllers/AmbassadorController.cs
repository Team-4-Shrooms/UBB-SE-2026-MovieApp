namespace MovieApp.Web.Controllers;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Web.Models;

public sealed class AmbassadorController : Controller
{
    private readonly IAmbassadorService _ambassadorService;
    private readonly ICurrentUserService _currentUserService;

    public AmbassadorController(IAmbassadorService ambassadorService, ICurrentUserService currentUserService)
    {
        _ambassadorService = ambassadorService;
        _currentUserService = currentUserService;
    }

    public async Task<IActionResult> Index()
    {
        int userId = _currentUserService.UserId;
        string? code = await _ambassadorService.GetReferralCodeAsync(userId);

        if (code is null)
        {
            return View(new AmbassadorDashboardViewModel { IsAmbassador = false });
        }

        var history = await _ambassadorService.GetReferralHistoryAsync(userId);
        int balance = await _ambassadorService.GetRewardBalanceAsync(userId);

        return View(new AmbassadorDashboardViewModel
        {
            IsAmbassador = true,
            ReferralCode = code,
            RewardBalance = balance,
            History = history,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Become()
    {
        int userId = _currentUserService.UserId;
        string code = GenerateReferralCode(userId);

        try
        {
            if (await _ambassadorService.GetReferralCodeAsync(userId) is not null)
            {
                TempData["Error"] = "You are already an ambassador.";
                return RedirectToAction(nameof(Index));
            }

            await _ambassadorService.CreateAmbassadorProfileAsync(userId, code);
            TempData["Success"] = "You are now an ambassador! Your referral code has been created.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private static string GenerateReferralCode(int userId) =>
        $"AMB{userId}{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
}

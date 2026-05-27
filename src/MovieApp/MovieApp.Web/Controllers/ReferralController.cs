using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Services;
using MovieApp.Web.Models;

namespace MovieApp.Web.Controllers;

//[Authorize]
public class ReferralController : Controller
{
    private readonly IAmbassadorService _ambassadorService;
    private readonly IReferralCodeGenerator _referralCodeGenerator;
    private readonly IReferralLogService _referralLogService;
    private readonly ICurrentUserService _currentUserService;

    public ReferralController(
       IAmbassadorService ambassadorService,
       IReferralCodeGenerator referralCodeGenerator,
       IReferralLogService referralLogService,
       ICurrentUserService currentUserService)
    {
        _ambassadorService = ambassadorService;
        _referralCodeGenerator = referralCodeGenerator;
        _referralLogService = referralLogService;
        _currentUserService = currentUserService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await BuildModelAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateCode()
    {
        int userId = _currentUserService.UserId;

        
        var existingCode = await _ambassadorService.GetReferralCodeAsync(userId);
        if (string.IsNullOrWhiteSpace(existingCode))
        {
            string username = User.Identity?.Name ?? "User";

            var code = _referralCodeGenerator.Generate(username, userId);
            await _ambassadorService.CreateAmbassadorProfileAsync(userId, code);
            TempData["Success"] = "Referral code generated successfully.";
        }
        else
        {
            TempData["Error"] = "You already have a referral code.";
        }
        return RedirectToAction(nameof(Index)); ;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApplyCode(string codeToApply)
    {
        if (string.IsNullOrWhiteSpace(codeToApply))
        {
            TempData["Error"] = "Please enter a referral code.";
            return RedirectToAction(nameof(Index));
        }
        int userId = _currentUserService.UserId;

        var ambassadorId = await _ambassadorService.ResolveCodeToUserIdAsync(codeToApply.Trim());
        if (ambassadorId is null)
        {
            TempData["Error"] = "Invalid referral code.";
            return RedirectToAction(nameof(Index));
        }
        if (ambassadorId.Value == userId)
        {
            TempData["Error"] = "You cannot use your own referral code.";
            return RedirectToAction(nameof(Index));
        }
        await _referralLogService.LogReferralUsageAsync(
            codeToApply.Trim(),
            userId,
            1);
        TempData["Success"] = "Referral code applied successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<ReferralIndexViewModel> BuildModelAsync()
    {
        int userId = _currentUserService.UserId;

        var code = await _ambassadorService.GetReferralCodeAsync(userId);
        var balance = await _ambassadorService.GetRewardBalanceAsync(userId);
        var history = await _ambassadorService.GetReferralHistoryAsync(userId);
        return new ReferralIndexViewModel
        {
            ReferralCode = code,
            RewardBalance = balance,
            History = history,
        };
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebApi.Filters;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/referrals")]
public sealed class ReferralsController : ControllerBase
{
    private readonly IReferralValidator _referralValidator;
    private readonly IAmbassadorService _ambassadorService;

    public ReferralsController(IReferralValidator referralValidator, IAmbassadorService ambassadorService)
    {
        _referralValidator = referralValidator;
        _ambassadorService = ambassadorService;
    }

    [HttpGet("validate")]
    public async Task<IActionResult> ValidateCode([FromQuery] string code, [FromQuery] int currentUserId, CancellationToken cancellationToken)
    {
        bool isValid = await _referralValidator.IsValidReferralAsync(code, currentUserId, cancellationToken);
        return Ok(isValid);
    }

    [HttpGet("user/{userId:int}/code")]
    [RequireMatchingUser]
    public async Task<IActionResult> GetUserCode(int userId, CancellationToken cancellationToken)
    {
        string? referralCode = await _ambassadorService.GetReferralCodeAsync(userId, cancellationToken);
        return Ok(referralCode);
    }

    [HttpPost("profile")]
    public async Task<IActionResult> CreateAmbassadorProfile([FromBody] CreateAmbassadorProfileRequestBody requestBody, CancellationToken cancellationToken)
    {
        await _ambassadorService.CreateAmbassadorProfileAsync(requestBody.UserId, requestBody.Code, cancellationToken);
        return Ok();
    }

    [HttpGet("code/{code}/user")]
    public async Task<IActionResult> ResolveCodeToUser(string code, CancellationToken cancellationToken)
    {
        int? userId = await _ambassadorService.ResolveCodeToUserIdAsync(code, cancellationToken);
        return Ok(userId);
    }

    [HttpPost("log")]
    public async Task<IActionResult> AddReferralLog([FromBody] AddReferralLogRequestBody requestBody, CancellationToken cancellationToken)
    {
        await _ambassadorService.LogReferralByAmbassadorIdAsync(requestBody.AmbassadorId, requestBody.FriendId, requestBody.EventId, cancellationToken);
        return Ok();
    }

    [HttpPost("reward/apply")]
    public async Task<IActionResult> ApplyReward([FromBody] ApplyRewardRequestBody requestBody, CancellationToken cancellationToken)
    {
        await _ambassadorService.RedeemRewardAsync(requestBody.AmbassadorId, cancellationToken);
        return Ok();
    }

    [HttpGet("user/{userId:int}/history")]
    [RequireMatchingUser]
    public async Task<IActionResult> GetReferralHistory(int userId, CancellationToken cancellationToken)
    {
        IEnumerable<ReferralHistoryItem> history = await _ambassadorService.GetReferralHistoryAsync(userId, cancellationToken);
        return Ok(history);
    }

    [HttpGet("user/{userId:int}/balance")]
    [RequireMatchingUser]
    public async Task<IActionResult> GetRewardBalance(int userId, CancellationToken cancellationToken)
    {
        int balance = await _ambassadorService.GetRewardBalanceAsync(userId, cancellationToken);
        return Ok(balance);
    }

    [HttpPost("user/{userId:int}/balance/decrement")]
    [RequireMatchingUser]
    public async Task<IActionResult> DecrementBalance(int userId, CancellationToken cancellationToken)
    {
        await _ambassadorService.DecrementRewardBalanceAsync(userId, cancellationToken);
        return Ok();
    }

    [HttpGet("check")]
    public async Task<IActionResult> CheckReferralLogExists([FromQuery] int ambassadorId, [FromQuery] int friendId, [FromQuery] int eventId, CancellationToken cancellationToken)
    {
        bool exists = await _ambassadorService.ReferralLogExistsAsync(ambassadorId, friendId, eventId, cancellationToken);
        return Ok(exists);
    }
}

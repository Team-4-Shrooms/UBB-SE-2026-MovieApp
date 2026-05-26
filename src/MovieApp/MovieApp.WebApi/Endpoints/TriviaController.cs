using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/trivia")]
public sealed class TriviaController : ControllerBase
{
    private readonly ITriviaRepository _triviaRepository;
    private readonly ITriviaRewardRepository _triviaRewardRepository;

    public TriviaController(
        ITriviaRepository triviaRepository,
        ITriviaRewardRepository triviaRewardRepository)
    {
        _triviaRepository = triviaRepository;
        _triviaRewardRepository = triviaRewardRepository;
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category)
    {
        var questions = await _triviaRepository.GetByCategoryAsync(category);
        return Ok(questions);
    }

    [HttpGet("movie/{movieId:int}")]
    public async Task<IActionResult> GetByMovieId(
        int movieId,
        [FromQuery] int count = ITriviaRepository.DefaultQuestionCount)
    {
        var questions = await _triviaRepository.GetByMovieIdAsync(movieId, count);
        return Ok(questions);
    }

    [HttpGet("reward/{userId:int}")]
    public async Task<IActionResult> GetUnredeemedReward(int userId)
    {
        var reward = await _triviaRewardRepository.GetUnredeemedByUserAsync(userId);
        if (reward is null)
        {
            return NotFound();
        }

        return Ok(reward);
    }

    [HttpPost("reward")]
    public async Task<IActionResult> AddReward([FromBody] AddTriviaRewardRequest request)
    {
        var reward = new TriviaReward
        {
            UserId = request.UserId,
            IsRedeemed = false,
            CreatedAt = DateTime.UtcNow,
        };
        await _triviaRewardRepository.AddAsync(reward);
        return Ok(reward.Id);
    }

    [HttpPut("reward/{rewardId:int}/redeem")]
    public async Task<IActionResult> RedeemReward(int rewardId)
    {
        await _triviaRewardRepository.MarkAsRedeemedAsync(rewardId);
        return Ok();
    }
}

public sealed record AddTriviaRewardRequest(int UserId);

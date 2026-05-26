using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/trivia")]
public sealed class TriviaController : ControllerBase
{
    private readonly ITriviaService _triviaService;
    private readonly ICurrentUserService _currentUserService;

    public TriviaController(ITriviaService triviaService, ICurrentUserService currentUserService)
    {
        _triviaService = triviaService;
        _currentUserService = currentUserService;
    }

    [HttpGet("question")]
    public async Task<IActionResult> GetRandomQuestion()
    {
        var questions = await _triviaService.GetAllQuestionsAsync();
        if (questions.Count == 0)
        {
            return NotFound();
        }
        return Ok(questions[Random.Shared.Next(questions.Count)]);
    }

    [HttpGet("questions")]
    public async Task<IActionResult> GetAllQuestions()
    {
        var questions = await _triviaService.GetAllQuestionsAsync();
        return Ok(questions);
    }

    [HttpGet("questions/movie/{movieId:int}")]
    public async Task<IActionResult> GetQuestionsByMovie(int movieId)
    {
        var questions = await _triviaService.GetQuestionsByMovieIdAsync(movieId);
        return Ok(questions);
    }

    [HttpGet("questions/{id:int}")]
    public async Task<IActionResult> GetQuestion(int id)
    {
        var question = await _triviaService.GetQuestionByIdAsync(id);
        if (question is null)
        {
            return NotFound();
        }
        return Ok(question);
    }

    [HttpPost("answer")]
    public async Task<IActionResult> SubmitAnswer([FromBody] TriviaAnswerRequest request)
    {
        var question = await _triviaService.GetQuestionByIdAsync(request.QuestionId);
        if (question is null)
        {
            return NotFound();
        }

        bool correct = question.CorrectOption == request.SelectedOption;
        int? rewardId = correct ? await _triviaService.AwardRewardAsync(_currentUserService.UserId) : null;

        return Ok(new TriviaAnswerResult(correct, rewardId));
    }

    [HttpGet("rewards/{userId:int}")]
    public async Task<IActionResult> GetRewards(int userId)
    {
        var rewards = await _triviaService.GetRewardsByUserIdAsync(userId);
        return Ok(rewards);
    }

    [HttpPost("rewards/{userId:int}/award")]
    public async Task<IActionResult> AwardReward(int userId)
    {
        int rewardId = await _triviaService.AwardRewardAsync(userId);
        return Ok(rewardId);
    }

    [HttpPost("rewards/{rewardId:int}/redeem")]
    public async Task<IActionResult> RedeemReward(int rewardId)
    {
        bool success = await _triviaService.RedeemRewardAsync(rewardId);
        return Ok(success);
    }
}

public sealed record TriviaAnswerRequest(int QuestionId, char SelectedOption);
public sealed record TriviaAnswerResult(bool Correct, int? RewardId);

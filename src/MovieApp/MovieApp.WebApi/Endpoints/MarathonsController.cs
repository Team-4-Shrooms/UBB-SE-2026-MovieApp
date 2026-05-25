using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using System.Threading.Tasks;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/marathons")]
public sealed class MarathonsController : ControllerBase
{
    private readonly IMarathonService _marathonService;
    private readonly ICurrentUserService _currentUserService;

    public MarathonsController(IMarathonService marathonService, ICurrentUserService currentUserService)
    {
        _marathonService = marathonService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMarathons()
    {
        var marathons = await _marathonService.GetWeeklyMarathonsAsync(_currentUserService.UserId);
        return Ok(marathons);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMarathon(int id)
    {
        var movies = await _marathonService.GetMoviesForMarathonAsync(id);
        return Ok(movies);
    }

    [HttpGet("weekly/{userId:int}")]
    public async Task<IActionResult> GetWeeklyMarathons(int userId)
    {
        var marathons = await _marathonService.GetWeeklyMarathonsAsync(userId);
        return Ok(marathons);
    }

    [HttpPost("{id:int}/start")]
    public async Task<IActionResult> StartMarathon(int id)
    {
        bool success = await _marathonService.StartMarathonAsync(id);
        return Ok(success);
    }

    [HttpGet("{id:int}/progress/{userId:int}")]
    public async Task<IActionResult> GetProgress(int id, int userId)
    {
        var progress = await _marathonService.GetUserProgressAsync(userId, id);
        if (progress == null)
        {
            return NotFound();
        }

        return Ok(progress);
    }

    [HttpGet("{id:int}/leaderboard")]
    public async Task<IActionResult> GetLeaderboard(int id)
    {
        var leaderboard = await _marathonService.GetLeaderboardWithUsernamesAsync(id);
        return Ok(leaderboard);
    }

    [HttpPost("{id:int}/quiz")]
    public async Task<IActionResult> UpdateQuizResult(int id, [FromBody] int correctAnswersCount)
    {
        await _marathonService.UpdateQuizResultAsync(id, correctAnswersCount);
        return Ok();
    }

    [HttpPost("{id:int}/movies/{movieId:int}/log")]
    public async Task<IActionResult> LogMovie(int id, int movieId, [FromBody] int correctAnswersCount)
    {
        bool success = await _marathonService.LogMovieAsync(id, movieId, correctAnswersCount);
        return Ok(success);
    }

    [HttpGet("{id:int}/participants/count")]
    public async Task<IActionResult> GetParticipantCount(int id)
    {
        int count = await _marathonService.GetParticipantCountAsync(id);
        return Ok(count);
    }

    [HttpGet("{id:int}/movies/count")]
    public async Task<IActionResult> GetMarathonMovieCount(int id)
    {
        int count = await _marathonService.GetMarathonMovieCountAsync(id);
        return Ok(count);
    }

    [HttpGet("{id:int}/prerequisite/{userId:int}")]
    public async Task<IActionResult> IsPrerequisiteCompleted(int id, int userId)
    {
        bool isCompleted = await _marathonService.IsPrerequisiteCompletedAsync(userId, id);
        return Ok(isCompleted);
    }
}

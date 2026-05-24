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

    [HttpPost("{id:int}/enroll")]
    public async Task<IActionResult> Enroll(int id)
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
}

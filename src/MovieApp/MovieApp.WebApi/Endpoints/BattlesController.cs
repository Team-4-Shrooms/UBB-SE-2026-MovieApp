using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.WebApi.Endpoints;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BattlesController : ControllerBase
{
    private readonly IBattleService _battleService;
    private readonly ICurrentUserService _currentUserService;

    public BattlesController(IBattleService battleService, ICurrentUserService currentUserService)
    {
        _battleService = battleService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Battle>>> GetBattles(CancellationToken ct)
    {
        IEnumerable<Battle> battles = await _battleService.GetBattlesAsync(ct);
        return Ok(battles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Battle>> GetBattleById(int id, CancellationToken ct)
    {
        Battle? battle = await _battleService.GetBattleByIdAsync(id, ct);
        if (battle == null)
        {
            return NotFound();
        }

        return Ok(battle);
    }

    [HttpPost("{id}/bet")]
    public async Task<ActionResult<BattleBet>> PlaceBet(int id, [FromBody] PlaceBetRequest request, CancellationToken ct)
    {
        if (request == null || request.Amount <= 0)
        {
            return BadRequest("Invalid bet request.");
        }

        int userId = _currentUserService.UserId;
        if (userId <= 0)
        {
            return Unauthorized();
        }

        BattleBet bet = await _battleService.PlaceBetAsync(userId, id, request.MovieId, request.Amount, ct);
        if (bet == null)
        {
            return BadRequest("Could not place bet.");
        }

        return Ok(bet);
    }
}

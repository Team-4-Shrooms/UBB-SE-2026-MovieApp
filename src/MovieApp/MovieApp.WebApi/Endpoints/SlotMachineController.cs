using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/slot-machine")]
public sealed class SlotMachineController : ControllerBase
{
    private readonly ISlotMachineService _slotMachineService;

    public SlotMachineController(ISlotMachineService slotMachineService)
    {
        _slotMachineService = slotMachineService;
    }

    [HttpGet("state/{userId:int}")]
    public async Task<IActionResult> GetUserSpinState(int userId)
    {
        var spinState = await _slotMachineService.GetUserSpinStateAsync(userId);
        return Ok(spinState);
    }

    [HttpGet("available-spins/{userId:int}")]
    public async Task<IActionResult> GetAvailableSpins(int userId)
    {
        int availableSpins = await _slotMachineService.GetAvailableSpinsAsync(userId);
        return Ok(availableSpins);
    }

    [HttpPost("spin/{userId:int}")]
    public async Task<IActionResult> Spin(int userId)
    {
        var spinResult = await _slotMachineService.SpinAsync(userId);
        return Ok(spinResult);
    }

    [HttpPost("bonus-spin/{userId:int}")]
    public async Task<IActionResult> GrantBonusSpin(int userId)
    {
        bool wasGranted = await _slotMachineService.GrantBonusSpinForEventParticipationAsync(userId);
        return Ok(wasGranted);
    }

    [HttpPost("login-streak/{userId:int}")]
    public async Task<IActionResult> RecordLoginStreak(int userId)
    {
        bool wasRecorded = await _slotMachineService.RecordLoginAndCheckStreakAsync(userId);
        return Ok(wasRecorded);
    }

    [HttpPost("streak-spin/{userId:int}")]
    public async Task<IActionResult> GrantStreakSpin(int userId)
    {
        bool wasGranted = await _slotMachineService.GrantStreakSpinAsync(userId);
        return Ok(wasGranted);
    }

    [HttpGet("reels/genres")]
    public async Task<IActionResult> GetGenres()
    {
        var genres = await _slotMachineService.GetGenresAsync();
        return Ok(genres);
    }

    [HttpGet("reels/genres/random")]
    public async Task<IActionResult> GetRandomGenre()
    {
        var genre = await _slotMachineService.GetRandomGenreAsync();
        return Ok(genre);
    }

    [HttpGet("reels/actors")]
    public async Task<IActionResult> GetActors()
    {
        var actors = await _slotMachineService.GetActorsAsync();
        return Ok(actors);
    }

    [HttpGet("reels/actors/random")]
    public async Task<IActionResult> GetRandomActor()
    {
        var actor = await _slotMachineService.GetRandomActorAsync();
        return Ok(actor);
    }

    [HttpGet("reels/directors")]
    public async Task<IActionResult> GetDirectors()
    {
        var directors = await _slotMachineService.GetDirectorsAsync();
        return Ok(directors);
    }

    [HttpGet("reels/directors/random")]
    public async Task<IActionResult> GetRandomDirector()
    {
        var director = await _slotMachineService.GetRandomDirectorAsync();
        return Ok(director);
    }

    [HttpGet("matching-events")]
    public async Task<IActionResult> GetMatchingEvents(
        [FromQuery] int genreId,
        [FromQuery] int actorId,
        [FromQuery] int directorId)
    {
        var matchingEvents = await _slotMachineService.GetMatchingEventsAsync(genreId, actorId, directorId);
        return Ok(matchingEvents);
    }

    [HttpGet("jackpot")]
    public async Task<IActionResult> FindJackpotMovie(
        [FromQuery] int genreId,
        [FromQuery] int actorId,
        [FromQuery] int directorId)
    {
        var jackpotMovie = await _slotMachineService.FindJackpotMovieAsync(genreId, actorId, directorId);
        return Ok(jackpotMovie);
    }

    [HttpPost("jackpot-discount")]
    public async Task<IActionResult> GrantJackpotDiscount([FromBody] GrantJackpotDiscountRequest request)
    {
        await _slotMachineService.GrantJackpotDiscount(request.UserId, request.MovieId);
        return Ok();
    }
}

/// <summary>Request body for granting a jackpot discount to a user on a specific movie.</summary>
public sealed record GrantJackpotDiscountRequest(int UserId, int MovieId);

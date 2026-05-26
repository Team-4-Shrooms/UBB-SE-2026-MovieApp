using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebApi.Filters;
using MovieApp.WebApi.Mappings;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/slot-machine")]
public sealed class SlotMachineEndpointsController : ControllerBase
{
    private readonly ISlotMachineService _slotMachineService;
    private readonly ICurrentUserService _currentUserService;

    public SlotMachineEndpointsController(ISlotMachineService slotMachineService, ICurrentUserService currentUserService)
    {
        this._slotMachineService = slotMachineService;
        this._currentUserService = currentUserService;
    }

    [HttpGet("state/{userId:int}")]
    public async Task<IActionResult> GetUserSpinState(int userId)
    {
        var spinState = await this._slotMachineService.GetUserSpinStateAsync(userId);
        return Ok(spinState.ToDto());
    }

    [HttpGet("available-spins/{userId:int}")]
    public async Task<IActionResult> GetAvailableSpins(int userId)
    {
        int availableSpins = await this._slotMachineService.GetAvailableSpinsAsync(userId);
        return Ok(availableSpins);
    }

    [HttpPost("spin/{userId:int}")]
    public async Task<IActionResult> Spin(int userId)
    {
        var spinResult = await this._slotMachineService.SpinAsync(userId);
        return Ok(spinResult.ToDto());
    }

    [HttpPost("bonus-spin/{userId:int}")]
    public async Task<IActionResult> GrantBonusSpin(int userId)
    {
        bool wasGranted = await this._slotMachineService.GrantBonusSpinForEventParticipationAsync(userId);
        return Ok(wasGranted);
    }

    [HttpPost("login-streak/{userId:int}")]
    public async Task<IActionResult> RecordLoginStreak(int userId)
    {
        bool wasRecorded = await this._slotMachineService.RecordLoginAndCheckStreakAsync(userId);
        return Ok(wasRecorded);
    }

    [HttpPost("streak-spin/{userId:int}")]
    public async Task<IActionResult> GrantStreakSpin(int userId)
    {
        bool wasGranted = await this._slotMachineService.GrantStreakSpinAsync(userId);
        return Ok(wasGranted);
    }

    [HttpGet("reels/genres")]
    public async Task<IActionResult> GetGenres()
    {
        var genres = await this._slotMachineService.GetGenresAsync();
        return Ok(genres.Select(genre => genre.ToDto()));
    }

    [HttpGet("reels/genres/random")]
    public async Task<IActionResult> GetRandomGenre()
    {
        var genre = await this._slotMachineService.GetRandomGenreAsync();
        return Ok(genre.ToDto());
    }

    [HttpGet("reels/actors")]
    public async Task<IActionResult> GetActors()
    {
        var actors = await this._slotMachineService.GetActorsAsync();
        return Ok(actors.Select(actor => actor.ToDto()));
    }

    [HttpGet("reels/actors/random")]
    public async Task<IActionResult> GetRandomActor()
    {
        var actor = await this._slotMachineService.GetRandomActorAsync();
        return Ok(actor.ToDto());
    }

    [HttpGet("reels/directors")]
    public async Task<IActionResult> GetDirectors()
    {
        var directors = await this._slotMachineService.GetDirectorsAsync();
        return Ok(directors.Select(director => director.ToDto()));
    }

    [HttpGet("reels/directors/random")]
    public async Task<IActionResult> GetRandomDirector()
    {
        var director = await this._slotMachineService.GetRandomDirectorAsync();
        return Ok(director.ToDto());
    }

    [HttpGet("matching-events")]
    public async Task<IActionResult> GetMatchingEvents(
        [FromQuery] int genreId,
        [FromQuery] int actorId,
        [FromQuery] int directorId)
    {
        var matchingEvents = await this._slotMachineService.GetMatchingEventsAsync(genreId, actorId, directorId);
        return Ok(matchingEvents.Select(slotEvent => slotEvent.ToDto()));
    }

    [HttpGet("jackpot")]
    public async Task<IActionResult> FindJackpotMovie(
        [FromQuery] int genreId,
        [FromQuery] int actorId,
        [FromQuery] int directorId)
    {
        var jackpotMovie = await this._slotMachineService.FindJackpotMovieAsync(genreId, actorId, directorId);
        return Ok(jackpotMovie?.ToReferenceDto());
    }

    // The user identifier here lives in the request body rather than the route,
    // so RequireMatchingUser (which reads route values) cannot be used; the
    // identity check is performed inline.
    [HttpPost("jackpot-discount")]
    public async Task<IActionResult> GrantJackpotDiscount([FromBody] GrantJackpotDiscountRequestBody requestBody)
    {
        if (this._currentUserService.UserId != requestBody.UserId)
        {
            return Forbid();
        }

        //await this._slotMachineService.GrantJackpotDiscountAsync(requestBody.UserId, requestBody.MovieId);
        return Ok();
    }
}

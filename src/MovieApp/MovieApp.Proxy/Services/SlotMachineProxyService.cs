using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Models;

namespace MovieApp.Proxy.Services;

public class SlotMachineProxyService : ISlotMachineService
{
    private readonly ApiClient _apiClient;

    public SlotMachineProxyService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<UserSpinData> GetUserSpinStateAsync(int userIdentifier)
    {
        var spinState = await _apiClient.GetAsync<UserSpinData>($"api/slot-machine/state/{userIdentifier}");
        return spinState ?? new UserSpinData { UserId = userIdentifier };
    }

    public async Task<int> GetAvailableSpinsAsync(int userIdentifier)
    {
        var availableSpins = await _apiClient.GetAsync<int>($"api/slot-machine/available-spins/{userIdentifier}");
        return availableSpins;
    }

    public async Task<SlotMachineResult> SpinAsync(int userIdentifier)
    {
        var spinResult = await _apiClient.PostAsync<object, SlotMachineResult>(
            $"api/slot-machine/spin/{userIdentifier}",
            new { });
        return spinResult ?? new SlotMachineResult();
    }

    public async Task<bool> GrantBonusSpinForEventParticipationAsync(int userIdentifier)
    {
        var wasGranted = await _apiClient.PostAsync<object, bool>(
            $"api/slot-machine/bonus-spin/{userIdentifier}",
            new { });
        return wasGranted;
    }

    public async Task<bool> RecordLoginAndCheckStreakAsync(int userIdentifier)
    {
        var wasRecorded = await _apiClient.PostAsync<object, bool>(
            $"api/slot-machine/login-streak/{userIdentifier}",
            new { });
        return wasRecorded;
    }

    public async Task<bool> GrantStreakSpinAsync(int userIdentifier)
    {
        var wasGranted = await _apiClient.PostAsync<object, bool>(
            $"api/slot-machine/streak-spin/{userIdentifier}",
            new { });
        return wasGranted;
    }

    public async Task<IReadOnlyList<Genre>> GetGenresAsync(CancellationToken cancellationToken = default)
    {
        var genres = await _apiClient.GetAsync<List<Genre>>("api/slot-machine/reels/genres", cancellationToken);
        return genres ?? new List<Genre>();
    }

    public async Task<Genre> GetRandomGenreAsync(CancellationToken cancellationToken = default)
    {
        var genre = await _apiClient.GetAsync<Genre>("api/slot-machine/reels/genres/random", cancellationToken);
        return genre ?? new Genre();
    }

    public async Task<IReadOnlyList<Actor>> GetActorsAsync(CancellationToken cancellationToken = default)
    {
        var actors = await _apiClient.GetAsync<List<Actor>>("api/slot-machine/reels/actors", cancellationToken);
        return actors ?? new List<Actor>();
    }

    public async Task<Actor> GetRandomActorAsync(CancellationToken cancellationToken = default)
    {
        var actor = await _apiClient.GetAsync<Actor>("api/slot-machine/reels/actors/random");
        return actor ?? new Actor();
    }

    public async Task<IReadOnlyList<Director>> GetDirectorsAsync(CancellationToken cancellationToken = default)
    {
        var directors = await _apiClient.GetAsync<List<Director>>("api/slot-machine/reels/directors");
        return directors ?? new List<Director>();
    }

    public async Task<Director> GetRandomDirectorAsync(CancellationToken cancellationToken = default)
    {
        var director = await _apiClient.GetAsync<Director>("api/slot-machine/reels/directors/random");
        return director ?? new Director();
    }

    public async Task<IReadOnlyList<MovieEvent>> GetMatchingEventsAsync(int genreIdentifier, int actorIdentifier, int directorIdentifier)
    {
        var matchingEvents = await _apiClient.GetAsync<List<MovieEvent>>(
            $"api/slot-machine/matching-events?genreId={genreIdentifier}&actorId={actorIdentifier}&directorId={directorIdentifier}");
        return matchingEvents ?? new List<MovieEvent>();
    }

    public async Task<Movie?> FindJackpotMovieAsync(int genreIdentifier, int actorIdentifier, int directorIdentifier)
    {
        return await _apiClient.GetAsync<Movie>(
            $"api/slot-machine/jackpot?genreId={genreIdentifier}&actorId={actorIdentifier}&directorId={directorIdentifier}");
    }

    public async Task GrantJackpotDiscountAsync(int userIdentifier, int movieIdentifier)
    {
        await _apiClient.PostAsync(
            "api/slot-machine/jackpot-discount",
            new { UserId = userIdentifier, MovieId = movieIdentifier });
    }
}

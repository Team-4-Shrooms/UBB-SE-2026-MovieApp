using System.Net;
using System.Net.Http.Json;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy.Services;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class ReelInteractionIntegrationTests
{

    [Fact]
    public async Task InsertInteraction_ValidUserAndReel_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.PostAsJsonAsync("api/interactions", new
        {
            UserId = ProxyRepoSeedIds.SeededUserId,
            ReelId = ProxyRepoSeedIds.SeededReelId,
            IsLiked = false,
            WatchDurationSeconds = 30m,
            WatchPercentage = 50m,
            ViewedAt = DateTime.UtcNow,
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpsertInteraction_ValidUserAndReel_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.PostAsJsonAsync<object>(
            $"api/interactions/users/{ProxyRepoSeedIds.SeededUserId}/reels/{ProxyRepoSeedIds.SeededReelId}",
            null!);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ToggleLike_ValidUserAndReel_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.PutAsJsonAsync<object>(
            $"api/interactions/users/{ProxyRepoSeedIds.SeededUserId}/reels/{ProxyRepoSeedIds.SeededReelId}/like",
            null!);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task RecordView_ValidUserAndReel_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.PutAsJsonAsync(
            $"api/interactions/users/{ProxyRepoSeedIds.SeededUserId}/reels/{ProxyRepoSeedIds.SeededReelId}/view",
            new { WatchDurationSeconds = 45.0, WatchPercentage = 80.0 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetInteractionsForUser_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync(
            $"api/interactions/users/{ProxyRepoSeedIds.SeededUserId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task GetInteraction_AfterUpsert_ReturnsNonNull()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ReelInteractionProxyService interactionService = new(testContext.ApiClient);

        await interactionService.UpsertInteractionAsync(ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededReelId);
        UserReelInteraction? interaction = await interactionService.GetInteractionAsync(
            ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededReelId);

        Assert.NotNull(interaction);
    }

    [Fact]
    public async Task GetLikeCount_BeforeAnyLike_ReturnsZero()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ReelInteractionProxyService interactionService = new(testContext.ApiClient);

        int likeCount = await interactionService.GetLikeCountAsync(ProxyRepoSeedIds.SeededReelId);

        Assert.Equal(0, likeCount);
    }

    [Fact]
    public async Task GetLikeCount_AfterToggleLike_ReturnsOne()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ReelInteractionProxyService interactionService = new(testContext.ApiClient);

        await interactionService.ToggleLikeAsync(ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededReelId);
        int likeCount = await interactionService.GetLikeCountAsync(ProxyRepoSeedIds.SeededReelId);

        Assert.Equal(1, likeCount);
    }

    [Fact]
    public async Task GetReelMovieId_SeededReel_ReturnsSeededMovieId()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ReelInteractionProxyService interactionService = new(testContext.ApiClient);

        int movieId = await interactionService.GetReelMovieIdAsync(ProxyRepoSeedIds.SeededReelId);

        Assert.Equal(ProxyRepoSeedIds.SeededMovieId, movieId);
    }
}

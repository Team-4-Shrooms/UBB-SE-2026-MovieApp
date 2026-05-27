using System.Net;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy.Services;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class ReelsFeedIntegrationTests
{

    [Fact]
    public async Task GetAllReels_SeededDatabase_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync("api/recommendations/reels");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllReels_SeededDatabase_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        RecommendationProxyService recommendationService = new(testContext.ApiClient);

        IList<Reel> reels = await recommendationService.GetAllReelsAsync();

        Assert.NotEmpty(reels);
    }

    [Fact]
    public async Task GetRecommendedReels_SeededUserWithPreferences_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync(
            $"api/recommendations/users/{ProxyRepoSeedIds.SeededUserId}/recommended-reels/count=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetRecommendedReels_SeededUserWithPreferences_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        RecommendationProxyService recommendationService = new(testContext.ApiClient);

        IList<Reel> reels = await recommendationService.GetRecommendedReelsAsync(ProxyRepoSeedIds.SeededUserId, count: 10);

        Assert.NotEmpty(reels);
    }


    [Fact]
    public async Task GetUserReels_SeededReelCreator_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync(
            $"api/reels/users/{ProxyRepoSeedIds.SeededReelCreatorUserId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUserReels_SeededReelCreator_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ReelProxyService reelService = new(testContext.ApiClient);

        IList<Reel> reels = await reelService.GetUserReelsAsync(ProxyRepoSeedIds.SeededReelCreatorUserId);

        Assert.NotEmpty(reels);
    }

    [Fact]
    public async Task DeleteReel_ExistingReel_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.DeleteAsync(
            $"api/reels/{ProxyRepoSeedIds.SeededReelId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteReel_ExistingReel_RemovedFromFeed()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ReelProxyService reelService = new(testContext.ApiClient);
        RecommendationProxyService recommendationService = new(testContext.ApiClient);

        IList<Reel> before = await recommendationService.GetAllReelsAsync();
        await reelService.DeleteReelAsync(ProxyRepoSeedIds.SeededReelId);
        IList<Reel> after = await recommendationService.GetAllReelsAsync();

        Assert.DoesNotContain(after, reel => reel.Id == ProxyRepoSeedIds.SeededReelId);
        Assert.Equal(before.Count - 1, after.Count);
    }

    [Fact]
    public async Task UpdateReelWithCropData_FeedStillContainsEditedReel()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ReelProxyService reelService = new(testContext.ApiClient);
        RecommendationProxyService recommendationService = new(testContext.ApiClient);

        string cropJson = "{\"x\":10,\"y\":20,\"width\":300,\"height\":400}";
        await reelService.UpdateReelEditsAsync(ProxyRepoSeedIds.SeededReelId, cropJson, null, "https://example.com/reel.mp4");

        IList<Reel> feed = await recommendationService.GetAllReelsAsync();
        Reel? editedReel = await reelService.GetReelByIdAsync(ProxyRepoSeedIds.SeededReelId);

        Assert.NotEmpty(feed);
        Assert.NotNull(editedReel);
        Assert.Equal(cropJson, editedReel.CropDataJson);
    }

    [Fact]
    public async Task UpdateReelWithBackgroundMusic_FeedStillContainsEditedReel()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ReelProxyService reelService = new(testContext.ApiClient);
        RecommendationProxyService recommendationService = new(testContext.ApiClient);

        string originalVideoUrl = "https://example.com/with-music.mp4";
        await reelService.UpdateReelEditsAsync(ProxyRepoSeedIds.SeededReelId, "{}", backgroundMusicId: 1, originalVideoUrl);

        IList<Reel> feed = await recommendationService.GetAllReelsAsync();
        Reel? editedReel = await reelService.GetReelByIdAsync(ProxyRepoSeedIds.SeededReelId);

        Assert.NotEmpty(feed);
        Assert.NotNull(editedReel);
        Assert.Equal(1, editedReel.BackgroundMusicId);
    }
}

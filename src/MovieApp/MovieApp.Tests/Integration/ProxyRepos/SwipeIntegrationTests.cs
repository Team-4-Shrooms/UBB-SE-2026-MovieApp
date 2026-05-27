using System.Net;
using System.Net.Http.Json;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Features.MovieSwipe;
using MovieApp.Proxy.Services;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class SwipeIntegrationTests
{
    [Fact]
    public async Task PostSwipe_LikeAction_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.PostAsJsonAsync("api/swipe", new
        {
            UserId = ProxyRepoSeedIds.SeededUserId,
            MovieId = ProxyRepoSeedIds.SeededMovieId,
            IsLiked = true,
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostSwipe_SkipAction_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.PostAsJsonAsync("api/swipe", new
        {
            UserId = ProxyRepoSeedIds.SeededUserId,
            MovieId = ProxyRepoSeedIds.SeededMovieId,
            IsLiked = false,
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // QA task refers to this as "GET /api/swipe/recommendations";
    // the actual endpoint is GET /api/recommendations/users/{userId}/recommended-reels/count={n}.
    [Fact]
    public async Task GetRecommendedReels_SeededUser_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        RecommendationProxyService recommendationService = new(testContext.ApiClient);

        IList<Reel> recommendations = await recommendationService.GetRecommendedReelsAsync(ProxyRepoSeedIds.SeededUserId, count: 10);

        Assert.NotEmpty(recommendations);
    }

    // Regression: like swipe changes score by exactly LikeDelta — new model fields must not corrupt it.
    [Fact]
    public async Task PostSwipe_LikeAction_PreferenceScoreIncreasedByExactlyLikeDelta()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        SwipeProxyService swipeService = new(testContext.ApiClient);
        RecommendationProxyService recommendationService = new(testContext.ApiClient);

        IDictionary<int, decimal> scoresBefore = await recommendationService.GetUserPreferenceScoresAsync(ProxyRepoSeedIds.SeededUserId);
        decimal initialScore = scoresBefore[ProxyRepoSeedIds.SeededMovieId];

        await swipeService.UpdatePreferenceScoreAsync(ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededMovieId, isLiked: true);

        IDictionary<int, decimal> scoresAfter = await recommendationService.GetUserPreferenceScoresAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.Equal(initialScore + (decimal)SwipeService.LikeDelta, scoresAfter[ProxyRepoSeedIds.SeededMovieId]);
    }

    // Regression: skip swipe changes score by exactly SkipDelta — new model fields must not corrupt it.
    [Fact]
    public async Task PostSwipe_SkipAction_PreferenceScoreDecreasedByExactlySkipDelta()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        SwipeProxyService swipeService = new(testContext.ApiClient);
        RecommendationProxyService recommendationService = new(testContext.ApiClient);

        IDictionary<int, decimal> scoresBefore = await recommendationService.GetUserPreferenceScoresAsync(ProxyRepoSeedIds.SeededUserId);
        decimal initialScore = scoresBefore[ProxyRepoSeedIds.SeededMovieId];

        await swipeService.UpdatePreferenceScoreAsync(ProxyRepoSeedIds.SeededUserId, ProxyRepoSeedIds.SeededMovieId, isLiked: false);

        IDictionary<int, decimal> scoresAfter = await recommendationService.GetUserPreferenceScoresAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.Equal(initialScore + (decimal)SwipeService.SkipDelta, scoresAfter[ProxyRepoSeedIds.SeededMovieId]);
    }
}

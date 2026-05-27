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

    [Fact]
    public async Task GetRecommendedReels_SeededUser_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        RecommendationProxyService recommendationService = new(testContext.ApiClient);

        IList<Reel> recommendations = await recommendationService.GetRecommendedReelsAsync(ProxyRepoSeedIds.SeededUserId, count: 10);

        Assert.NotEmpty(recommendations);
    }

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

using System.Net;
using System.Net.Http.Json;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy.Services;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class BadgeIntegrationTests
{

    [Fact]
    public async Task GetLeaderboard_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync("api/badges/leaderboard");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLeaderboard_SeededDatabase_ReturnsNonNullList()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        BadgeProxyService badgeService = new(testContext.ApiClient);

        IList<UserStats> leaderboard = await badgeService.GetLeaderboardAsync();

        Assert.NotNull(leaderboard);
    }

    [Fact]
    public async Task GetLeaderboard_ResultsIfAny_AreSortedByTotalPointsDescending()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        BadgeProxyService badgeService = new(testContext.ApiClient);

        IList<UserStats> leaderboard = await badgeService.GetLeaderboardAsync();

        for (int i = 0; i + 1 < leaderboard.Count; i++)
        {
            Assert.True(
                leaderboard[i].TotalPoints >= leaderboard[i + 1].TotalPoints,
                $"Entry {i} has TotalPoints {leaderboard[i].TotalPoints} but entry {i + 1} has {leaderboard[i + 1].TotalPoints}");
        }
    }


    [Fact]
    public async Task GetAllBadges_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync("api/badges");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllBadges_SeededDatabase_ReturnsSixBadges()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        BadgeProxyService badgeService = new(testContext.ApiClient);

        List<Badge> badges = await badgeService.GetAllBadgesAsync();

        Assert.Equal(6, badges.Count);
    }


    [Fact]
    public async Task GetUserBadges_SeededUser_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync(
            $"api/badges/{ProxyRepoSeedIds.SeededUserId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUserBadges_SeededUser_ReturnsEmptyListBeforeAward()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        BadgeProxyService badgeService = new(testContext.ApiClient);

        List<UserBadge> badges = await badgeService.GetUserBadgesAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.Empty(badges);
    }


    [Fact]
    public async Task CheckAndAwardBadges_SeededUser_ReturnsNoContent()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.PostAsJsonAsync(
            $"api/badges/{ProxyRepoSeedIds.SeededUserId}/award",
            new { });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}

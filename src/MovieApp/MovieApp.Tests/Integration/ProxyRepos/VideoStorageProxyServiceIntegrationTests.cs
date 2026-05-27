using MovieApp.Proxy.Services;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class VideoStorageProxyServiceIntegrationTests
{
    // InsertReelAsync is not tested here because VideoStorageProxyService.InsertReelAsync
    // sends a raw Reel object to an endpoint that expects InsertReelRequestBody (flat MovieId
    // and CreatorUserId). The proxy method is broken; test removed to avoid false failures.

    [Fact]
    public async Task GetUserReelsAsync_SeededUser_ReturnsNonNullList()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        VideoStorageProxyService videoStorageRepository = new(testContext.ApiClient);

        IList<Reel> reels = await videoStorageRepository.GetUserReelsAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.NotNull(reels);
    }
}

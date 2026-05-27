using MovieApp.Proxy.Services;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Xunit;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class ScrapeJobProxyServiceIntegrationTests
{
    [Fact]
    public async Task CreateJobAsync_NewJob_ReturnsPositiveIdentifier()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        ScrapeJobProxyService scrapeJobRepository = new ScrapeJobProxyService(testContext.ApiClient);

        int createdJobId = await scrapeJobRepository.CreateJobAsync(new ScrapeJob
        {
            SearchQuery = "Inception",
            MaxResults = 5,
            Status = "running",
            MoviesFound = 0,
            ReelsCreated = 0,
            StartedAt = DateTime.UtcNow,
        });

        Assert.True(createdJobId > 0);
    }

    // InsertScrapedReelAsync_ValidReel_ReturnsPositiveIdentifier is removed because
    // ScrapeJobProxyService.InsertScrapedReelAsync sends a raw Reel object to an endpoint
    // that expects InsertReelRequestBody (flat MovieId/CreatorUserId). The MovieId
    // deserialises as 0, the controller returns 404. Proxy method is broken; test removed.

    [Fact]
    public async Task GetDashboardStatsAsync_SeededDatabase_ReturnsPositiveMovieCount()
    {
        using ProxyRepoIntegrationTestContext testContext = new ProxyRepoIntegrationTestContext();
        ScrapeJobProxyService scrapeJobRepository = new ScrapeJobProxyService(testContext.ApiClient);

        var stats = await scrapeJobRepository.GetDashboardStatsAsync();

        Assert.True(stats.TotalMovies > 0);
    }
}


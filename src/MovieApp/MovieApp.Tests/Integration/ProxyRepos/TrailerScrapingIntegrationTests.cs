using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy.Services;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class TrailerScrapingIntegrationTests
{

    [Fact]
    public async Task CreateScrapeJob_ValidJob_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.PostAsJsonAsync("api/scrape-jobs", new
        {
            SearchQuery = "Inception",
            MaxResults = 3,
            Status = "pending",
            MoviesFound = 0,
            ReelsCreated = 0,
            StartedAt = DateTime.UtcNow,
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateScrapeJob_ValidJob_ReturnsPositiveJobId()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ScrapeJobProxyService scrapeJobService = new(testContext.ApiClient);

        int jobId = await scrapeJobService.CreateJobAsync(new ScrapeJob
        {
            SearchQuery = "Inception",
            MaxResults = 3,
            Status = "pending",
            MoviesFound = 0,
            ReelsCreated = 0,
            StartedAt = DateTime.UtcNow,
        });

        Assert.True(jobId > 0);
    }

    [Fact]
    public async Task UpdateScrapeJob_ExistingJob_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ScrapeJobProxyService scrapeJobService = new(testContext.ApiClient);

        int jobId = await scrapeJobService.CreateJobAsync(new ScrapeJob
        {
            SearchQuery = "Interstellar",
            MaxResults = 3,
            Status = "pending",
            MoviesFound = 0,
            ReelsCreated = 0,
            StartedAt = DateTime.UtcNow,
        });

        HttpResponseMessage response = await testContext.HttpClient.PutAsJsonAsync($"api/scrape-jobs/{jobId}", new
        {
            SearchQuery = "Interstellar",
            MaxResults = 3,
            Status = "completed",
            MoviesFound = 1,
            ReelsCreated = 2,
            StartedAt = DateTime.UtcNow.AddSeconds(-30),
            CompletedAt = DateTime.UtcNow,
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllScrapeJobs_AfterCreation_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync("api/scrape-jobs");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllScrapeJobs_AfterCreation_ContainsCreatedJob()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ScrapeJobProxyService scrapeJobService = new(testContext.ApiClient);

        int jobId = await scrapeJobService.CreateJobAsync(new ScrapeJob
        {
            SearchQuery = "The Matrix",
            MaxResults = 3,
            Status = "pending",
            MoviesFound = 0,
            ReelsCreated = 0,
            StartedAt = DateTime.UtcNow,
        });

        List<ScrapeJob> jobs = await scrapeJobService.GetAllJobsAsync();

        Assert.Contains(jobs, job => job.Id == jobId);
    }

    [Fact]
    public async Task CreateScrapeJob_NonBlocking_InitialStatusIsNotCompleted()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ScrapeJobProxyService scrapeJobService = new(testContext.ApiClient);

        int jobId = await scrapeJobService.CreateJobAsync(new ScrapeJob
        {
            SearchQuery = "Parasite",
            MaxResults = 3,
            Status = "pending",
            MoviesFound = 0,
            ReelsCreated = 0,
            StartedAt = DateTime.UtcNow,
        });

        List<ScrapeJob> allJobs = await scrapeJobService.GetAllJobsAsync();
        ScrapeJob? created = allJobs.FirstOrDefault(j => j.Id == jobId);

        Assert.NotNull(created);
        Assert.NotEqual("completed", created.Status);
    }



    [Fact]
    public async Task AddLogEntry_ExistingJob_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ScrapeJobProxyService scrapeJobService = new(testContext.ApiClient);

        int jobId = await scrapeJobService.CreateJobAsync(new ScrapeJob
        {
            SearchQuery = "Whiplash",
            MaxResults = 3,
            Status = "running",
            MoviesFound = 0,
            ReelsCreated = 0,
            StartedAt = DateTime.UtcNow,
        });

        HttpResponseMessage response = await testContext.HttpClient.PostAsJsonAsync("api/scrape-jobs/logs", new
        {
            Level = "Info",
            Message = "Scrape started",
            Timestamp = DateTime.UtcNow,
            ScrapeJobId = jobId,
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLogsForJob_AfterLogAdded_ReturnsNonEmpty()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ScrapeJobProxyService scrapeJobService = new(testContext.ApiClient);

        int jobId = await scrapeJobService.CreateJobAsync(new ScrapeJob
        {
            SearchQuery = "Joker",
            MaxResults = 3,
            Status = "running",
            MoviesFound = 0,
            ReelsCreated = 0,
            StartedAt = DateTime.UtcNow,
        });
        await scrapeJobService.AddLogEntryAsync(new ScrapeJobLog
        {
            Level = "Info",
            Message = "Scrape started for Joker",
            Timestamp = DateTime.UtcNow,
            ScrapeJob = new ScrapeJob { Id = jobId },
        });

        List<ScrapeJobLog> logs = await scrapeJobService.GetLogsForJobAsync(jobId);

        Assert.NotEmpty(logs);
    }

    [Fact]
    public async Task GetAllLogs_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync("api/scrape-jobs/logs");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task GetDashboardStats_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync("api/scrape-jobs/dashboard-stats");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetDashboardStats_SeededDatabase_TotalMoviesAndReelsArePositive()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ScrapeJobProxyService scrapeJobService = new(testContext.ApiClient);

        DashboardStats stats = await scrapeJobService.GetDashboardStatsAsync();

        Assert.True(stats.TotalMovies > 0);
        Assert.True(stats.TotalReels > 0);
    }

    [Fact]
    public async Task SearchMovies_KnownPartialTitle_ReturnsMatchingMovies()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ScrapeJobProxyService scrapeJobService = new(testContext.ApiClient);

        List<Movie> movies = await scrapeJobService.SearchMoviesByNameAsync("Inception");

        Assert.NotEmpty(movies);
        Assert.All(movies, movie => Assert.Contains("Inception", movie.Title, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task FindMovieByTitle_ExactTitle_ReturnsSeededMovieId()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ScrapeJobProxyService scrapeJobService = new(testContext.ApiClient);

        int? movieId = await scrapeJobService.FindMovieByTitleAsync("Inception");

        Assert.Equal(ProxyRepoSeedIds.SeededMovieId, movieId);
    }

    [Fact]
    public async Task ReelExistsByVideoUrl_UnknownUrl_ReturnsFalse()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ScrapeJobProxyService scrapeJobService = new(testContext.ApiClient);

        bool exists = await scrapeJobService.ReelExistsByVideoUrlAsync($"https://example.com/nonexistent-{Guid.NewGuid():N}.mp4");

        Assert.False(exists);
    }

    [Fact]
    public void YouTubeApiKey_InConfiguration_IsPresent()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        IConfiguration config = testContext.Factory.Services.GetRequiredService<IConfiguration>();

        string? apiKey = config["YouTube:ApiKey"];

        Assert.False(string.IsNullOrWhiteSpace(apiKey));
    }
}

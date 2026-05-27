using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MovieApp.DataLayer.Models;
using MovieApp.Tests.Integration.ProxyRepos;

namespace MovieApp.Tests.Integration.Endpoints;

public sealed class ReelsUploadEndpointsIntegrationTests
    : IClassFixture<MovieAppWebApplicationFactory>
{
    private const int MaxUploadResponseMilliseconds = 2000;
    private const int FeedPollingAttempts = 10;
    private const int FeedPollingDelayMilliseconds = 200;

    private readonly HttpClient _client;

    public ReelsUploadEndpointsIntegrationTests(MovieAppWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test", "integration-test");
    }

    [Fact]
    public async Task UploadVideo_ValidRequest_ReturnsOk_AndEventuallyAppearsInUserReels()
    {
        string uniqueTitle = $"Integration Test Reel {Guid.NewGuid():N}";
        string tempVideoPath = await CreateTempMp4Async();

        try
        {
            var request = new
            {
                LocalFilePath = tempVideoPath,
                Title = uniqueTitle,
                Caption = "Uploaded from integration test",
                UploaderUserId = ProxyRepoSeedIds.SeededUserId,
                MovieId = ProxyRepoSeedIds.SeededMovieId
            };

            HttpResponseMessage uploadResponse =
                await _client.PostAsJsonAsync("/api/reels/upload", request);

            Assert.Equal(HttpStatusCode.OK, uploadResponse.StatusCode);

            bool reelAppeared = await WaitUntilReelAppearsAsync(uniqueTitle);

            Assert.True(
                reelAppeared,
                "Uploaded reel did not appear in the user's reels feed within the polling window.");
        }
        finally
        {
            DeleteTempFileIfExists(tempVideoPath);
        }
    }

    [Fact]
    public async Task UploadVideo_ValidRequest_ResponseIsNotBlockedForLongRunningPipeline()
    {
        string tempVideoPath = await CreateTempMp4Async();

        try
        {
            var request = new
            {
                LocalFilePath = tempVideoPath,
                Title = $"Non Blocking Upload Test {Guid.NewGuid():N}",
                Caption = "Checks that HTTP response is returned quickly",
                UploaderUserId = ProxyRepoSeedIds.SeededUserId,
                MovieId = ProxyRepoSeedIds.SeededMovieId
            };

            Stopwatch stopwatch = Stopwatch.StartNew();

            HttpResponseMessage response =
                await _client.PostAsJsonAsync("/api/reels/upload", request);

            stopwatch.Stop();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.True(
                stopwatch.ElapsedMilliseconds < MaxUploadResponseMilliseconds,
                $"Upload endpoint blocked for {stopwatch.ElapsedMilliseconds}ms.");
        }
        finally
        {
            DeleteTempFileIfExists(tempVideoPath);
        }
    }

    private async Task<bool> WaitUntilReelAppearsAsync(string expectedTitle)
    {
        for (int attempt = 0; attempt < FeedPollingAttempts; attempt++)
        {
            List<Reel>? reels =
                await _client.GetFromJsonAsync<List<Reel>>(
                    $"/api/reels/users/{ProxyRepoSeedIds.SeededUserId}");

            if (reels != null && reels.Any(reel => reel.Title == expectedTitle))
            {
                return true;
            }

            await Task.Delay(FeedPollingDelayMilliseconds);
        }

        return false;
    }

    private static async Task<string> CreateTempMp4Async()
    {
        string tempVideoPath = Path.Combine(
            Path.GetTempPath(),
            $"{Guid.NewGuid():N}.mp4");

        await File.WriteAllBytesAsync(
            tempVideoPath,
            new byte[] { 0, 0, 0, 24, 102, 116, 121, 112 });

        return tempVideoPath;
    }

    private static void DeleteTempFileIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}

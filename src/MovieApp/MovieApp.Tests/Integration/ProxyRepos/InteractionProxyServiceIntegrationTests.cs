using System.Net.Http.Json;
using System.Text.Json;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy.Services;
using MovieApp.Proxy;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class ReelInteractionProxyServiceIntegrationTests
{
    [Fact]
    public async Task InsertInteractionAsync_ExistingUserAndReel_CreatesInteractionRecord()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ReelInteractionProxyService interactionRepository = new(testContext.ApiClient);

        // Create a reel with the correct flat DTO that the endpoint expects.
        // ScrapeJobProxyService.InsertScrapedReelAsync is intentionally not used here
        // because it sends a raw Reel object instead of InsertReelRequestBody (known proxy bug).
        HttpResponseMessage reelResponse = await testContext.HttpClient.PostAsJsonAsync(
            "api/video-storage/insert",
            new
            {
                VideoUrl = $"https://example.com/interactions/{Guid.NewGuid():N}.mp4",
                ThumbnailUrl = "https://example.com/interactions/thumbnail.png",
                Title = "Interaction Reel",
                Caption = "Proxy repository integration test",
                FeatureDurationSeconds = 11m,
                CropDataJson = "{}",
                Source = "unit-test",
                Genre = "Drama",
                CreatedAt = DateTime.UtcNow,
                MovieId = ProxyRepoSeedIds.SeededMovieId,
                CreatorUserId = ProxyRepoSeedIds.SeededUserId,
            });
        reelResponse.EnsureSuccessStatusCode();

        string reelJson = await reelResponse.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(reelJson);
        int createdReelId = doc.RootElement.GetProperty("id").GetInt32();

        await interactionRepository.InsertInteractionAsync(new UserReelInteraction
        {
            IsLiked = true,
            WatchDurationSeconds = 12.5m,
            WatchPercentage = 80m,
            ViewedAt = DateTime.UtcNow,
            User = new User { Id = ProxyRepoSeedIds.SeededUserId },
            Reel = new Reel { Id = createdReelId },
        });

        int likeCount = await interactionRepository.GetLikeCountAsync(createdReelId);

        Assert.Equal(1, likeCount);
    }
}

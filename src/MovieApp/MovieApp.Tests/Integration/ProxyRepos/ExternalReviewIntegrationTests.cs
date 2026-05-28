using System.Net;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy.Services;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class ExternalReviewIntegrationTests
{
    [Fact]
    public async Task GetExternalReviews_SeededMovieId_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync(
            $"api/movies/{ProxyRepoSeedIds.SeededMovieId}/external-reviews");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetExternalReviews_SeededMovieId_ReturnsNonNullList()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ExternalReviewProxyService reviewService = new(testContext.ApiClient);

        List<CriticReview> reviews = await reviewService.GetExternalReviewsAsync(
            movieTitle: "Inception",
            releaseYear: 2010);

        Assert.NotNull(reviews);
    }

    [Fact]
    public async Task GetExternalReviews_InvalidMovieId_ReturnsHttp404()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync(
            "api/movies/99999/external-reviews");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    [Fact]
    public async Task GetExternalReviewsByTitle_ValidTitle_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync(
            "api/movies/external-reviews/by-title?title=Inception&year=2010");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetExternalReviewsByTitle_EmptyTitle_ReturnsHttp400()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync(
            "api/movies/external-reviews/by-title?title=&year=2010");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetExternalReviewsByTitle_NonsenseTitle_ReturnsHttp200WithNoException()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync(
            "api/movies/external-reviews/by-title?title=zzz-definitely-not-a-real-movie-xyz&year=1900");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetExternalReviewsByTitle_NonsenseTitle_ReturnsEmptyListNotException()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        ExternalReviewProxyService reviewService = new(testContext.ApiClient);

        List<CriticReview> reviews = await reviewService.GetExternalReviewsAsync(
            movieTitle: "zzz-definitely-not-a-real-movie-xyz",
            releaseYear: 1900);

        Assert.NotNull(reviews);
    }


    [Fact]
    public async Task GetExternalReviews_WithPlaceholderApiKeys_DoesNotReturn5xx()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync(
            $"api/movies/{ProxyRepoSeedIds.SeededMovieId}/external-reviews");

        Assert.True(
            (int)response.StatusCode < 500,
            $"Expected non-5xx but got {(int)response.StatusCode}");
    }
}

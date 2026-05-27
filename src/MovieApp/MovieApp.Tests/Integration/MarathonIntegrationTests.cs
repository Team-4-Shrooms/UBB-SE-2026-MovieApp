using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieApp.DataLayer;
using MovieApp.DataLayer.Models;

namespace MovieApp.Tests.Integration;

public sealed class MarathonIntegrationTests
    : IClassFixture<MovieAppWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly MovieAppWebApplicationFactory _factory;

    public MarathonIntegrationTests(MovieAppWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Enroll_ExistingMarathon_ReturnsOkStatusCode()
    {
        // Arrange
        int marathonId = 1;

        // Act
        HttpResponseMessage response =
            await _client.PostAsync(
                $"/api/marathons/{marathonId}/start",
                null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProgress_ExistingProgress_ReturnsMarathonProgress()
    {
        MarathonProgress seededProgress = await GetSeededProgressAsync();

        // Act
        HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/marathons/{seededProgress.MarathonId}/progress/{seededProgress.UserId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        MarathonProgress? progress =
            await response.Content.ReadFromJsonAsync<MarathonProgress>();

        Assert.NotNull(progress);
        Assert.Equal(seededProgress.UserId, progress.UserId);
        Assert.Equal(seededProgress.MarathonId, progress.MarathonId);
    }

    private async Task<MarathonProgress> GetSeededProgressAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await context.MarathonProgressions
            .AsNoTracking()
            .FirstAsync();
    }
}

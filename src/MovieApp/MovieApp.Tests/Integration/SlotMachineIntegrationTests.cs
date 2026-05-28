using System.Net;
using System.Net.Http.Json;
using MovieApp.WebApi.DTOs;

namespace MovieApp.Tests.Integration;

public sealed class SlotMachineIntegrationTests
    : IClassFixture<MovieAppWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SlotMachineIntegrationTests(MovieAppWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Spin_AuthenticatedUser_ReturnsOkWithValidSlotResult()
    {
        // Arrange
        int userId = 1;

        // Act
        HttpResponseMessage response =
            await _client.PostAsync(
                $"/api/slot-machine/spin/{userId}",
                null);

        // Assert
        Assert.True(
            response.IsSuccessStatusCode,
            $"Actual status code: {response.StatusCode}");

        SlotMachineResultDto? result =
            await response.Content.ReadFromJsonAsync<SlotMachineResultDto>();

        Assert.NotNull(result);
    }

    [Fact]
    public async Task AvailableSpins_ReturnsOk()
    {
        // Arrange
        int userId = 1;

        // Act
        HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/slot-machine/available-spins/{userId}");

        // Assert
        Assert.True(
            response.IsSuccessStatusCode,
            $"Actual status code: {response.StatusCode}");

        int spins =
            await response.Content.ReadFromJsonAsync<int>();

        Assert.True(spins >= 0);
    }
}

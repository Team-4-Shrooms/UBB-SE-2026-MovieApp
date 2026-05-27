using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace MovieApp.Tests.Integration;

public sealed class TournamentGameIntegrationTests : IClassFixture<MovieAppWebApplicationFactory>
{
    private const int UserId = 1;
    private const int PoolSize = 4;

    private readonly HttpClient _client;

    public TournamentGameIntegrationTests(MovieAppWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        // Needed if TestAuthHandler expects a test auth header.
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test", "integration-test");
    }

    [Fact]
    public async Task TournamentFlow_StartGetVote_BracketStaysActiveAndUpdates()
    {
        await _client.PostAsync($"/api/tournament-game/{UserId}/reset", null);

        HttpResponseMessage startResponse =
            await _client.PostAsync($"/api/tournament-game/{UserId}/start?poolSize={PoolSize}", null);

        Assert.Equal(HttpStatusCode.OK, startResponse.StatusCode);

        HttpResponseMessage getResponse =
            await _client.GetAsync($"/api/tournament-game/{UserId}/current-match");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        string firstMatchJson = await getResponse.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(firstMatchJson));

        using JsonDocument firstMatchDocument = JsonDocument.Parse(firstMatchJson);

        int winnerMovieId = firstMatchDocument
            .RootElement
            .GetProperty("firstMovie")
            .GetProperty("id")
            .GetInt32();

        HttpResponseMessage voteResponse =
            await _client.PostAsJsonAsync($"/api/tournament-game/{UserId}/advance", winnerMovieId);

        Assert.Equal(HttpStatusCode.OK, voteResponse.StatusCode);

        HttpResponseMessage afterVoteMatchResponse =
            await _client.GetAsync($"/api/tournament-game/{UserId}/current-match");

        HttpResponseMessage completeResponse =
            await _client.GetAsync($"/api/tournament-game/{UserId}/is-complete");

        Assert.Equal(HttpStatusCode.OK, completeResponse.StatusCode);

        bool isComplete = await completeResponse.Content.ReadFromJsonAsync<bool>();

        Assert.True(
            afterVoteMatchResponse.StatusCode == HttpStatusCode.OK ||
            afterVoteMatchResponse.StatusCode == HttpStatusCode.NoContent);

        Assert.True(
            afterVoteMatchResponse.StatusCode == HttpStatusCode.OK || isComplete);

        if (afterVoteMatchResponse.StatusCode == HttpStatusCode.OK)
        {
            string secondMatchJson = await afterVoteMatchResponse.Content.ReadAsStringAsync();

            Assert.False(string.IsNullOrWhiteSpace(secondMatchJson));
            Assert.NotEqual(firstMatchJson, secondMatchJson);
        }
    }
}

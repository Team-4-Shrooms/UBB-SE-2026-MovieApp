using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MovieApp.Logic.Features.PersonalityMatch;

namespace MovieApp.Tests.Integration;

public sealed class PersonalityMatchIntegrationTests
    : IClassFixture<MovieAppWebApplicationFactory>
{
    private const int UserId = 1;
    private const int MatchCount = 5;

    private readonly HttpClient _client;

    public PersonalityMatchIntegrationTests(MovieAppWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test", "integration-test");
    }

    [Fact]
    public async Task GetTopMatches_ForExistingUser_ReturnsRecommendationsList()
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"/api/personality-match/users/{UserId}/top-matches?count={MatchCount}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        List<MatchResult>? recommendations =
            await response.Content.ReadFromJsonAsync<List<MatchResult>>();

        Assert.NotNull(recommendations);
        Assert.NotEmpty(recommendations!);

        Assert.All(recommendations!, recommendation =>
        {
            Assert.True(recommendation.MatchedUserId > 0);
            Assert.False(string.IsNullOrWhiteSpace(recommendation.MatchedUsername));
            Assert.True(recommendation.MatchScore >= 0);
            Assert.False(string.IsNullOrWhiteSpace(recommendation.FacebookAccount));
        });
    }

    [Fact]
    public async Task GetTopMatches_ResponseDeserializesCorrectly_AfterUserModelMerge()
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"/api/personality-match/users/{UserId}/top-matches?count={MatchCount}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        List<MatchResult>? recommendations =
            await response.Content.ReadFromJsonAsync<List<MatchResult>>();

        Assert.NotNull(recommendations);
        Assert.NotEmpty(recommendations!);

        MatchResult firstRecommendation = recommendations![0];

        Assert.True(firstRecommendation.MatchedUserId > 0);
        Assert.False(string.IsNullOrWhiteSpace(firstRecommendation.MatchedUsername));
        Assert.True(firstRecommendation.MatchScore >= 0);
        Assert.False(string.IsNullOrWhiteSpace(firstRecommendation.FacebookAccount));
    }

    [Fact]
    public async Task GetTopPreferences_ForExistingUser_ReturnsQuizOptionsSource()
    {
        HttpResponseMessage response = await _client.GetAsync(
            $"/api/personality-match/users/{UserId}/top-preferences?count=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string body = await response.Content.ReadAsStringAsync();

        Assert.False(string.IsNullOrWhiteSpace(body));
        Assert.NotEqual("[]", body);
    }
}

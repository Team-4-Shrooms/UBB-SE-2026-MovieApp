using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MovieApp.DataLayer.Models;

namespace MovieApp.Tests.Integration.Endpoints;

public sealed class TriviaIntegrationTests
    : IClassFixture<MovieAppWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TriviaIntegrationTests(MovieAppWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Test",
                "integration-test");
    }

    [Fact]
    public async Task GetQuestion_ReturnsQuestionWithNonNullText()
    {
        HttpResponseMessage response =
            await _client.GetAsync("/api/trivia/question");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        TriviaQuestion? question =
            await response.Content.ReadFromJsonAsync<TriviaQuestion>();

        Assert.NotNull(question);

        Assert.False(
            string.IsNullOrWhiteSpace(
                question!.QuestionText));
    }

    [Fact]
    public async Task SubmitAnswer_WithCorrectAnswer_ReturnsCorrectTrue()
    {
        TriviaQuestion? question =
            await _client.GetFromJsonAsync<TriviaQuestion>(
                "/api/trivia/question");

        Assert.NotNull(question);

        var request = new
        {
            QuestionId = question!.Id,
            SelectedOption = question.CorrectOption
        };

        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/trivia/answer",
                request);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        TriviaAnswerResponse? result =
            await response.Content
                .ReadFromJsonAsync<TriviaAnswerResponse>();

        Assert.NotNull(result);

        Assert.True(result!.Correct);

        Assert.NotNull(result.RewardId);
    }

    [Fact]
    public async Task SubmitAnswer_WithIncorrectAnswer_ReturnsCorrectFalse()
    {
        TriviaQuestion? question =
            await _client.GetFromJsonAsync<TriviaQuestion>(
                "/api/trivia/question");

        Assert.NotNull(question);

        char incorrectOption =
            question!.CorrectOption == 'A'
                ? 'B'
                : 'A';

        var request = new
        {
            QuestionId = question.Id,
            SelectedOption = incorrectOption
        };

        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/trivia/answer",
                request);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        TriviaAnswerResponse? result =
            await response.Content
                .ReadFromJsonAsync<TriviaAnswerResponse>();

        Assert.NotNull(result);

        Assert.False(result!.Correct);

        Assert.Null(result.RewardId);
    }

    private sealed class TriviaAnswerResponse
    {
        public bool Correct { get; set; }

        public int? RewardId { get; set; }
    }
}

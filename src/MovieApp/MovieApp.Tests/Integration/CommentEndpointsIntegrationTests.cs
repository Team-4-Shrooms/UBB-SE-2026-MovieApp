using System.Net;
using System.Net.Http.Json;
using MovieApp.WebApi.DTOs;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.Tests.Integration;

public sealed class CommentEndpointsIntegrationTests
    : IClassFixture<MovieAppWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public CommentEndpointsIntegrationTests(MovieAppWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task AddComment_ExistingMovie_ReturnsOkStatusCode()
    {
        int movieId = 1;

        var request = new AddCommentRequest
        {
            UserId = 1,
            Content = $"Integration comment {Guid.NewGuid():N}",
        };

        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(
                $"/api/movies/{movieId}/comments",
                request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        CommentResponseDto? comment =
            await response.Content.ReadFromJsonAsync<CommentResponseDto>();

        Assert.NotNull(comment);
        Assert.Equal(request.Content, comment.Content);
        Assert.Equal(movieId, comment.MovieId);
    }

    [Fact]
    public async Task ReplyComment_ExistingComment_ReturnsCommentWithParentCommentId()
    {
        int movieId = 1;
        var parentRequest = new AddCommentRequest
        {
            UserId = 1,
            Content = $"Parent comment {Guid.NewGuid():N}",
        };

        HttpResponseMessage parentResponse = await _httpClient.PostAsJsonAsync(
            $"/api/movies/{movieId}/comments",
            parentRequest);
        parentResponse.EnsureSuccessStatusCode();

        CommentResponseDto? parentComment =
            await parentResponse.Content.ReadFromJsonAsync<CommentResponseDto>();

        Assert.NotNull(parentComment);

        var replyRequest = new ReplyCommentRequest
        {
            UserId = 1,
            Content = $"Reply comment {Guid.NewGuid():N}",
        };

        HttpResponseMessage replyResponse = await _httpClient.PostAsJsonAsync(
            $"/api/comments/{parentComment.CommentId}/reply",
            replyRequest);
        replyResponse.EnsureSuccessStatusCode();

        CommentResponseDto? reply =
            await replyResponse.Content.ReadFromJsonAsync<CommentResponseDto>();

        Assert.NotNull(reply);
        Assert.Equal(parentComment.CommentId, reply.ParentCommentId);
    }
}

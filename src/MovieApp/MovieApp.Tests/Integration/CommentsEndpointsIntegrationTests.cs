using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Proxy;
using MovieApp.Proxy.Services;
using MovieApp.WebApi.Endpoints;
using MovieApp.WebApi.DTOs;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using Xunit;

namespace MovieApp.Tests.Integration
{
    public sealed class CommentsEndpointsIntegrationTests : IClassFixture<MovieAppWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly MovieAppWebApplicationFactory _factory;

        public CommentsEndpointsIntegrationTests(MovieAppWebApplicationFactory factory)
        {
            _factory = factory;
            _httpClient = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<ICommentService, MockCommentService>();
                });
            }).CreateClient();
        }

        [Fact]
        public async Task GetComments_ReturnsThreadedListCorrectly()
        {
            List<CommentResponseDto>? response = await _httpClient.GetFromJsonAsync<List<CommentResponseDto>>("/api/movies/1/comments");

            Assert.NotNull(response);
            Assert.NotEmpty(response);

            CommentResponseDto? rootComment = response.FirstOrDefault(c => c.CommentId == 1);
            Assert.NotNull(rootComment);
            Assert.Equal("Root Comment 1", rootComment.Content);
            Assert.NotEmpty(rootComment.Replies);
            Assert.Equal(2, rootComment.Replies[0].CommentId);
            Assert.Equal("Reply to Comment 1", rootComment.Replies[0].Content);
        }

        [Fact]
        public async Task AddComment_CreatesRootCommentCorrectly()
        {
            AddCommentRequest request = new AddCommentRequest { UserId = 3, Content = "A new root comment" };
            HttpResponseMessage? response = await _httpClient.PostAsJsonAsync("/api/movies/1/comments", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            CommentResponseDto? created = await response.Content.ReadFromJsonAsync<CommentResponseDto>();
            Assert.NotNull(created);
            Assert.Equal("A new root comment", created.Content);
            Assert.Null(created.ParentCommentId);
        }

        [Fact]
        public async Task ReplyComment_CreatesChildCommentCorrectly()
        {
            ReplyCommentRequest request = new ReplyCommentRequest { UserId = 4, Content = "A reply to root" };
            HttpResponseMessage? response = await _httpClient.PostAsJsonAsync("/api/comments/1/reply", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            CommentResponseDto? created = await response.Content.ReadFromJsonAsync<CommentResponseDto>();
            Assert.NotNull(created);
            Assert.Equal("A reply to root", created.Content);
            Assert.Equal(1, created.ParentCommentId);
        }

        [Fact]
        public async Task CommentProxyService_GetsAndPostsCorrectly()
        {
            MockTokenProvider mockTokenProvider = new MockTokenProvider();
            ApiClient apiClient = new ApiClient(_httpClient, mockTokenProvider);
            CommentProxyService proxyService = new CommentProxyService(apiClient);

            List<Comment> comments = await proxyService.GetCommentsForMovieAsync(1);
            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Comment? root = comments.FirstOrDefault(c => c.CommentId == 1);
            Assert.NotNull(root);
            Assert.NotEmpty(root.Replies);

            Comment? newRoot = await proxyService.AddCommentAsync(5, 1, "Proxy root comment");
            Assert.NotNull(newRoot);
            Assert.Equal("Proxy root comment", newRoot.Content);
            
            Comment? newReply = await proxyService.AddReplyAsync(6, 1, "Proxy reply");
            Assert.NotNull(newReply);
            Assert.Equal("Proxy reply", newReply.Content);
            Assert.Equal(1, newReply.ParentCommentId);
        }

        private class MockTokenProvider : IAuthTokenProvider
        {
            public string GetToken() => "mock-token";
            public Task RefreshAsync() => Task.CompletedTask;
        }

        private class MockCommentService : ICommentService
        {
            private static readonly List<Comment> _comments = new()
            {
                new Comment
                {
                    CommentId = 1,
                    AuthorId = 1,
                    MovieId = 1,
                    Content = "Root Comment 1",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                    Author = new User { Id = 1, Username = "Alice" }
                },
                new Comment
                {
                    CommentId = 2,
                    AuthorId = 2,
                    MovieId = 1,
                    ParentCommentId = 1,
                    Content = "Reply to Comment 1",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                    Author = new User { Id = 2, Username = "Bob" }
                }
            };

            public Task<List<Comment>> GetCommentsForMovieAsync(int movieId, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(_comments.Where(c => c.MovieId == movieId).ToList());
            }

            public Task<Comment> AddCommentAsync(int userId, int movieId, string content, CancellationToken cancellationToken = default)
            {
                Comment? comment = new Comment
                {
                    CommentId = _comments.Count + 1,
                    AuthorId = userId,
                    MovieId = movieId,
                    Content = content,
                    CreatedAt = DateTime.UtcNow,
                    Author = new User { Id = userId, Username = $"User_{userId}" }
                };
                _comments.Add(comment);
                return Task.FromResult(comment);
            }

            public Task<Comment> AddReplyAsync(int userId, int parentCommentId, string content, CancellationToken cancellationToken = default)
            {
                Comment? parent = _comments.FirstOrDefault(c => c.CommentId == parentCommentId);
                Comment reply = new Comment
                {
                    CommentId = _comments.Count + 1,
                    AuthorId = userId,
                    MovieId = parent?.MovieId ?? 1,
                    ParentCommentId = parentCommentId,
                    Content = content,
                    CreatedAt = DateTime.UtcNow,
                    Author = new User { Id = userId, Username = $"User_{userId}" }
                };
                _comments.Add(reply);
                return Task.FromResult(reply);
            }

            public Task DeleteCommentAsync(int commentId, CancellationToken cancellationToken = default)
            {
                _comments.RemoveAll(c => c.CommentId == commentId);
                return Task.CompletedTask;
            }
        }
    }
}

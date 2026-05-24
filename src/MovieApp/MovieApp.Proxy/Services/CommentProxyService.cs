using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebApi.DTOs;

namespace MovieApp.Proxy.Services
{
    public class CommentProxyService : ICommentService
    {
        private readonly ApiClient _apiClient;

        public CommentProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<Comment>> GetCommentsForMovieAsync(int movieId, CancellationToken cancellationToken = default)
        {
            List<CommentReferenceDto>? result = await _apiClient.GetAsync<List<CommentReferenceDto>>($"api/movies/{movieId}/comments", cancellationToken);
            return result?.Select(MapToEntity).ToList() ?? new List<Comment>();
        }

        public async Task<Comment> AddCommentAsync(int userId, int movieId, string content, CancellationToken cancellationToken = default)
        {
            CommentReferenceDto? result = await _apiClient.PostAsync<object, CommentReferenceDto>(
                $"api/movies/{movieId}/comments",
                new { UserId = userId, Content = content },
                cancellationToken);
            return result != null ? MapToEntity(result) : null!;
        }

        public async Task<Comment> AddReplyAsync(int userId, int parentCommentId, string content, CancellationToken cancellationToken = default)
        {
            CommentReferenceDto? result = await _apiClient.PostAsync<object, CommentReferenceDto>(
                $"api/comments/{parentCommentId}/reply",
                new { UserId = userId, Content = content },
                cancellationToken);
            return result != null ? MapToEntity(result) : null!;
        }

        public async Task DeleteCommentAsync(int commentId, CancellationToken cancellationToken = default)
        {
            await _apiClient.DeleteAsync($"api/comments/{commentId}", cancellationToken);
        }

        private static Comment MapToEntity(CommentReferenceDto dto)
        {
            return new Comment
            {
                CommentId = dto.CommentId,
                AuthorId = dto.AuthorId,
                MovieId = dto.MovieId,
                ParentCommentId = dto.ParentCommentId,
                Content = dto.Content,
                CreatedAt = dto.CreatedAt,
                Author = new User { Id = dto.AuthorId, Username = dto.AuthorUsername },
                Replies = dto.Replies?.Select(MapToEntity).ToList() ?? new List<Comment>()
            };
        }
    }
}

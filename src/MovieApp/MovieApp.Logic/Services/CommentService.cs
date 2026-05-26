using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepo;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepo = commentRepository;
        }

        /// <inheritdoc />
        public async Task<List<Comment>> GetCommentsForMovieAsync(int movieId, CancellationToken cancellationToken = default)
        {
            var allComments = await _commentRepo.GetAllAsync(cancellationToken);

            // Filter to the requested movie and return only top-level comments.
            // Replies are already eagerly loaded via Include(), so the nested
            // thread structure is preserved without additional queries.
            return allComments
                .Where(comment => comment.MovieId == movieId && comment.ParentCommentId == null)
                .OrderByDescending(comment => comment.CreatedAt)
                .ToList();
        }

        /// <inheritdoc />
        public async Task<Comment> AddCommentAsync(int userId, int movieId, string content, CancellationToken cancellationToken = default)
        {
            var comment = new Comment
            {
                AuthorId = userId,
                MovieId = movieId,
                Content = content,
                CreatedAt = DateTime.UtcNow,
            };

            await _commentRepo.InsertAsync(comment, cancellationToken);
            return comment;
        }

        /// <inheritdoc />
        public async Task<Comment> AddReplyAsync(int userId, int parentCommentId, string content, CancellationToken cancellationToken = default)
        {
            var parentComment = await _commentRepo.GetByIdAsync(parentCommentId, cancellationToken);

            if (parentComment == null)
            {
                throw new KeyNotFoundException($"Parent comment with id {parentCommentId} was not found.");
            }

            // The reply inherits the MovieId from the parent comment,
            // ensuring it always belongs to the same movie.
            var reply = new Comment
            {
                AuthorId = userId,
                MovieId = parentComment.MovieId,
                ParentCommentId = parentCommentId,
                Content = content,
                CreatedAt = DateTime.UtcNow,
            };

            await _commentRepo.InsertAsync(reply, cancellationToken);
            return reply;
        }

        /// <inheritdoc />
        public async Task DeleteCommentAsync(int commentId, CancellationToken cancellationToken = default)
        {
            var deleted = await _commentRepo.DeleteAsync(commentId, cancellationToken);

            if (!deleted)
            {
                throw new KeyNotFoundException($"Comment with id {commentId} was not found.");
            }
        }
    }
}

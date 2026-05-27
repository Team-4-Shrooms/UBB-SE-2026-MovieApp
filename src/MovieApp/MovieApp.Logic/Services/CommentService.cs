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

            var movieComments = allComments.Where(c => c.MovieId == movieId).ToList();

            var childrenByParent = movieComments
                .Where(c => c.ParentCommentId != null)
                .GroupBy(c => c.ParentCommentId!.Value)
                .ToDictionary(g => g.Key, g => g.OrderBy(c => c.CreatedAt).ToList());

            void AssignReplies(Comment comment)
            {
                comment.Replies = childrenByParent.TryGetValue(comment.CommentId, out var children)
                    ? (ICollection<Comment>)children
                    : new List<Comment>();

                foreach (var child in comment.Replies)
                    AssignReplies(child);
            }

            var rootComments = movieComments
                .Where(c => c.ParentCommentId == null)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            foreach (var root in rootComments)
                AssignReplies(root);

            return rootComments;
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

using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class CommentRepository : ICommentRepository
    {
        private readonly IMovieAppDbContext _context;

        public CommentRepository(IMovieAppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<List<Comment>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Comments
                .AsNoTracking()
                .Include(comment => comment.Author)
                .Include(comment => comment.Replies)
                    .ThenInclude(reply => reply.Author)
                .OrderByDescending(comment => comment.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Comment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Comments
                .Include(comment => comment.Author)
                .Include(comment => comment.Replies)
                    .ThenInclude(reply => reply.Author)
                .FirstOrDefaultAsync(comment => comment.CommentId == id, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> InsertAsync(Comment comment, CancellationToken cancellationToken = default)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync(cancellationToken);
            return comment.CommentId;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateAsync(Comment comment, CancellationToken cancellationToken = default)
        {
            var existing = await _context.Comments
                .FirstOrDefaultAsync(existingComment => existingComment.CommentId == comment.CommentId, cancellationToken);

            if (existing == null)
            {
                return false;
            }

            existing.Content = comment.Content;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(existingComment => existingComment.CommentId == id, cancellationToken);

            if (comment == null)
            {
                return false;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

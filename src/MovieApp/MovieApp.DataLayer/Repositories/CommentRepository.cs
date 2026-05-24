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
        public async Task<List<Comment>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Comments
                .AsNoTracking()
                .Include(c => c.Author)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.Author)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(ct);
        }

        /// <inheritdoc />
        public async Task<Comment?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.Author)
                .FirstOrDefaultAsync(c => c.CommentId == id, ct);
        }

        /// <inheritdoc />
        public async Task<int> InsertAsync(Comment comment, CancellationToken ct = default)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync(ct);
            return comment.CommentId;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateAsync(Comment comment, CancellationToken ct = default)
        {
            var existing = await _context.Comments
                .FirstOrDefaultAsync(c => c.CommentId == comment.CommentId, ct);

            if (existing == null)
            {
                return false;
            }

            existing.Content = comment.Content;
            await _context.SaveChangesAsync(ct);
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.CommentId == id, ct);

            if (comment == null)
            {
                return false;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}

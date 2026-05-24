using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class MovieRepository : IMovieRepository
    {
        private readonly MovieApp.DataLayer.Interfaces.IMovieAppDbContext _context;
        public MovieRepository(MovieApp.DataLayer.Interfaces.IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task<Movie?> GetMovieByIdAsync(int movieId)
        {
            return await _context.Movies.FindAsync(movieId);
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            return await _context.Movies.ToListAsync();
        }

        public async Task<bool> UserOwnsMovieAsync(int userId, int movieId)
        {
            return await _context.OwnedMovies.AnyAsync(om => om.User.Id == userId && om.Movie.Id == movieId);
        }

        public async Task AddOwnedMovieAsync(OwnedMovie ownership)
        {
            await _context.OwnedMovies.AddAsync(ownership);
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Movie>> SearchMoviesAsync(string pattern, int limit)
        {
            return await _context.Movies.Where(m => m.Title.Contains(pattern)).Take(limit).ToListAsync();
        }

        public async Task<IReadOnlyList<Genre>> GetGenresAsync(CancellationToken ct = default)
        => await _context.Genres.ToListAsync(ct);

        public async Task<IReadOnlyList<Actor>> GetActorsAsync(CancellationToken ct = default)
            => await _context.Actors.ToListAsync(ct);

        public async Task<IReadOnlyList<Director>> GetDirectorsAsync(CancellationToken ct = default)
            => await _context.Directors.ToListAsync(ct);

        public async Task<IReadOnlyList<Movie>> FindMoviesByCriteriaAsync(int genreId, int actorId, int directorId, CancellationToken ct = default)
        {
            return await _context.Movies
                .Where(m => m.Genres.Any(g => g.Id == genreId) &&
                            m.Actors.Any(a => a.Id == actorId) &&
                            m.Directors.Any(d => d.Id == directorId))
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Movie>> FindMoviesByAnyCriteriaAsync(int genreId, int actorId, int directorId, CancellationToken ct = default)
        {
            return await _context.Movies
                .Where(m => m.Genres.Any(g => g.Id == genreId) ||
                            m.Actors.Any(a => a.Id == actorId) ||
                            m.Directors.Any(d => d.Id == directorId))
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<int>> FindScreeningEventIdsForMovieAsync(int movieId, CancellationToken ct = default)
        {
            return await _context.Screenings
                .Where(s => s.MovieId == movieId)
                .Select(s => s.EventId)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<ReelCombination>> GetValidReelCombinationsAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            // Get movies that have at least one screening in the future
            var moviesWithFutureScreenings = await _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.Directors)
                .Where(m => _context.Screenings.Any(s => s.MovieId == m.Id && s.ScreeningTime >= now))
                .ToListAsync(ct);

            var combinations = new List<ReelCombination>();

            foreach (var movie in moviesWithFutureScreenings)
            {
                foreach (var genre in movie.Genres)
                {
                    foreach (var actor in movie.Actors)
                    {
                        foreach (var director in movie.Directors)
                        {
                            combinations.Add(new ReelCombination
                            {
                                Genre = genre,
                                Actor = actor,
                                Director = director
                            });
                        }
                    }
                }
            }

            // Filter to unique combinations based on the IDs of the entities
            return combinations
                .DistinctBy(c => new
                {
                    GenreId = c.Genre.Id,
                    ActorId = c.Actor.Id,
                    DirectorId = c.Director.Id
                })
                .ToList();
        }
    }
}


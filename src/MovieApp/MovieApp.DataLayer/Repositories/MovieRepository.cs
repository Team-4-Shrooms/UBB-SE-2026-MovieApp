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
            return await _context.OwnedMovies.AnyAsync(ownedMovie => ownedMovie.User.Id == userId && ownedMovie.Movie.Id == movieId);
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
            return await _context.Movies.Where(movie => movie.Title.Contains(pattern)).Take(limit).ToListAsync();
        }

        public async Task<IReadOnlyList<Genre>> GetGenresAsync(CancellationToken cancellationToken = default)
        => await _context.Genres.ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Actor>> GetActorsAsync(CancellationToken cancellationToken = default)
            => await _context.Actors.ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Director>> GetDirectorsAsync(CancellationToken cancellationToken = default)
            => await _context.Directors.ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Movie>> FindMoviesByCriteriaAsync(int genreId, int actorId, int directorId, CancellationToken cancellationToken = default)
        {
            return await _context.Movies
                .Where(movie => movie.Genres.Any(genre => genre.Id == genreId) &&
                            movie.Actors.Any(actor => actor.Id == actorId) &&
                            movie.Directors.Any(director => director.Id == directorId))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Movie>> FindMoviesByAnyCriteriaAsync(int genreId, int actorId, int directorId, CancellationToken cancellationToken = default)
        {
            return await _context.Movies
                .Where(movie => movie.Genres.Any(genre => genre.Id == genreId) ||
                            movie.Actors.Any(actor => actor.Id == actorId) ||
                            movie.Directors.Any(director => director.Id == directorId))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<int>> FindScreeningEventIdsForMovieAsync(int movieId, CancellationToken cancellationToken = default)
        {
            return await _context.Screenings
                .Where(screening => screening.MovieId == movieId)
                .Select(screening => screening.EventId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ReelCombination>> GetValidReelCombinationsAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            var moviesWithFutureScreenings = await _context.Movies
                .Include(movie => movie.Genres)
                .Include(movie => movie.Actors)
                .Include(movie => movie.Directors)
                .Where(movie => _context.Screenings.Any(screening => screening.MovieId == movie.Id && screening.ScreeningTime >= now))
                .ToListAsync(cancellationToken);

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

            return combinations
                .DistinctBy(combination => new
                {
                    GenreId = combination.Genre.Id,
                    ActorId = combination.Actor.Id,
                    DirectorId = combination.Director.Id
                })
                .ToList();
        }
    }
}


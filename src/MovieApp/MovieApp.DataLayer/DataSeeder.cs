using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace MovieApp.DataLayer
{
    /// <summary>
    /// Provides a single consolidated seed method for the application database.
    /// Replaces the old raw-SQL DatabaseInitializer from the PureCaffeine project.
    /// Call <see cref="SeedAsync"/> once at application startup after running migrations.
    /// </summary>
    public class DataSeeder
    {
        private readonly Interfaces.IMovieAppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSeeder"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public DataSeeder(Interfaces.IMovieAppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Seeds all required data into the database if it does not already exist.
        /// Safe to call multiple times — all operations are idempotent.
        /// </summary>
        public async Task SeedAsync()
        {
            await SeedAdminUserAsync();
            await SeedUsersAsync();
            await SeedMoviesAsync();
            await SeedMusicTracksAsync();
            await SeedReelsAsync();
            await SeedUserMoviePreferencesAsync();
            await SeedUserProfilesAsync();
            await SeedSellersAsync();
            await SeedActiveSalesAsync();
            await SeedMovieEventsAsync();
            await SeedMovieReviewsAsync();
            await SeedEquipmentAsync();
            await SeedScreeningsAndRoomsAsync();
            await SeedAdditionalRoomsAsync();
            await FixExistingDataAsync();
            await SeedTriviaQuestionsAsync();
            await SeedMarathonsAsync();
            await SeedMarathonProgressionsAsync();
        }

        /// <summary>
        /// Ensures the built-in admin account exists.
        /// Runs on every startup — safe on existing databases.
        /// The placeholder hash is replaced with a real BCrypt hash by Program.cs on first run.
        /// </summary>
        private async Task SeedAdminUserAsync()
        {
            bool adminExists = await _context.Users.AnyAsync(u => u.Username == "admin");
            if (!adminExists)
            {
                _context.Users.Add(new User
                {
                    Username = "admin",
                    Email = "admin@movieapp.com",
                    PasswordHash = "placeholder_admin",
                    Balance = 50000m,
                });
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedUsersAsync()
        {
            if (await _context.Users.AnyAsync(u => u.Username != "admin"))
            {
                return;
            }

            _context.Users.AddRange(
                new User
                {
                    Username = "User1",
                    Email = "user1@movieapp.com",
                    PasswordHash = "placeholder_hash_1",
                    Balance = 50000m,
                },
                new User
                {
                    Username = "Alice",
                    Email = "alice@movieapp.com",
                    PasswordHash = "placeholder_hash_2",
                    Balance = 50000m,
                },
                new User
                {
                    Username = "Bob",
                    Email = "bob@movieapp.com",
                    PasswordHash = "placeholder_hash_3",
                    Balance = 50000m,
                },
                new User
                {
                    Username = "Carol",
                    Email = "carol@movieapp.com",
                    PasswordHash = "placeholder_hash_4",
                    Balance = 100m,
                },
                new User
                {
                    Username = "Dave",
                    Email = "dave@movieapp.com",
                    PasswordHash = "placeholder_hash_5",
                    Balance = 100m,
                },
                new User
                {
                    Username = "Eve",
                    Email = "eve@movieapp.com",
                    PasswordHash = "placeholder_hash_6",
                    Balance = 100m,
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedMoviesAsync()
        {
            if (await _context.Movies.AnyAsync())
            {
                return;
            }

            _context.Movies.AddRange(
                new Movie
                {
                    Title = "Inception",
                    PosterUrl = "https://media.themoviedb.org/t/p/w600_and_h900_face/vr6ouTojPp0zlSpJvCbODPp19nd.jpg",
                    PrimaryGenre = "Sci-Fi",
                    ReleaseYear = 2010,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Dark Knight",
                    PosterUrl = "https://media.themoviedb.org/t/p/w600_and_h900_face/a1UL3FTJDgQikYIebnMDhTPFVfm.jpg",
                    PrimaryGenre = "Action",
                    ReleaseYear = 2008,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Interstellar",
                    PosterUrl = "https://media.themoviedb.org/t/p/w600_and_h900_face/wbnrYkn59cdFuu0LNAZ2BWh2i37.jpg",
                    PrimaryGenre = "Adventure",
                    ReleaseYear = 2014,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Matrix",
                    PosterUrl = "https://media.themoviedb.org/t/p/w600_and_h900_face/p96dm7sCMn4VYAStA6siNz30G1r.jpg",
                    PrimaryGenre = "Sci-Fi",
                    ReleaseYear = 1999,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Parasite",
                    PosterUrl = "https://media.themoviedb.org/t/p/w600_and_h900_face/7IiTTgloJzvGI1TAYymCfbfl3vT.jpg",
                    PrimaryGenre = "Thriller",
                    ReleaseYear = 2019,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "La La Land",
                    PosterUrl = "https://media.themoviedb.org/t/p/w600_and_h900_face/uDO8zWDhfWwoFdKS4fzkUJt0Rf0.jpg",
                    PrimaryGenre = "Musical",
                    ReleaseYear = 2016,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Whiplash",
                    PosterUrl = "https://media.themoviedb.org/t/p/w600_and_h900_face/7fn624j5lj3xTme2SgiLCeuedmO.jpg",
                    PrimaryGenre = "Drama",
                    ReleaseYear = 2014,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Grand Budapest Hotel",
                    PosterUrl = "https://media.themoviedb.org/t/p/w600_and_h900_face/eWdyYQreja6JGCzqHWXpWHDrrPo.jpg",
                    PrimaryGenre = "Comedy",
                    ReleaseYear = 2014,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Avatar",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/gKY6q7SjCkAU6FqvqWybDYgUKIF.jpg",
                    PrimaryGenre = "Sci-Fi",
                    ReleaseYear = 2009,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Titanic",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/9xjZS2rlVxm8SFx8kPC3aIGCOYQ.jpg",
                    PrimaryGenre = "Romance",
                    ReleaseYear = 1997,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Gladiator",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/yafsp1whNDGqmn6vqHdgg0PbZA5.jpg",
                    PrimaryGenre = "Action",
                    ReleaseYear = 2000,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Shawshank Redemption",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/tsY4m4IH8MZly4kvxZszbommLKj.jpg",
                    PrimaryGenre = "Drama",
                    ReleaseYear = 1994,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Forrest Gump",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/n7NEvy20kMLD0X6lrzoaSGXnr3I.jpg",
                    PrimaryGenre = "Drama",
                    ReleaseYear = 1994,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Godfather",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/z4lzwl3Gff5IOWKGiYY7gUFYXUb.jpg",
                    PrimaryGenre = "Crime",
                    ReleaseYear = 1972,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Pulp Fiction",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/vQWk5YBFWF4bZaofAbv0tShwBvQ.jpg",
                    PrimaryGenre = "Crime",
                    ReleaseYear = 1994,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Fight Club",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/pB8BM7pdSp6B6Ih7QZ4DrQ3PmJK.jpg",
                    PrimaryGenre = "Drama",
                    ReleaseYear = 1999,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Social Network",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/jD2LsdNu9zWwXjjlbxO0Iibpefz.jpg",
                    PrimaryGenre = "Drama",
                    ReleaseYear = 2010,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Joker",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/6zIyfSojJxoCj6mL3TZPFZBByfP.jpg",
                    PrimaryGenre = "Thriller",
                    ReleaseYear = 2019,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Avengers: Endgame",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/5hRf3rT7T5QNTmfbv00yXzpGvXw.jpg",
                    PrimaryGenre = "Action",
                    ReleaseYear = 2019,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Iron Man",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/frW3QzyDondJdnd6ydzT8ekKHAw.jpg",
                    PrimaryGenre = "Action",
                    ReleaseYear = 2008,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Toy Story",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/RyAWIbSmt4xC859hyQ43wUumM9.jpg",
                    PrimaryGenre = "Animation",
                    ReleaseYear = 1995,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Finding Nemo",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/7YrKAe5GBg2f2WnwMrX9IdLFqCq.jpg",
                    PrimaryGenre = "Animation",
                    ReleaseYear = 2003,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Up",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/f6ytrGR8IJ0qizc2gI0HSJN6OaU.jpg",
                    PrimaryGenre = "Animation",
                    ReleaseYear = 2009,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Lion King",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/Pa9euUkkwUqHJoMh6eIj2XVeV4.jpg",
                    PrimaryGenre = "Animation",
                    ReleaseYear = 1994,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Frozen",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/itAKcobTYGpYT8Phwjd8c9hleTo.jpg",
                    PrimaryGenre = "Animation",
                    ReleaseYear = 2013,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Conjuring",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/A09nKeXAa0FlOr6Y6EVnvAINKQ2.jpg",
                    PrimaryGenre = "Horror",
                    ReleaseYear = 2013,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Get Out",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/yY33WCqDYUHdT6LJWsJUekSM4E.jpg",
                    PrimaryGenre = "Horror",
                    ReleaseYear = 2017,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "A Quiet Place",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/yelqLnp4My4WWqD647wwwpw552P.jpg",
                    PrimaryGenre = "Horror",
                    ReleaseYear = 2018,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Mad Max: Fury Road",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/89BIhHMvGAoxQkniNFB8ENrfzxk.jpg",
                    PrimaryGenre = "Action",
                    ReleaseYear = 2015,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "John Wick",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/rP7X52bxOEBc09h5qzHJnHXxE3C.jpg",
                    PrimaryGenre = "Action",
                    ReleaseYear = 2014,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Wolf of Wall Street",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/bm6TEyJMpvzTeBOP1V43UfRrrfg.jpg",
                    PrimaryGenre = "Biography",
                    ReleaseYear = 2013,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Django Unchained",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/w1P4HHXJT6CRbcZ2x6Yq2sjWsdF.jpg",
                    PrimaryGenre = "Western",
                    ReleaseYear = 2012,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Revenant",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/6Z6bzPW2K6i5nD2NO8wRZRKQJ5y.jpg",
                    PrimaryGenre = "Adventure",
                    ReleaseYear = 2015,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Blade Runner 2049",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/9pz5bPDufSrJd8yTNjp0apTAVf8.jpg",
                    PrimaryGenre = "Sci-Fi",
                    ReleaseYear = 2017,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Her",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/kBK4UlVOIx6NZyD0QkHlFi9XnAw.jpg",
                    PrimaryGenre = "Romance",
                    ReleaseYear = 2013,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "The Prestige",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/cNpg2TjWtsut8QUBqezkbHXQFgb.jpg",
                    PrimaryGenre = "Drama",
                    ReleaseYear = 2006,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Memento",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/fKTPH2WvH8nHTXeBYBVhawtRqtR.jpg",
                    PrimaryGenre = "Thriller",
                    ReleaseYear = 2000,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                },
                new Movie
                {
                    Title = "Shutter Island",
                    PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/qfpFopx4AHd3oTOkj0VGG50AS39.jpg",
                    PrimaryGenre = "Thriller",
                    ReleaseYear = 2010,
                    Description = string.Empty,
                    Synopsis = string.Empty,
                    Rating = 0m,
                    Price = 9.99m,
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedMusicTracksAsync()
        {
            if (await _context.MusicTracks.AnyAsync())
            {
                return;
            }

            _context.MusicTracks.AddRange(
                new MusicTrack
                {
                    TrackName = "Epic Cinematic Theme",
                    Author = "Hans Zimmer",
                    AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3",
                    DurationSeconds = 180.5m,
                },
                new MusicTrack
                {
                    TrackName = "Upbeat Pop Track",
                    Author = "Mark Ronson",
                    AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-2.mp3",
                    DurationSeconds = 150.0m,
                },
                new MusicTrack
                {
                    TrackName = "Dramatic Orchestral",
                    Author = "John Williams",
                    AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-3.mp3",
                    DurationSeconds = 200.3m,
                },
                new MusicTrack
                {
                    TrackName = "Chill Lo-Fi Beats",
                    Author = "Nujabes",
                    AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-4.mp3",
                    DurationSeconds = 120.0m,
                },
                new MusicTrack
                {
                    TrackName = "Action Packed Rock",
                    Author = "AC/DC",
                    AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-5.mp3",
                    DurationSeconds = 165.7m,
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedReelsAsync()
        {
            if (await _context.Reels.AnyAsync())
            {
                return;
            }

            User? user1 = await _context.Users.FirstOrDefaultAsync(user => user.Username == "User1");

            if (user1 is null)
            {
                return;
            }

            Movie? inception = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Inception");
            Movie? darkKnight = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "The Dark Knight");
            Movie? interstellar = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Interstellar");
            Movie? matrix = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "The Matrix");
            Movie? parasite = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Parasite");
            Movie? laLaLand = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "La La Land");
            Movie? whiplash = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Whiplash");

            if (inception is null || darkKnight is null || interstellar is null ||
                matrix is null || parasite is null || laLaLand is null || whiplash is null)
            {
                return;
            }

            _context.Reels.AddRange(
                new Reel
                {
                    Movie = inception,
                    CreatorUser = user1,
                    VideoUrl = "https://samplelib.com/lib/preview/mp4/sample-10s.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BMjAxMzY3NjcxNF5BMl5BanBnXkFtZTcwNTI5OTM0Mw@@._V1_.jpg",
                    Title = "Inception - Dream Within a Dream",
                    Caption = "Mind-bending scene from Inception where reality bends",
                    FeatureDurationSeconds = 45.5m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                },
                new Reel
                {
                    Movie = inception,
                    CreatorUser = user1,
                    VideoUrl = "https://samplelib.com/lib/preview/mp4/sample-15s.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BMjAxMzY3NjcxNF5BMl5BanBnXkFtZTcwNTI5OTM0Mw@@._V1_.jpg",
                    Title = "Inception - Rotating Hallway Fight",
                    Caption = "The iconic zero-gravity hallway fight sequence",
                    FeatureDurationSeconds = 60.2m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-9),
                },
                new Reel
                {
                    Movie = darkKnight,
                    CreatorUser = user1,
                    VideoUrl = "https://samplelib.com/lib/preview/mp4/sample-20s.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BMTMxNTMwODM0NF5BMl5BanBnXkFtZTcwODAyMTk2Mw@@._V1_.jpg",
                    Title = "The Dark Knight - Joker Interrogation",
                    Caption = "Heath Ledger's legendary Joker interrogation scene",
                    FeatureDurationSeconds = 55.0m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                },
                new Reel
                {
                    Movie = darkKnight,
                    CreatorUser = user1,
                    VideoUrl = "https://samplelib.com/lib/preview/mp4/sample-30s.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BMTMxNTMwODM0NF5BMl5BanBnXkFtZTcwODAyMTk2Mw@@._V1_.jpg",
                    Title = "The Dark Knight - Batmobile Chase",
                    Caption = "Epic chase scene through Gotham streets",
                    FeatureDurationSeconds = 70.8m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                },
                new Reel
                {
                    Movie = interstellar,
                    CreatorUser = user1,
                    VideoUrl = "https://samplelib.com/lib/preview/mp4/sample-5s.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BZjdkOTU3MDktN2IxOS00OGEyLWFmMjktY2FiMmZkNWIyODZiXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_.jpg",
                    Title = "Interstellar - Docking Scene",
                    Caption = "The intense docking sequence with the spinning station",
                    FeatureDurationSeconds = 90.3m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-6),
                },
                new Reel
                {
                    Movie = interstellar,
                    CreatorUser = user1,
                    VideoUrl = "https://filesamples.com/samples/video/mp4/sample_640x360.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BZjdkOTU3MDktN2IxOS00OGEyLWFmMjktY2FiMmZkNWIyODZiXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_.jpg",
                    Title = "Interstellar - Tesseract Scene",
                    Caption = "Cooper in the 5th dimension tesseract",
                    FeatureDurationSeconds = 80.0m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                },
                new Reel
                {
                    Movie = matrix,
                    CreatorUser = user1,
                    VideoUrl = "https://filesamples.com/samples/video/mp4/sample_960x400_ocean_with_audio.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BNzQzOTk3NTAtNDQ2Ny00Njc2LTk3M2QtN2FjYTJjNzQzYzQwXkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_.jpg",
                    Title = "The Matrix - Bullet Time",
                    Caption = "The revolutionary bullet time effect scene",
                    FeatureDurationSeconds = 35.5m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                },
                new Reel
                {
                    Movie = parasite,
                    CreatorUser = user1,
                    VideoUrl = "https://test-videos.co.uk/vids/bigbuckbunny/mp4/h264/360/Big_Buck_Bunny_360_10s_1MB.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BYWZjMjk3ZTAtZGYzMC00ODQ0LWI2YTMtYjQ5NDU3N2NmZDIzXkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_.jpg",
                    Title = "Parasite - Basement Reveal",
                    Caption = "The shocking basement discovery scene",
                    FeatureDurationSeconds = 50.0m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                },
                new Reel
                {
                    Movie = laLaLand,
                    CreatorUser = user1,
                    VideoUrl = "https://archive.org/download/ElephantsDream/ed_1024_512kb.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BMjA2OTYxNTY2Nl5BMl5BanBnXkFtZTgwNzg4OTA5OTE@._V1_.jpg",
                    Title = "La La Land - Highway Opening",
                    Caption = "The colorful highway opening dance number",
                    FeatureDurationSeconds = 65.2m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                },
                new Reel
                {
                    Movie = whiplash,
                    CreatorUser = user1,
                    VideoUrl = "https://media.w3.org/2010/05/sintel/trailer.mp4",
                    ThumbnailUrl = "https://m.media-amazon.com/images/M/MV5BMjE4NDYxNTAxNV5BMl5BanBnXkFtZTgwNzM0NDM1MjE@._V1_.jpg",
                    Title = "Whiplash - Final Performance",
                    Caption = "The intense final drum performance",
                    FeatureDurationSeconds = 75.0m,
                    Source = "youtube",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedUserMoviePreferencesAsync()
        {
            if (await _context.UserMoviePreferences.AnyAsync())
            {
                return;
            }

            List<User> users = await _context.Users.OrderBy(user => user.Id).ToListAsync();
            List<Movie> movies = await _context.Movies.OrderBy(movie => movie.Id).ToListAsync();

            if (users.Count < 6 || movies.Count < 8)
            {
                return;
            }

            User user1 = users[0];
            User user2 = users[1];
            User user3 = users[2];
            User user4 = users[3];
            User user5 = users[4];
            User user6 = users[5];

            Movie movie1 = movies[0];
            Movie movie2 = movies[1];
            Movie movie3 = movies[2];
            Movie movie4 = movies[3];
            Movie movie5 = movies[4];
            Movie movie6 = movies[5];
            Movie movie7 = movies[6];
            Movie movie8 = movies[7];

            _context.UserMoviePreferences.AddRange(
                // User 1 — seed for tournament
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie1,
                    Score = 8.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie2,
                    Score = 9.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie3,
                    Score = 7.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie4,
                    Score = 8.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie5,
                    Score = 9.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie6,
                    Score = 8.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie7,
                    Score = 7.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user1,
                    Movie = movie8,
                    Score = 9.2m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },

                // User 2 (Alice) — very similar to User 1
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie1,
                    Score = 8.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie2,
                    Score = 9.2m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie3,
                    Score = 7.8m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie4,
                    Score = 8.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie5,
                    Score = 9.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie6,
                    Score = 3.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie7,
                    Score = 6.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user2,
                    Movie = movie8,
                    Score = 9.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },

                // User 3 (Bob) — moderate overlap, prefers dramas/musicals
                new UserMoviePreference
                {
                    User = user3,
                    Movie = movie1,
                    Score = 5.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user3,
                    Movie = movie2,
                    Score = 4.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user3,
                    Movie = movie5,
                    Score = 8.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user3,
                    Movie = movie6,
                    Score = 9.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user3,
                    Movie = movie7,
                    Score = 9.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user3,
                    Movie = movie8,
                    Score = 8.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },

                // User 4 (Carol) — likes Sci-Fi but differs on drama
                new UserMoviePreference
                {
                    User = user4,
                    Movie = movie1,
                    Score = 9.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user4,
                    Movie = movie3,
                    Score = 8.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user4,
                    Movie = movie4,
                    Score = 9.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user4,
                    Movie = movie6,
                    Score = 2.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user4,
                    Movie = movie7,
                    Score = 3.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },

                // User 5 (Dave) — opposite taste
                new UserMoviePreference
                {
                    User = user5,
                    Movie = movie1,
                    Score = 2.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user5,
                    Movie = movie2,
                    Score = 3.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user5,
                    Movie = movie3,
                    Score = 2.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user5,
                    Movie = movie4,
                    Score = 1.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user5,
                    Movie = movie5,
                    Score = 3.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user5,
                    Movie = movie6,
                    Score = 9.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user5,
                    Movie = movie7,
                    Score = 8.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user5,
                    Movie = movie8,
                    Score = 2.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },

                // User 6 (Eve) — partial overlap
                new UserMoviePreference
                {
                    User = user6,
                    Movie = movie2,
                    Score = 8.8m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user6,
                    Movie = movie5,
                    Score = 9.0m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                },
                new UserMoviePreference
                {
                    User = user6,
                    Movie = movie8,
                    Score = 8.5m,
                    LastModified = DateTime.UtcNow,
                    ChangeFromPreviousValue = 1,
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedUserProfilesAsync()
        {
            if (await _context.UserProfiles.AnyAsync())
            {
                return;
            }

            List<User> users = await _context.Users.OrderBy(user => user.Id).ToListAsync();

            if (users.Count < 6)
            {
                return;
            }

            _context.UserProfiles.AddRange(
                new UserProfile
                {
                    User = users[0],
                    TotalLikes = 0,
                    TotalWatchTimeSeconds = 0,
                    AverageWatchTimeSeconds = 0m,
                    TotalClipsViewed = 0,
                    LikeToViewRatio = 0m,
                    LastUpdated = DateTime.UtcNow,
                },
                new UserProfile
                {
                    User = users[1],
                    TotalLikes = 42,
                    TotalWatchTimeSeconds = 18000,
                    AverageWatchTimeSeconds = 120.5m,
                    TotalClipsViewed = 150,
                    LikeToViewRatio = 0.28m,
                    LastUpdated = DateTime.UtcNow,
                },
                new UserProfile
                {
                    User = users[2],
                    TotalLikes = 15,
                    TotalWatchTimeSeconds = 7200,
                    AverageWatchTimeSeconds = 90.0m,
                    TotalClipsViewed = 80,
                    LikeToViewRatio = 0.19m,
                    LastUpdated = DateTime.UtcNow,
                },
                new UserProfile
                {
                    User = users[3],
                    TotalLikes = 68,
                    TotalWatchTimeSeconds = 32000,
                    AverageWatchTimeSeconds = 145.0m,
                    TotalClipsViewed = 220,
                    LikeToViewRatio = 0.31m,
                    LastUpdated = DateTime.UtcNow,
                },
                new UserProfile
                {
                    User = users[4],
                    TotalLikes = 8,
                    TotalWatchTimeSeconds = 3600,
                    AverageWatchTimeSeconds = 60.0m,
                    TotalClipsViewed = 60,
                    LikeToViewRatio = 0.13m,
                    LastUpdated = DateTime.UtcNow,
                },
                new UserProfile
                {
                    User = users[5],
                    TotalLikes = 25,
                    TotalWatchTimeSeconds = 12000,
                    AverageWatchTimeSeconds = 110.0m,
                    TotalClipsViewed = 109,
                    LikeToViewRatio = 0.23m,
                    LastUpdated = DateTime.UtcNow,
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedSellersAsync()
        {
            bool dummy1Exists = await _context.Users.AnyAsync(user => user.Username == "dummy1");
            bool dummy2Exists = await _context.Users.AnyAsync(user => user.Username == "dummy2");

            if (dummy1Exists && dummy2Exists)
            {
                return;
            }

            if (!dummy1Exists)
            {
                _context.Users.Add(new User
                {
                    Username = "dummy1",
                    Email = "dummy1@gmail.com",
                    PasswordHash = "pass1",
                    Balance = 50000m,
                });
            }

            if (!dummy2Exists)
            {
                _context.Users.Add(new User
                {
                    Username = "dummy2",
                    Email = "dummy2@gmail.com",
                    PasswordHash = "pass2",
                    Balance = 50000m,
                });
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedActiveSalesAsync()
        {
            if (await _context.ActiveSales.AnyAsync())
            {
                return;
            }

            Movie? inception = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Inception");
            Movie? matrix = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "The Matrix");
            Movie? interstellar = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Interstellar");

            if (inception is null || matrix is null || interstellar is null)
            {
                return;
            }

            DateTime currentDate = DateTime.UtcNow;

            _context.ActiveSales.AddRange(
                new ActiveSale
                {
                    Movie = inception,
                    DiscountPercentage = 20.00m,
                    StartTime = currentDate.AddDays(-1),
                    EndTime = currentDate.AddDays(5),
                },
                new ActiveSale
                {
                    Movie = matrix,
                    DiscountPercentage = 20.00m,
                    StartTime = currentDate.AddDays(-1),
                    EndTime = currentDate.AddDays(5),
                },
                new ActiveSale
                {
                    Movie = interstellar,
                    DiscountPercentage = 35.00m,
                    StartTime = currentDate.AddDays(-1),
                    EndTime = currentDate.AddDays(5),
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedMovieEventsAsync()
        {
            if (await _context.MovieEvents.AnyAsync())
            {
                return;
            }

            Movie? inception = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Inception");
            Movie? matrix = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "The Matrix");
            Movie? interstellar = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Interstellar");
            Movie? whiplash = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Whiplash");

            if (inception is null || matrix is null || interstellar is null || whiplash is null)
            {
                return;
            }

            DateTime currentDate = DateTime.UtcNow;

            _context.MovieEvents.AddRange(
                new MovieEvent
                {
                    Movie = inception,
                    Title = "Inception - Midnight Screening",
                    Description = "One-night-only midnight screening with a short pre-show talk.",
                    Date = currentDate.AddDays(7),
                    Location = "Cinema Hall A",
                    TicketPrice = 12.50m,
                    Capacity = 100,
                    PosterUrl = "https://m.media-amazon.com/images/I/71DwIcSgFcS._AC_UF894,1000_QL80_.jpg",
                },
                new MovieEvent
                {
                    Movie = matrix,
                    Title = "The Matrix - Fan Marathon",
                    Description = "Back-to-back screening + trivia. Doors open 18:00.",
                    Date = currentDate.AddDays(14),
                    Location = "Retro Theater",
                    TicketPrice = 18.00m,
                    Capacity = 100,
                    PosterUrl = "https://m.media-amazon.com/images/I/51EG732BV3L.jpg",
                },
                new MovieEvent
                {
                    Movie = interstellar,
                    Title = "Interstellar - Space Night",
                    Description = "Screening followed by a small astronomy Q&A.",
                    Date = currentDate.AddDays(21),
                    Location = "Science Center Auditorium",
                    TicketPrice = 15.00m,
                    Capacity = 100,
                    PosterUrl = "https://m.media-amazon.com/images/I/91vIHsL-zjL._AC_UF894,1000_QL80_.jpg",
                },
                new MovieEvent
                {
                    Movie = whiplash,
                    Title = "Whiplash - Live Jazz Intro",
                    Description = "Short live jazz set before the movie.",
                    Date = currentDate.AddDays(10),
                    Location = "Downtown Arts Cinema",
                    TicketPrice = 14.00m,
                    Capacity = 100,
                    PosterUrl = "https://m.media-amazon.com/images/I/81hKZ6oTqUL._AC_UF894,1000_QL80_.jpg",
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedMovieReviewsAsync()
        {
            if (await _context.MovieReviews.AnyAsync())
            {
                return;
            }

            User? seller1 = await _context.Users.FirstOrDefaultAsync(user => user.Username == "dummy1");
            User? seller2 = await _context.Users.FirstOrDefaultAsync(user => user.Username == "dummy2");

            if (seller1 is null || seller2 is null)
            {
                return;
            }

            Movie? matrix = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "The Matrix");
            Movie? interstellar = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Interstellar");
            Movie? parasite = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Parasite");
            Movie? johnWick = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "John Wick");
            Movie? whiplash = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Whiplash");

            if (matrix is null || interstellar is null || parasite is null || johnWick is null || whiplash is null)
            {
                return;
            }

            DateTime currentDate = DateTime.UtcNow;

            _context.MovieReviews.AddRange(
                new MovieReview
                {
                    Movie = matrix,
                    User = seller1,
                    StarRating = 9m,
                    Comment = "A mind-bending classic with unforgettable world-building.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = matrix,
                    User = seller2,
                    StarRating = 7m,
                    Comment = "Great action and ideas, but definitely not for everyone.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = interstellar,
                    User = seller1,
                    StarRating = 10m,
                    Comment = "Epic, emotional, and incredibly thought-provoking.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = interstellar,
                    User = seller2,
                    StarRating = 8m,
                    Comment = "Beautiful visuals and a satisfying emotional payoff.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = parasite,
                    User = seller1,
                    StarRating = 9m,
                    Comment = "Smart, tense, and darkly funny all the way through.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = parasite,
                    User = seller2,
                    StarRating = 6m,
                    Comment = "Surprisingly entertaining, but the pacing felt uneven.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = johnWick,
                    User = seller1,
                    StarRating = 8m,
                    Comment = "Non-stop style and killer action choreography.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = johnWick,
                    User = seller2,
                    StarRating = 7m,
                    Comment = "Solid thrills and great atmosphere; easy to binge.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = whiplash,
                    User = seller1,
                    StarRating = 9m,
                    Comment = "A brutal, addictive rivalry that stays with you.",
                    CreatedAt = currentDate,
                },
                new MovieReview
                {
                    Movie = whiplash,
                    User = seller2,
                    StarRating = 8m,
                    Comment = "Fantastic performances and a soundtrack that demands attention.",
                    CreatedAt = currentDate,
                });

            await _context.SaveChangesAsync();
        }

        private async Task SeedEquipmentAsync()
        {
            if (await _context.Equipment.AnyAsync())
            {
                return;
            }

            User? seller1 = await _context.Users.FirstOrDefaultAsync(user => user.Username == "dummy1");
            User? seller2 = await _context.Users.FirstOrDefaultAsync(user => user.Username == "dummy2");

            if (seller1 is null || seller2 is null)
            {
                return;
            }

            _context.Equipment.AddRange(
                new Equipment
                {
                    Seller = seller1,
                    Title = "Canon EOS 2000D Kit",
                    Category = "Cameras",
                    Description = "24.1 MP APS-C CMOS sensor. Perfect entry-level DSLR for student films, includes 18-55mm IS II Lens and 1080p cinematic video mode.",
                    Condition = "Good",
                    Price = 1200.00m,
                    ImageUrl = "https://static0.pocketlintimages.com/wordpress/wp-content/uploads/wm/143700-cameras-review-hands-on-canon-eos-2000d-review-image1-xploy5pbva.jpg",
                    Status = EquipmentStatus.Available,
                },
                new Equipment
                {
                    Seller = seller1,
                    Title = "Rode NTG Shotgun Mic",
                    Category = "Audio",
                    Description = "Professional directional condenser microphone. Super-cardioid polar pattern, ideal for isolating dialogue on noisy film sets.",
                    Condition = "New",
                    Price = 1200.00m,
                    ImageUrl = "https://fstudio.vtexassets.com/arquivos/ids/750303-1200-1200",
                    Status = EquipmentStatus.Sold,
                },
                new Equipment
                {
                    Seller = seller2,
                    Title = "Blackmagic Pocket Cinema 6K",
                    Category = "Cameras",
                    Description = "EF Mount, Super 35 HDR sensor, 13 stops of dynamic range and dual native ISO up to 25,600 for incredible low light performance.",
                    Condition = "Like New",
                    Price = 9500.00m,
                    ImageUrl = "https://images.blackmagicdesign.com/images/products/blackmagicpocketcinemacamera/main/pocket-6k-g2-xl.jpg",
                    Status = EquipmentStatus.Available,
                },
                new Equipment
                {
                    Seller = seller1,
                    Title = "DJI RS 3 Pro Gimbal",
                    Category = "Stabilization",
                    Description = "Carbon fiber construction, 4.5kg (10lbs) tested payload. Automated axis locks and LiDAR focusing for professional solo cinematographers.",
                    Condition = "New",
                    Price = 4200.00m,
                    ImageUrl = "https://m.media-amazon.com/images/I/61S6h1S-z3L._AC_SL1500_.jpg",
                    Status = EquipmentStatus.Available,
                },
                new Equipment
                {
                    Seller = seller2,
                    Title = "Atomos Ninja V+ Monitor",
                    Category = "Monitoring",
                    Description = "5-inch 4K HDMI Recording Monitor. 1000 nits brightness for outdoor use, supports ProRes RAW recording directly from camera sensor.",
                    Condition = "Used",
                    Price = 2800.00m,
                    ImageUrl = "https://m.media-amazon.com/images/I/71N-W-vV6NL._AC_SL1500_.jpg",
                    Status = EquipmentStatus.Available,
                });

            await _context.SaveChangesAsync();
        }
        private async Task SeedTriviaQuestionsAsync()
        {
            if (await _context.TriviaQuestions.AnyAsync())
            {
                return;
            }

            // Look up movie IDs for movie-specific questions.
            Movie? inception = await _context.Movies.FirstOrDefaultAsync(m => m.Title == "Inception");
            Movie? darkKnight = await _context.Movies.FirstOrDefaultAsync(m => m.Title == "The Dark Knight");
            Movie? matrix = await _context.Movies.FirstOrDefaultAsync(m => m.Title == "The Matrix");
            Movie? interstellar = await _context.Movies.FirstOrDefaultAsync(m => m.Title == "Interstellar");
            Movie? pulpFiction = await _context.Movies.FirstOrDefaultAsync(m => m.Title == "Pulp Fiction");
            Movie? godfather = await _context.Movies.FirstOrDefaultAsync(m => m.Title == "The Godfather");
            Movie? shawshank = await _context.Movies.FirstOrDefaultAsync(m => m.Title == "The Shawshank Redemption");
            Movie? forrestGump = await _context.Movies.FirstOrDefaultAsync(m => m.Title == "Forrest Gump");
            Movie? laLaLand = await _context.Movies.FirstOrDefaultAsync(m => m.Title == "La La Land");

            var questions = new List<TriviaQuestion>();

            // ── Action ──────────────────────────────────────────────────────────
            questions.AddRange(new[]
            {
                new TriviaQuestion { Category = "Action", QuestionText = "In 'The Dark Knight' (2008), who played the Joker?", OptionA = "Jared Leto", OptionB = "Heath Ledger", OptionC = "Joaquin Phoenix", OptionD = "Jack Nicholson", CorrectOption = 'B' },
                new TriviaQuestion { Category = "Action", QuestionText = "In 'John Wick', what breed of dog is given to John Wick?", OptionA = "German Shepherd", OptionB = "Pit Bull", OptionC = "Beagle", OptionD = "Labrador", CorrectOption = 'C' },
                new TriviaQuestion { Category = "Action", QuestionText = "In 'Mad Max: Fury Road', who is the main villain?", OptionA = "Lord Humungus", OptionB = "The Toecutter", OptionC = "Immortan Joe", OptionD = "Scabrous Scrotus", CorrectOption = 'C' },
                new TriviaQuestion { Category = "Action", QuestionText = "In 'Avengers: Endgame', whose snap destroys Thanos's army?", OptionA = "Thor", OptionB = "Captain America", OptionC = "Hulk", OptionD = "Iron Man", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Action", QuestionText = "In 'Iron Man' (2008), what is Tony Stark's first suit designation?", OptionA = "Mark I", OptionB = "Mark II", OptionC = "Mark III", OptionD = "Mark IV", CorrectOption = 'A' },
                new TriviaQuestion { Category = "Action", QuestionText = "In 'Gladiator' (2000), what is the name of Russell Crowe's character?", OptionA = "Commodus", OptionB = "Proximo", OptionC = "Lucilla", OptionD = "Maximus", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Action", QuestionText = "What city does Batman protect in 'The Dark Knight'?", OptionA = "Metropolis", OptionB = "Star City", OptionC = "Gotham City", OptionD = "Central City", CorrectOption = 'C' },
                new TriviaQuestion { Category = "Action", QuestionText = "In 'Mad Max: Fury Road', what is the name of the female lead?", OptionA = "Toast the Knowing", OptionB = "The Splendid Angharad", OptionC = "The Dag", OptionD = "Furiosa", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Action", QuestionText = "In 'John Wick', what is John Wick's nickname among assassins?", OptionA = "The Ghost", OptionB = "The Shadow", OptionC = "The Boogeyman", OptionD = "The Reaper", CorrectOption = 'C' },
                new TriviaQuestion { Category = "Action", QuestionText = "In 'Iron Man', what company does Tony Stark run?", OptionA = "Hammer Industries", OptionB = "S.H.I.E.L.D.", OptionC = "Oscorp", OptionD = "Stark Industries", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Action", QuestionText = "In 'Gladiator', who orders Maximus's family killed?", OptionA = "Marcus Aurelius", OptionB = "Lucius", OptionC = "Commodus", OptionD = "Proximo", CorrectOption = 'C' },
                new TriviaQuestion { Category = "Action", QuestionText = "In 'Avengers: Endgame', who says 'I am Iron Man' before the final snap?", OptionA = "Thor", OptionB = "Captain America", OptionC = "Hulk", OptionD = "Tony Stark", CorrectOption = 'D' },
            });

            // ── Sci-Fi ───────────────────────────────────────────────────────────
            questions.AddRange(new[]
            {
                new TriviaQuestion { Category = "Sci-Fi", QuestionText = "In 'Inception', what object does Dom Cobb use as a totem to test reality?", OptionA = "A coin", OptionB = "A ring", OptionC = "A spinning top", OptionD = "A die", CorrectOption = 'C' },
                new TriviaQuestion { Category = "Sci-Fi", QuestionText = "In 'The Matrix' (1999), what colour pill does Neo take to see the truth?", OptionA = "Blue", OptionB = "Green", OptionC = "White", OptionD = "Red", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Sci-Fi", QuestionText = "In 'Interstellar', near which planet is the wormhole located?", OptionA = "Mars", OptionB = "Jupiter", OptionC = "Saturn", OptionD = "Neptune", CorrectOption = 'C' },
                new TriviaQuestion { Category = "Sci-Fi", QuestionText = "In 'Blade Runner 2049', what is Ryan Gosling's character called?", OptionA = "Rick Deckard", OptionB = "Roy Batty", OptionC = "J", OptionD = "K", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Sci-Fi", QuestionText = "In 'Inception', what is a person called who can build dream architecture?", OptionA = "Extractor", OptionB = "Forger", OptionC = "Architect", OptionD = "Point Man", CorrectOption = 'C' },
                new TriviaQuestion { Category = "Sci-Fi", QuestionText = "In 'The Matrix', what is the name of Morpheus's ship?", OptionA = "Enterprise", OptionB = "Nebuchadnezzar", OptionC = "Prometheus", OptionD = "Zion", CorrectOption = 'B' },
                new TriviaQuestion { Category = "Sci-Fi", QuestionText = "Who composed the score for 'Interstellar' (2014)?", OptionA = "John Williams", OptionB = "Ennio Morricone", OptionC = "James Horner", OptionD = "Hans Zimmer", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Sci-Fi", QuestionText = "In 'Her' (2013), what is the name of the AI operating system?", OptionA = "Alexa", OptionB = "Samantha", OptionC = "Cortana", OptionD = "Aria", CorrectOption = 'B' },
                new TriviaQuestion { Category = "Sci-Fi", QuestionText = "In 'The Matrix', who offers Neo the choice between pills?", OptionA = "Neo", OptionB = "Trinity", OptionC = "Agent Smith", OptionD = "Morpheus", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Sci-Fi", QuestionText = "Who played Dom Cobb in 'Inception' (2010)?", OptionA = "Matt Damon", OptionB = "Brad Pitt", OptionC = "Tom Hanks", OptionD = "Leonardo DiCaprio", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Sci-Fi", QuestionText = "Who directed 'Blade Runner 2049' (2017)?", OptionA = "Ridley Scott", OptionB = "Christopher Nolan", OptionC = "Denis Villeneuve", OptionD = "James Cameron", CorrectOption = 'C' },
                new TriviaQuestion { Category = "Sci-Fi", QuestionText = "In 'The Matrix', what are the machines using humans for?", OptionA = "Entertainment", OptionB = "Soldiers", OptionC = "Energy", OptionD = "Food", CorrectOption = 'C' },
            });

            // ── Drama ────────────────────────────────────────────────────────────
            questions.AddRange(new[]
            {
                new TriviaQuestion { Category = "Drama", QuestionText = "Who narrates 'The Shawshank Redemption' (1994)?", OptionA = "Andy Dufresne", OptionB = "Captain Hadley", OptionC = "Warden Norton", OptionD = "Red", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Drama", QuestionText = "What is Forrest Gump's famous quote about life?", OptionA = "\"Run, Forrest, run!\"", OptionB = "\"Life is like a box of chocolates\"", OptionC = "\"Stupid is as stupid does\"", OptionD = "\"I'm not a smart man\"", CorrectOption = 'B' },
                new TriviaQuestion { Category = "Drama", QuestionText = "In 'Whiplash' (2014), what is the demanding music teacher's name?", OptionA = "Prof. Mueller", OptionB = "Mr. Davis", OptionC = "Morrison", OptionD = "Fletcher", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Drama", QuestionText = "In 'The Social Network', who is the main character?", OptionA = "Jack Dorsey", OptionB = "Larry Page", OptionC = "Bill Gates", OptionD = "Mark Zuckerberg", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Drama", QuestionText = "In 'The Shawshank Redemption', what crime is Andy Dufresne convicted of?", OptionA = "Armed robbery", OptionB = "Fraud", OptionC = "Kidnapping", OptionD = "Murder", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Drama", QuestionText = "In 'Whiplash', what instrument does the protagonist Andrew play?", OptionA = "Piano", OptionB = "Trumpet", OptionC = "Guitar", OptionD = "Drums", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Drama", QuestionText = "Who plays the title character in 'Forrest Gump' (1994)?", OptionA = "Tom Cruise", OptionB = "Kevin Costner", OptionC = "Tom Hanks", OptionD = "Robin Williams", CorrectOption = 'C' },
                new TriviaQuestion { Category = "Drama", QuestionText = "What is the first rule of Fight Club (1999)?", OptionA = "Always fight", OptionB = "No weapons", OptionC = "You do not talk about Fight Club", OptionD = "Anything goes", CorrectOption = 'C' },
                new TriviaQuestion { Category = "Drama", QuestionText = "In 'The Prestige' (2006), what is the name of the third and final act of a magic trick?", OptionA = "The Pledge", OptionB = "The Turn", OptionC = "The Reveal", OptionD = "The Prestige", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Drama", QuestionText = "In 'Memento' (2000), what condition does the protagonist Leonard suffer from?", OptionA = "Blindness", OptionB = "Short-term memory loss", OptionC = "Deafness", OptionD = "Paralysis", CorrectOption = 'B' },
                new TriviaQuestion { Category = "Drama", QuestionText = "In 'The Social Network', what university does Mark Zuckerberg attend?", OptionA = "MIT", OptionB = "Stanford", OptionC = "Yale", OptionD = "Harvard", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Drama", QuestionText = "Who plays Tyler Durden in 'Fight Club' (1999)?", OptionA = "Leonardo DiCaprio", OptionB = "Edward Norton", OptionC = "Matt Damon", OptionD = "Brad Pitt", CorrectOption = 'D' },
            });

            // ── Animation ────────────────────────────────────────────────────────
            questions.AddRange(new[]
            {
                new TriviaQuestion { Category = "Animation", QuestionText = "In 'Toy Story' (1995), what is the name of the space ranger toy?", OptionA = "Woody", OptionB = "Rex", OptionC = "Hamm", OptionD = "Buzz Lightyear", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Animation", QuestionText = "In 'Finding Nemo' (2003), what type of fish is Nemo?", OptionA = "Goldfish", OptionB = "Angelfish", OptionC = "Swordfish", OptionD = "Clownfish", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Animation", QuestionText = "In 'Up' (2009), what is the name of the old man who floats his house with balloons?", OptionA = "Russell", OptionB = "Charles Muntz", OptionC = "Carl Fredricksen", OptionD = "Kevin", CorrectOption = 'C' },
                new TriviaQuestion { Category = "Animation", QuestionText = "In 'The Lion King' (1994), who is Simba's father?", OptionA = "Scar", OptionB = "Rafiki", OptionC = "Timon", OptionD = "Mufasa", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Animation", QuestionText = "In 'Frozen' (2013), what is the name of Elsa's sister?", OptionA = "Bella", OptionB = "Sofia", OptionC = "Elena", OptionD = "Anna", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Animation", QuestionText = "In 'Finding Nemo', what is the name of the forgetful fish who helps Marlin?", OptionA = "Nemo", OptionB = "Gill", OptionC = "Dory", OptionD = "Bloat", CorrectOption = 'C' },
                new TriviaQuestion { Category = "Animation", QuestionText = "In 'Up', what is the name of the young scout who joins Carl?", OptionA = "Kevin", OptionB = "Dug", OptionC = "Alpha", OptionD = "Russell", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Animation", QuestionText = "In 'The Lion King', what is the name of the villain?", OptionA = "Hyena", OptionB = "Nala", OptionC = "Scar", OptionD = "Zazu", CorrectOption = 'C' },
                new TriviaQuestion { Category = "Animation", QuestionText = "In 'Frozen', what is Elsa's power?", OptionA = "Controlling water", OptionB = "Controlling fire", OptionC = "Controlling wind", OptionD = "Controlling ice and snow", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Animation", QuestionText = "In 'Toy Story', what is Andy's last name?", OptionA = "Smith", OptionB = "Johnson", OptionC = "Williams", OptionD = "Davis", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Animation", QuestionText = "In 'Frozen', what is the name of the friendly snowman?", OptionA = "Jack", OptionB = "Sven", OptionC = "Kristoff", OptionD = "Olaf", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Animation", QuestionText = "In 'Up', what loyal animal companion accompanies Carl?", OptionA = "A bird named Kevin", OptionB = "A cat", OptionC = "A rabbit", OptionD = "A dog named Dug", CorrectOption = 'D' },
            });

            // ── Horror ───────────────────────────────────────────────────────────
            questions.AddRange(new[]
            {
                new TriviaQuestion { Category = "Horror", QuestionText = "In 'The Conjuring' (2013), what is the name of the haunted farmhouse family?", OptionA = "Warrens", OptionB = "Lamson", OptionC = "Turner", OptionD = "Perron", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Horror", QuestionText = "Who directed 'Get Out' (2017)?", OptionA = "James Wan", OptionB = "Wes Craven", OptionC = "Ari Aster", OptionD = "Jordan Peele", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Horror", QuestionText = "Who directed 'A Quiet Place' (2018)?", OptionA = "Jordan Peele", OptionB = "James Wan", OptionC = "Mike Flanagan", OptionD = "John Krasinski", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Horror", QuestionText = "In 'The Conjuring', who are the paranormal investigators?", OptionA = "The Perron family", OptionB = "The Hodgsons", OptionC = "Ed and Lorraine Warren", OptionD = "The Lamberts", CorrectOption = 'C' },
                new TriviaQuestion { Category = "Horror", QuestionText = "In 'Get Out', what hypnotic state are victims sent to?", OptionA = "The Void", OptionB = "The Dark Room", OptionC = "The Abyss", OptionD = "The Sunken Place", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Horror", QuestionText = "In 'A Quiet Place', why must the characters stay silent?", OptionA = "To avoid ghosts", OptionB = "To avoid being seen", OptionC = "Religious ritual", OptionD = "To avoid sound-hunting creatures", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Horror", QuestionText = "In 'Get Out', who plays the lead character Chris Washington?", OptionA = "Jordan Peele", OptionB = "Lakeith Stanfield", OptionC = "Lil Rel Howery", OptionD = "Daniel Kaluuya", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Horror", QuestionText = "In 'A Quiet Place', what family is at the centre of the story?", OptionA = "The Warrens", OptionB = "The Abbotts", OptionC = "The Meades", OptionD = "The Greens", CorrectOption = 'B' },
                new TriviaQuestion { Category = "Horror", QuestionText = "In 'The Conjuring', who plays Lorraine Warren?", OptionA = "Toni Collette", OptionB = "Katie Sackhoff", OptionC = "Vera Farmiga", OptionD = "Naomi Watts", CorrectOption = 'C' },
                new TriviaQuestion { Category = "Horror", QuestionText = "In 'A Quiet Place', who plays Evelyn Abbott?", OptionA = "Cate Blanchett", OptionB = "Sandra Bullock", OptionC = "Amy Adams", OptionD = "Emily Blunt", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Horror", QuestionText = "In 'Get Out', at what type of event does Chris first encounter the sinister plot?", OptionA = "A house party", OptionB = "A church gathering", OptionC = "A wedding", OptionD = "A garden party", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Horror", QuestionText = "In 'The Conjuring', what year is the case set in?", OptionA = "1960", OptionB = "1985", OptionC = "1952", OptionD = "1971", CorrectOption = 'D' },
            });

            // ── Thriller ─────────────────────────────────────────────────────────
            questions.AddRange(new[]
            {
                new TriviaQuestion { Category = "Thriller", QuestionText = "Which country produced 'Parasite' (2019)?", OptionA = "Japan", OptionB = "China", OptionC = "Taiwan", OptionD = "South Korea", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Thriller", QuestionText = "In 'Memento' (2000), what tools does Leonard use to track information?", OptionA = "Notebook", OptionB = "Voice recorder", OptionC = "Map", OptionD = "Tattoos and Polaroid photos", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Thriller", QuestionText = "In 'Shutter Island' (2010), what is the name of the mental institution?", OptionA = "Blackwood", OptionB = "Westmore", OptionC = "Coldwater", OptionD = "Ashecliffe", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Thriller", QuestionText = "Who directed 'Parasite' (2019)?", OptionA = "Park Chan-wook", OptionB = "Kim Jee-woon", OptionC = "Lee Chang-dong", OptionD = "Bong Joon-ho", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Thriller", QuestionText = "In 'Memento', what is the full name of the protagonist?", OptionA = "John Shelby", OptionB = "Teddy Gammell", OptionC = "Jimmy Grantz", OptionD = "Leonard Shelby", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Thriller", QuestionText = "In 'Shutter Island', who plays U.S. Marshal Teddy Daniels?", OptionA = "Matt Damon", OptionB = "Tom Hanks", OptionC = "Ryan Gosling", OptionD = "Leonardo DiCaprio", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Thriller", QuestionText = "'Parasite' won Best Picture at which awards ceremony?", OptionA = "BAFTA", OptionB = "Golden Globes", OptionC = "Emmy Awards", OptionD = "Academy Awards", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Thriller", QuestionText = "In 'Shutter Island', what year is the film set in?", OptionA = "1945", OptionB = "1968", OptionC = "1975", OptionD = "1954", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Thriller", QuestionText = "Who directed 'Memento' (2000)?", OptionA = "Quentin Tarantino", OptionB = "David Fincher", OptionC = "Darren Aronofsky", OptionD = "Christopher Nolan", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Thriller", QuestionText = "In 'Parasite', what do the Kim family discover hidden in the Park basement?", OptionA = "A safe", OptionB = "A time machine", OptionC = "Evidence of crimes", OptionD = "A secret bunker", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Thriller", QuestionText = "Who directed 'Shutter Island' (2010)?", OptionA = "Clint Eastwood", OptionB = "Ridley Scott", OptionC = "Steven Spielberg", OptionD = "Martin Scorsese", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Thriller", QuestionText = "In 'Shutter Island', what is the shocking twist at the end?", OptionA = "The institution doesn't exist", OptionB = "Teddy is already dead", OptionC = "It is all a dream", OptionD = "Teddy is actually a patient", CorrectOption = 'D' },
            });

            // ── Comedy ───────────────────────────────────────────────────────────
            questions.AddRange(new[]
            {
                new TriviaQuestion { Category = "Comedy", QuestionText = "Who directed 'The Grand Budapest Hotel' (2014)?", OptionA = "Tim Burton", OptionB = "Joel Coen", OptionC = "Michel Gondry", OptionD = "Wes Anderson", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Comedy", QuestionText = "Who plays concierge Monsieur Gustave in 'The Grand Budapest Hotel'?", OptionA = "Bill Murray", OptionB = "Owen Wilson", OptionC = "Jason Schwartzman", OptionD = "Ralph Fiennes", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Comedy", QuestionText = "What fictional country is 'The Grand Budapest Hotel' set in?", OptionA = "Fantasia", OptionB = "Ruritania", OptionC = "Grand Duchy of Fenwick", OptionD = "The Republic of Zubrowka", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Comedy", QuestionText = "What is the name of the young lobby boy in 'The Grand Budapest Hotel'?", OptionA = "Agatha", OptionB = "Dmitri", OptionC = "J.G. Jopling", OptionD = "Zero", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Comedy", QuestionText = "What is stolen from Madame D. in 'The Grand Budapest Hotel'?", OptionA = "A necklace", OptionB = "A statue", OptionC = "A diamond", OptionD = "A painting called Boy with Apple", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Comedy", QuestionText = "What is Monsieur Gustave's signature scent in 'The Grand Budapest Hotel'?", OptionA = "Midnight in Paris", OptionB = "Fleur de Rose", OptionC = "Grand Budapest", OptionD = "L'Air de Panache", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Comedy", QuestionText = "Who plays the villain Dmitri in 'The Grand Budapest Hotel'?", OptionA = "Jude Law", OptionB = "Tom Wilkinson", OptionC = "F. Murray Abraham", OptionD = "Adrien Brody", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Comedy", QuestionText = "Which iconic visual style is evident throughout 'The Grand Budapest Hotel'?", OptionA = "Found footage", OptionB = "Black and white photography", OptionC = "Documentary style", OptionD = "Symmetrical compositions and pastel colours", CorrectOption = 'D' },
            });

            // ── Romance ──────────────────────────────────────────────────────────
            questions.AddRange(new[]
            {
                new TriviaQuestion { Category = "Romance", QuestionText = "Who plays Mia Dolan in 'La La Land' (2016)?", OptionA = "Anne Hathaway", OptionB = "Emma Watson", OptionC = "Amy Adams", OptionD = "Emma Stone", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Romance", QuestionText = "Who plays Sebastian Wilder in 'La La Land' (2016)?", OptionA = "Ryan Reynolds", OptionB = "Chris Evans", OptionC = "Jake Gyllenhaal", OptionD = "Ryan Gosling", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Romance", QuestionText = "Who directed 'Titanic' (1997)?", OptionA = "Steven Spielberg", OptionB = "Ridley Scott", OptionC = "Ron Howard", OptionD = "James Cameron", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Romance", QuestionText = "In 'Her' (2013), what actor plays Theodore Twombly?", OptionA = "Ryan Gosling", OptionB = "Oscar Isaac", OptionC = "Adam Driver", OptionD = "Joaquin Phoenix", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Romance", QuestionText = "In 'Titanic', what is the name of Kate Winslet's character?", OptionA = "Ruth", OptionB = "Molly", OptionC = "Margaret", OptionD = "Rose", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Romance", QuestionText = "In 'La La Land', what is the name of the jazz club Sebastian dreams of opening?", OptionA = "The Jazz Palace", OptionB = "Jazz Corner", OptionC = "Sebastian's", OptionD = "Seb's", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Romance", QuestionText = "In 'Titanic', in what year does the historic sinking take place?", OptionA = "1902", OptionB = "1922", OptionC = "1932", OptionD = "1912", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Romance", QuestionText = "Who directed 'La La Land' (2016)?", OptionA = "Derek Cianfrance", OptionB = "Tom McCarthy", OptionC = "Barry Jenkins", OptionD = "Damien Chazelle", CorrectOption = 'D' },
            });

            // ── Crime ────────────────────────────────────────────────────────────
            questions.AddRange(new[]
            {
                new TriviaQuestion { Category = "Crime", QuestionText = "Who plays Vito Corleone in 'The Godfather' (1972)?", OptionA = "Robert De Niro", OptionB = "Al Pacino", OptionC = "James Caan", OptionD = "Marlon Brando", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Crime", QuestionText = "Who directed 'Pulp Fiction' (1994)?", OptionA = "Martin Scorsese", OptionB = "Oliver Stone", OptionC = "Brian De Palma", OptionD = "Quentin Tarantino", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Crime", QuestionText = "In 'The Godfather', who delivers the line 'I'm gonna make him an offer he can't refuse'?", OptionA = "Michael Corleone", OptionB = "Sonny Corleone", OptionC = "Tom Hagen", OptionD = "Vito Corleone", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Crime", QuestionText = "In 'Pulp Fiction', what dance do Vincent Vega and Mia Wallace perform?", OptionA = "The Fox Trot", OptionB = "The Lindy Hop", OptionC = "The Charleston", OptionD = "The Twist", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Crime", QuestionText = "In 'The Wolf of Wall Street' (2013), what is the name of DiCaprio's character?", OptionA = "John Doe", OptionB = "Frank Wheeler", OptionC = "Patrick Bateman", OptionD = "Jordan Belfort", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Crime", QuestionText = "Who plays German bounty hunter Dr. King Schultz in 'Django Unchained'?", OptionA = "Liam Neeson", OptionB = "Michael Fassbender", OptionC = "Tom Hardy", OptionD = "Christoph Waltz", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Crime", QuestionText = "In 'The Godfather', what animal's head is placed in a character's bed?", OptionA = "Dog", OptionB = "Cat", OptionC = "Pig", OptionD = "Horse", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Crime", QuestionText = "Who directed 'The Godfather' (1972)?", OptionA = "Martin Scorsese", OptionB = "Brian De Palma", OptionC = "Sergio Leone", OptionD = "Francis Ford Coppola", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Crime", QuestionText = "What is the name of Jordan Belfort's firm in 'The Wolf of Wall Street'?", OptionA = "Goldman Sachs", OptionB = "Lehman Brothers", OptionC = "JP Morgan", OptionD = "Stratton Oakmont", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Crime", QuestionText = "In 'Pulp Fiction', what are the names of the two hitmen?", OptionA = "Jimmy and Marvin", OptionB = "Butch and Fabienne", OptionC = "Lance and Jody", OptionD = "Vincent and Jules", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Crime", QuestionText = "In what time period is 'Django Unchained' (2012) set?", OptionA = "After the Civil War", OptionB = "During the Civil War", OptionC = "During World War I", OptionD = "Two years before the Civil War", CorrectOption = 'D' },
                new TriviaQuestion { Category = "Crime", QuestionText = "Who plays Django in 'Django Unchained' (2012)?", OptionA = "Will Smith", OptionB = "Denzel Washington", OptionC = "Idris Elba", OptionD = "Jamie Foxx", CorrectOption = 'D' },
            });

            // ── General Movie Knowledge ──────────────────────────────────────────
            questions.AddRange(new[]
            {
                new TriviaQuestion { Category = "General", QuestionText = "The Academy Award statuette is commonly known as what?", OptionA = "The Globe", OptionB = "The Golden Ticket", OptionC = "The Emmy", OptionD = "The Oscar", CorrectOption = 'D' },
                new TriviaQuestion { Category = "General", QuestionText = "Which film won the first Academy Award for Best Picture?", OptionA = "Sunrise", OptionB = "Casablanca", OptionC = "Gone with the Wind", OptionD = "Wings", CorrectOption = 'D' },
                new TriviaQuestion { Category = "General", QuestionText = "Who played James Bond in 'Dr. No' (1962), the first official Bond film?", OptionA = "Roger Moore", OptionB = "Timothy Dalton", OptionC = "Pierce Brosnan", OptionD = "Sean Connery", CorrectOption = 'D' },
                new TriviaQuestion { Category = "General", QuestionText = "Which actress holds the record for the most Academy Award nominations?", OptionA = "Katharine Hepburn", OptionB = "Cate Blanchett", OptionC = "Glenn Close", OptionD = "Meryl Streep", CorrectOption = 'D' },
                new TriviaQuestion { Category = "General", QuestionText = "What was Disney's first feature-length animated film?", OptionA = "Cinderella", OptionB = "Bambi", OptionC = "Fantasia", OptionD = "Snow White and the Seven Dwarfs", CorrectOption = 'D' },
                new TriviaQuestion { Category = "General", QuestionText = "In which year was the first talking feature film 'The Jazz Singer' released?", OptionA = "1915", OptionB = "1920", OptionC = "1935", OptionD = "1927", CorrectOption = 'D' },
                new TriviaQuestion { Category = "General", QuestionText = "What is a 'MacGuffin' in film storytelling?", OptionA = "A type of camera lens", OptionB = "A dramatic plot twist", OptionC = "The final scene of a film", OptionD = "An object that drives the plot but is unimportant in itself", CorrectOption = 'D' },
                new TriviaQuestion { Category = "General", QuestionText = "Who directed 'Schindler's List' (1993)?", OptionA = "Martin Scorsese", OptionB = "Francis Ford Coppola", OptionC = "Stanley Kubrick", OptionD = "Steven Spielberg", CorrectOption = 'D' },
                new TriviaQuestion { Category = "General", QuestionText = "Which film is famous for the line 'I see dead people'?", OptionA = "The Ring", OptionB = "Poltergeist", OptionC = "Ghost", OptionD = "The Sixth Sense", CorrectOption = 'D' },
                new TriviaQuestion { Category = "General", QuestionText = "Which country is primarily associated with the spaghetti western genre?", OptionA = "United States", OptionB = "Spain", OptionC = "Mexico", OptionD = "Italy", CorrectOption = 'D' },
                new TriviaQuestion { Category = "General", QuestionText = "Which director is known for 'Psycho' (1960) and 'Rear Window' (1954)?", OptionA = "David Lynch", OptionB = "Orson Welles", OptionC = "Billy Wilder", OptionD = "Alfred Hitchcock", CorrectOption = 'D' },
                new TriviaQuestion { Category = "General", QuestionText = "Which film famously swept all five major Oscar categories (Picture, Director, Actor, Actress, Screenplay)?", OptionA = "One Flew Over the Cuckoo's Nest", OptionB = "The Godfather", OptionC = "All About Eve", OptionD = "The Silence of the Lambs", CorrectOption = 'D' },
                new TriviaQuestion { Category = "General", QuestionText = "What award is given to the best director at the Cannes Film Festival?", OptionA = "Golden Bear", OptionB = "Golden Lion", OptionC = "Silver Bear", OptionD = "Palme d'Or", CorrectOption = 'D' },
                new TriviaQuestion { Category = "General", QuestionText = "Which famous shower scene is set to Bernard Herrmann's screeching strings?", OptionA = "Vertigo", OptionB = "Rear Window", OptionC = "North by Northwest", OptionD = "Psycho", CorrectOption = 'D' },
                new TriviaQuestion { Category = "General", QuestionText = "What filming technique involves overcranking the camera to produce slow motion?", OptionA = "Dolly shot", OptionB = "Dutch angle", OptionC = "Undercranking", OptionD = "Overcranking", CorrectOption = 'D' },
            });

            // ── Movie-specific questions ─────────────────────────────────────────
            if (inception != null)
            {
                questions.AddRange(new[]
                {
                    new TriviaQuestion { Category = "Sci-Fi", MovieId = inception.Id, QuestionText = "In 'Inception', how many dream levels deep does the team ultimately go?", OptionA = "Two", OptionB = "Three", OptionC = "Five", OptionD = "Four", CorrectOption = 'D' },
                    new TriviaQuestion { Category = "Sci-Fi", MovieId = inception.Id, QuestionText = "In 'Inception', what is the name of the deepest dream level with no way back?", OptionA = "The Void", OptionB = "The Dream", OptionC = "The Subconscious", OptionD = "Limbo", CorrectOption = 'D' },
                    new TriviaQuestion { Category = "Sci-Fi", MovieId = inception.Id, QuestionText = "In 'Inception', who is Cobb's deceased wife whose memory haunts him?", OptionA = "Ariadne", OptionB = "Mal", OptionC = "Yusuf", OptionD = "Philippa", CorrectOption = 'B' },
                    new TriviaQuestion { Category = "Sci-Fi", MovieId = inception.Id, QuestionText = "Who plays the Architect Ariadne in 'Inception'?", OptionA = "Natalie Portman", OptionB = "Keira Knightley", OptionC = "Ellen Page", OptionD = "Rooney Mara", CorrectOption = 'C' },
                    new TriviaQuestion { Category = "Sci-Fi", MovieId = inception.Id, QuestionText = "In 'Inception', what does Cobb want to achieve by completing the job?", OptionA = "Gain wealth", OptionB = "Return to his children in the US", OptionC = "Destroy a corporation", OptionD = "Become the best extractor", CorrectOption = 'B' },
                });
            }

            if (darkKnight != null)
            {
                questions.AddRange(new[]
                {
                    new TriviaQuestion { Category = "Action", MovieId = darkKnight.Id, QuestionText = "In 'The Dark Knight', what does the Joker want Gotham to descend into?", OptionA = "War", OptionB = "Plague", OptionC = "Poverty", OptionD = "Chaos", CorrectOption = 'D' },
                    new TriviaQuestion { Category = "Action", MovieId = darkKnight.Id, QuestionText = "In 'The Dark Knight', who becomes the villain Two-Face?", OptionA = "Commissioner Gordon", OptionB = "Lucius Fox", OptionC = "Harvey Dent", OptionD = "Jonathan Crane", CorrectOption = 'C' },
                    new TriviaQuestion { Category = "Action", MovieId = darkKnight.Id, QuestionText = "In 'The Dark Knight', what is the Joker's iconic question to Batman?", OptionA = "Why so afraid?", OptionB = "Why so sad?", OptionC = "Why so weak?", OptionD = "Why so serious?", CorrectOption = 'D' },
                    new TriviaQuestion { Category = "Action", MovieId = darkKnight.Id, QuestionText = "Who plays Alfred in 'The Dark Knight'?", OptionA = "Ian McKellen", OptionB = "Michael Caine", OptionC = "Anthony Hopkins", OptionD = "Gary Oldman", CorrectOption = 'B' },
                    new TriviaQuestion { Category = "Action", MovieId = darkKnight.Id, QuestionText = "In 'The Dark Knight', what vehicle does Batman primarily use for transport?", OptionA = "The Batwing", OptionB = "The Batpod", OptionC = "The Batboat", OptionD = "The Batmobile (Tumbler)", CorrectOption = 'D' },
                });
            }

            if (matrix != null)
            {
                questions.AddRange(new[]
                {
                    new TriviaQuestion { Category = "Sci-Fi", MovieId = matrix.Id, QuestionText = "In 'The Matrix', who plays Neo?", OptionA = "Will Smith", OptionB = "Laurence Fishburne", OptionC = "Keanu Reeves", OptionD = "Hugo Weaving", CorrectOption = 'C' },
                    new TriviaQuestion { Category = "Sci-Fi", MovieId = matrix.Id, QuestionText = "In 'The Matrix', what is Neo's real name?", OptionA = "Thomas Anderson", OptionB = "James Wilson", OptionC = "David Clark", OptionD = "Robert Hughes", CorrectOption = 'A' },
                    new TriviaQuestion { Category = "Sci-Fi", MovieId = matrix.Id, QuestionText = "In 'The Matrix', who voices/plays the main villain Agent Smith?", OptionA = "Laurence Fishburne", OptionB = "Joe Pantoliano", OptionC = "Hugo Weaving", OptionD = "Keanu Reeves", CorrectOption = 'C' },
                    new TriviaQuestion { Category = "Sci-Fi", MovieId = matrix.Id, QuestionText = "In 'The Matrix', what year does Morpheus believe the story takes place in?", OptionA = "1985", OptionB = "2000", OptionC = "2099", OptionD = "Close to 2199", CorrectOption = 'D' },
                    new TriviaQuestion { Category = "Sci-Fi", MovieId = matrix.Id, QuestionText = "In 'The Matrix', what is the last human city called?", OptionA = "New Zion", OptionB = "The Last City", OptionC = "Zion", OptionD = "Eden", CorrectOption = 'C' },
                });
            }

            if (interstellar != null)
            {
                questions.AddRange(new[]
                {
                    new TriviaQuestion { Category = "Sci-Fi", MovieId = interstellar.Id, QuestionText = "In 'Interstellar', who plays the astronaut Cooper?", OptionA = "Matt Damon", OptionB = "Ryan Gosling", OptionC = "Matthew McConaughey", OptionD = "Tom Hardy", CorrectOption = 'C' },
                    new TriviaQuestion { Category = "Sci-Fi", MovieId = interstellar.Id, QuestionText = "In 'Interstellar', what is the name of the black hole?", OptionA = "Sagittarius A*", OptionB = "Gargantua", OptionC = "Nebula Prime", OptionD = "Event Horizon", CorrectOption = 'B' },
                    new TriviaQuestion { Category = "Sci-Fi", MovieId = interstellar.Id, QuestionText = "In 'Interstellar', what is the name of Cooper's daughter?", OptionA = "Jessica", OptionB = "Emma", OptionC = "Murphy", OptionD = "Sarah", CorrectOption = 'C' },
                    new TriviaQuestion { Category = "Sci-Fi", MovieId = interstellar.Id, QuestionText = "In 'Interstellar', which planet harbours Dr. Mann's frozen world?", OptionA = "Miller's Planet", OptionB = "Brand's Planet", OptionC = "Edmunds' Planet", OptionD = "Mann's Planet", CorrectOption = 'D' },
                    new TriviaQuestion { Category = "Sci-Fi", MovieId = interstellar.Id, QuestionText = "Who directed 'Interstellar' (2014)?", OptionA = "Ridley Scott", OptionB = "Denis Villeneuve", OptionC = "James Cameron", OptionD = "Christopher Nolan", CorrectOption = 'D' },
                });
            }

            if (pulpFiction != null)
            {
                questions.AddRange(new[]
                {
                    new TriviaQuestion { Category = "Crime", MovieId = pulpFiction.Id, QuestionText = "In 'Pulp Fiction', who plays Vincent Vega?", OptionA = "Samuel L. Jackson", OptionB = "John Travolta", OptionC = "Bruce Willis", OptionD = "Harvey Keitel", CorrectOption = 'B' },
                    new TriviaQuestion { Category = "Crime", MovieId = pulpFiction.Id, QuestionText = "In 'Pulp Fiction', what is in the briefcase that everyone covets?", OptionA = "Money", OptionB = "Drugs", OptionC = "Diamonds", OptionD = "Never revealed (deliberately ambiguous)", CorrectOption = 'D' },
                    new TriviaQuestion { Category = "Crime", MovieId = pulpFiction.Id, QuestionText = "In 'Pulp Fiction', what famous restaurant do Vincent and Mia visit?", OptionA = "Big Kahuna Burger", OptionB = "Jack Rabbit Slim's", OptionC = "The Pitt Stop", OptionD = "Red Apple", CorrectOption = 'B' },
                    new TriviaQuestion { Category = "Crime", MovieId = pulpFiction.Id, QuestionText = "In 'Pulp Fiction', what does Jules quote before killing someone?", OptionA = "Psalms 21:4", OptionB = "Proverbs 14:7", OptionC = "Revelations 3:15", OptionD = "Ezekiel 25:17", CorrectOption = 'D' },
                    new TriviaQuestion { Category = "Crime", MovieId = pulpFiction.Id, QuestionText = "In 'Pulp Fiction', what sport does Butch Coolidge (Bruce Willis) compete in?", OptionA = "Wrestling", OptionB = "Boxing", OptionC = "MMA", OptionD = "Kickboxing", CorrectOption = 'B' },
                });
            }

            if (godfather != null)
            {
                questions.AddRange(new[]
                {
                    new TriviaQuestion { Category = "Crime", MovieId = godfather.Id, QuestionText = "In 'The Godfather', who does Michael Corleone ultimately become?", OptionA = "A senator", OptionB = "A lawyer", OptionC = "Head of the Corleone family", OptionD = "A police officer", CorrectOption = 'C' },
                    new TriviaQuestion { Category = "Crime", MovieId = godfather.Id, QuestionText = "In 'The Godfather', what is the name of Vito Corleone's eldest son?", OptionA = "Michael", OptionB = "Fredo", OptionC = "Tom Hagen", OptionD = "Sonny", CorrectOption = 'D' },
                    new TriviaQuestion { Category = "Crime", MovieId = godfather.Id, QuestionText = "Who plays Michael Corleone in 'The Godfather'?", OptionA = "Robert De Niro", OptionB = "James Caan", OptionC = "Al Pacino", OptionD = "Robert Duvall", CorrectOption = 'C' },
                    new TriviaQuestion { Category = "Crime", MovieId = godfather.Id, QuestionText = "In 'The Godfather', what is the name of the rival drug lord who Vito refuses to support?", OptionA = "Barzini", OptionB = "Tattaglia", OptionC = "Virgil Sollozzo", OptionD = "Stracci", CorrectOption = 'C' },
                    new TriviaQuestion { Category = "Crime", MovieId = godfather.Id, QuestionText = "In 'The Godfather', what does the phrase 'going to the mattresses' mean?", OptionA = "Taking a nap", OptionB = "Moving house", OptionC = "Going into hiding", OptionD = "Preparing for all-out gang war", CorrectOption = 'D' },
                });
            }

            if (shawshank != null)
            {
                questions.AddRange(new[]
                {
                    new TriviaQuestion { Category = "Drama", MovieId = shawshank.Id, QuestionText = "In 'The Shawshank Redemption', how does Andy Dufresne escape?", OptionA = "He bribes a guard", OptionB = "He overpowers a guard", OptionC = "He tunnels through the wall over 19 years", OptionD = "He escapes during a prison riot", CorrectOption = 'C' },
                    new TriviaQuestion { Category = "Drama", MovieId = shawshank.Id, QuestionText = "In 'The Shawshank Redemption', what object conceals Andy's escape tunnel?", OptionA = "A bookshelf", OptionB = "A poster of Raquel Welch", OptionC = "A window curtain", OptionD = "A map", CorrectOption = 'B' },
                    new TriviaQuestion { Category = "Drama", MovieId = shawshank.Id, QuestionText = "Who plays Andy Dufresne in 'The Shawshank Redemption'?", OptionA = "Tom Hanks", OptionB = "Morgan Freeman", OptionC = "Tim Robbins", OptionD = "Kevin Spacey", CorrectOption = 'C' },
                    new TriviaQuestion { Category = "Drama", MovieId = shawshank.Id, QuestionText = "In 'The Shawshank Redemption', where do Andy and Red reunite at the end?", OptionA = "Cancun, Mexico", OptionB = "Zihuatanejo, Mexico", OptionC = "Key West, Florida", OptionD = "Havana, Cuba", CorrectOption = 'B' },
                    new TriviaQuestion { Category = "Drama", MovieId = shawshank.Id, QuestionText = "Who plays Red in 'The Shawshank Redemption'?", OptionA = "Denzel Washington", OptionB = "Samuel L. Jackson", OptionC = "Morgan Freeman", OptionD = "Will Smith", CorrectOption = 'C' },
                });
            }

            if (forrestGump != null)
            {
                questions.AddRange(new[]
                {
                    new TriviaQuestion { Category = "Drama", MovieId = forrestGump.Id, QuestionText = "In 'Forrest Gump', what sport does Forrest excel at?", OptionA = "Basketball", OptionB = "Tennis", OptionC = "Swimming", OptionD = "Football (American)", CorrectOption = 'D' },
                    new TriviaQuestion { Category = "Drama", MovieId = forrestGump.Id, QuestionText = "In 'Forrest Gump', what does Forrest shout as Lieutenant Dan falls into the river?", OptionA = "Jump in!", OptionB = "Lieutenant Dan!", OptionC = "Come on!", OptionD = "Watch out!", CorrectOption = 'B' },
                    new TriviaQuestion { Category = "Drama", MovieId = forrestGump.Id, QuestionText = "In 'Forrest Gump', what is the name of Forrest's shrimping boat?", OptionA = "Gulf Star", OptionB = "Sea King", OptionC = "Jenny", OptionD = "Gulf Princess", CorrectOption = 'C' },
                    new TriviaQuestion { Category = "Drama", MovieId = forrestGump.Id, QuestionText = "In 'Forrest Gump', what is the name of Forrest's childhood and lifelong love?", OptionA = "Susan", OptionB = "Mary", OptionC = "Jenny", OptionD = "Laura", CorrectOption = 'C' },
                    new TriviaQuestion { Category = "Drama", MovieId = forrestGump.Id, QuestionText = "Who plays Lieutenant Dan in 'Forrest Gump'?", OptionA = "Tom Sizemore", OptionB = "Gary Sinise", OptionC = "Kevin Bacon", OptionD = "John Cusack", CorrectOption = 'B' },
                });
            }

            if (laLaLand != null)
            {
                questions.AddRange(new[]
                {
                    new TriviaQuestion { Category = "Romance", MovieId = laLaLand.Id, QuestionText = "In 'La La Land', what instrument does Sebastian play?", OptionA = "Guitar", OptionB = "Trumpet", OptionC = "Piano (jazz)", OptionD = "Drums", CorrectOption = 'C' },
                    new TriviaQuestion { Category = "Romance", MovieId = laLaLand.Id, QuestionText = "In 'La La Land', where do Mia and Sebastian share their first dance?", OptionA = "On a rooftop", OptionB = "At a jazz club", OptionC = "On the Hollywood Hills overlook", OptionD = "At a park", CorrectOption = 'C' },
                    new TriviaQuestion { Category = "Romance", MovieId = laLaLand.Id, QuestionText = "How many Academy Awards did 'La La Land' win?", OptionA = "Four", OptionB = "Five", OptionC = "Seven", OptionD = "Six", CorrectOption = 'D' },
                    new TriviaQuestion { Category = "Romance", MovieId = laLaLand.Id, QuestionText = "In 'La La Land', what is Mia Dolan's profession when we first meet her?", OptionA = "Singer", OptionB = "Barista on the Warner Bros. lot", OptionC = "Actress in films", OptionD = "Dancer", CorrectOption = 'B' },
                    new TriviaQuestion { Category = "Romance", MovieId = laLaLand.Id, QuestionText = "At the end of 'La La Land', where does the 'what if' epilogue take place?", OptionA = "Paris", OptionB = "New York", OptionC = "Sebastian's jazz club", OptionD = "The original restaurant", CorrectOption = 'C' },
                });
            }

            _context.TriviaQuestions.AddRange(questions);
            await _context.SaveChangesAsync();
        }

        private async Task FixExistingDataAsync()
        {
            // Fix existing events with 0 capacity from previous migrations
            var zeroCapacityEvents = await _context.MovieEvents.Where(e => e.Capacity == 0).ToListAsync();
            foreach (var e in zeroCapacityEvents)
            {
                e.Capacity = 100;
            }

            // Fix users with 0 or low balance to enable marketplace testing
            var lowBalanceUsers = await _context.Users.Where(u => u.Balance < 50000).ToListAsync();
            foreach (var u in lowBalanceUsers)
            {
                u.Balance = 50000;
            }

            if (zeroCapacityEvents.Any() || lowBalanceUsers.Any())
            {
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedScreeningsAndRoomsAsync()
        {
            if (await _context.Screenings.AnyAsync())
            {
                return;
            }

            var firstMovie = await _context.Movies.FirstOrDefaultAsync();
            var firstUser = await _context.Users.FirstOrDefaultAsync();
            if (firstMovie is null || firstUser is null)
            {
                return;
            }

            var cinemaEvent = new Event
            {
                Id = 0,
                Title = "Demo Screening Event",
                EventDateTime = DateTime.UtcNow.AddDays(7),
                LocationReference = "Hall A",
                TicketPrice = 12.50m,
                MaxCapacity = 100,
                CreatorUserId = firstUser.Id,
            };
            _context.Events.Add(cinemaEvent);
            await _context.SaveChangesAsync();

            var room = new Room
            {
                EventId = cinemaEvent.Id,
                Name = "Hall A",
                Rows = 5,
                Columns = 8,
            };
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var screening = new Screening
            {
                Id = 0,
                EventId = cinemaEvent.Id,
                MovieId = firstMovie.Id,
                RoomId = room.Id,
                ScreeningTime = cinemaEvent.EventDateTime,
            };
            _context.Screenings.Add(screening);
            await _context.SaveChangesAsync();
        }

        private async Task SeedAdditionalRoomsAsync()
        {
            var demoEvent = await _context.Events.FirstOrDefaultAsync(e => e.Title == "Demo Screening Event");
            if (demoEvent is null)
            {
                return;
            }

            var desiredRooms = new[]
            {
                new Room { EventId = demoEvent.Id, Name = "Hall B (IMAX)", Rows = 8, Columns = 10 },
                new Room { EventId = demoEvent.Id, Name = "Hall C (Premium)", Rows = 4, Columns = 6 },
                new Room { EventId = demoEvent.Id, Name = "Hall D (Small)", Rows = 3, Columns = 5 },
                new Room { EventId = demoEvent.Id, Name = "Hall E (Standard)", Rows = 6, Columns = 9 },
            };

            bool addedAny = false;
            foreach (var desired in desiredRooms)
            {
                bool exists = await _context.Rooms.AnyAsync(r => r.EventId == desired.EventId && r.Name == desired.Name);
                if (!exists)
                {
                    if (desired.Rows * desired.Columns > demoEvent.MaxCapacity)
                    {
                        demoEvent.MaxCapacity = desired.Rows * desired.Columns;
                    }
                    _context.Rooms.Add(desired);
                    addedAny = true;
                }
            }

            if (addedAny)
            {
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedMarathonsAsync()
        {
            if (await _context.Marathons.AnyAsync())
            {
                return;
            }

            DateTime now = DateTime.UtcNow;
            string currentWeek = $"{now.Year}-W{System.Globalization.ISOWeek.GetWeekOfYear(now):D2}";

            Movie? inception = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Inception");
            Movie? matrix = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "The Matrix");
            Movie? bladeRunner = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Blade Runner 2049");
            Movie? her = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Her");
            Movie? interstellar = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Interstellar");
            Movie? darkKnight = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "The Dark Knight");
            Movie? gladiator = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Gladiator");
            Movie? madMax = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Mad Max: Fury Road");
            Movie? johnWick = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "John Wick");
            Movie? parasite = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Parasite");
            Movie? shutter = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Shutter Island");
            Movie? memento = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Memento");
            Movie? toyStory = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Toy Story");
            Movie? findingNemo = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Finding Nemo");
            Movie? up = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Up");
            Movie? lionKing = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "The Lion King");
            Movie? godfather = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "The Godfather");
            Movie? pulpFiction = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Pulp Fiction");
            Movie? django = await _context.Movies.FirstOrDefaultAsync(movie => movie.Title == "Django Unchained");

            var scifiMarathon = new Marathon
            {
                Title = "Sci-Fi Minds",
                Description = "Explore the boundaries of reality, AI, and space through cinema's best science fiction.",
                Theme = "Science Fiction",
                PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/vr6ouTojPp0zlSpJvCbODPp19nd.jpg",
                IsActive = true,
                WeekScoping = currentWeek,
                Movies = new List<Movie>(
                    new[] { inception, matrix, bladeRunner, her, interstellar }
                    .OfType<Movie>()),
            };

            var actionMarathon = new Marathon
            {
                Title = "Pure Adrenaline",
                Description = "Back-to-back action masterpieces that redefined the genre.",
                Theme = "Action",
                PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/a1UL3FTJDgQikYIebnMDhTPFVfm.jpg",
                IsActive = true,
                WeekScoping = currentWeek,
                Movies = new List<Movie>(
                    new[] { darkKnight, gladiator, madMax, johnWick }
                    .OfType<Movie>()),
            };

            var thrillerMarathon = new Marathon
            {
                Title = "Mind Benders",
                Description = "Psychological thrillers that will keep you second-guessing until the credits roll.",
                Theme = "Thriller",
                PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/7IiTTgloJzvGI1TAYymCfbfl3vT.jpg",
                IsActive = true,
                WeekScoping = currentWeek,
                Movies = new List<Movie>(
                    new[] { parasite, shutter, memento }
                    .OfType<Movie>()),
            };

            var animationMarathon = new Marathon
            {
                Title = "Forever Young",
                Description = "Pixar and Disney classics that prove animation is for everyone.",
                Theme = "Animation",
                PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/RyAWIbSmt4xC859hyQ43wUumM9.jpg",
                IsActive = true,
                WeekScoping = currentWeek,
                Movies = new List<Movie>(
                    new[] { toyStory, findingNemo, up, lionKing }
                    .OfType<Movie>()),
            };

            var crimeMarathon = new Marathon
            {
                Title = "Crime & Punishment",
                Description = "The finest crime dramas — morally complex, stylishly executed.",
                Theme = "Crime",
                PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/z4lzwl3Gff5IOWKGiYY7gUFYXUb.jpg",
                IsActive = true,
                WeekScoping = currentWeek,
                Movies = new List<Movie>(
                    new[] { godfather, pulpFiction, django }
                    .OfType<Movie>()),
            };

            _context.Marathons.AddRange(
                scifiMarathon,
                actionMarathon,
                thrillerMarathon,
                animationMarathon,
                crimeMarathon);

            await _context.SaveChangesAsync();

            var eliteScifi = new Marathon
            {
                Title = "Sci-Fi Elite",
                Description = "Only for those who have proven their Sci-Fi knowledge. Harder questions, deeper cuts.",
                Theme = "Science Fiction — Elite",
                PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/9pz5bPDufSrJd8yTNjp0apTAVf8.jpg",
                IsActive = true,
                WeekScoping = currentWeek,
                PrerequisiteMarathonId = scifiMarathon.Id,
                Movies = new List<Movie>(
                    new[] { inception, interstellar, bladeRunner }
                    .OfType<Movie>()),
            };

            var eliteAction = new Marathon
            {
                Title = "Action Elite",
                Description = "A curated action gauntlet reserved for verified adrenaline junkies.",
                Theme = "Action — Elite",
                PosterUrl = "https://image.tmdb.org/t/p/w600_and_h900_bestv2/89BIhHMvGAoxQkniNFB8ENrfzxk.jpg",
                IsActive = true,
                WeekScoping = currentWeek,
                PrerequisiteMarathonId = actionMarathon.Id,
                Movies = new List<Movie>(
                    new[] { darkKnight, madMax, johnWick }
                    .OfType<Movie>()),
            };

            _context.Marathons.AddRange(eliteScifi, eliteAction);
            await _context.SaveChangesAsync();
        }

        private async Task SeedMarathonProgressionsAsync()
        {
            if (await _context.MarathonProgressions.AnyAsync())
            {
                return;
            }

            List<User> users = await _context.Users.OrderBy(user => user.Id).ToListAsync();
            List<Marathon> marathons = await _context.Marathons.ToListAsync();

            if (users.Count < 3 || marathons.Count == 0)
            {
                return;
            }

            User user1 = users.First(user => user.Username == "User1");
            User alice = users.First(user => user.Username == "Alice");
            User bob = users.First(user => user.Username == "Bob");

            Marathon? scifi = marathons.FirstOrDefault(marathon => marathon.Title == "Sci-Fi Minds");
            Marathon? action = marathons.FirstOrDefault(marathon => marathon.Title == "Pure Adrenaline");
            Marathon? thriller = marathons.FirstOrDefault(marathon => marathon.Title == "Mind Benders");
            Marathon? elite = marathons.FirstOrDefault(marathon => marathon.Title == "Sci-Fi Elite");

            var progressions = new List<MarathonProgress>();

            if (scifi != null)
            {
                int scifiMovieCount = scifi.Movies?.Count ?? 5;

                progressions.Add(new MarathonProgress
                {
                    UserId = user1.Id,
                    MarathonId = scifi.Id,
                    JoinedAt = DateTime.UtcNow.AddDays(-6),
                    CompletedMoviesCount = scifiMovieCount,
                    TriviaAccuracy = 100.0,
                    FinishedAt = DateTime.UtcNow.AddDays(-1),
                });

                progressions.Add(new MarathonProgress
                {
                    UserId = alice.Id,
                    MarathonId = scifi.Id,
                    JoinedAt = DateTime.UtcNow.AddDays(-4),
                    CompletedMoviesCount = 2,
                    TriviaAccuracy = 83.5,
                    FinishedAt = null,
                });

                progressions.Add(new MarathonProgress
                {
                    UserId = bob.Id,
                    MarathonId = scifi.Id,
                    JoinedAt = DateTime.UtcNow.AddDays(-1),
                    CompletedMoviesCount = 0,
                    TriviaAccuracy = 0,
                    FinishedAt = null,
                });
            }

            if (action != null)
            {
                int actionMovieCount = action.Movies?.Count ?? 4;

                progressions.Add(new MarathonProgress
                {
                    UserId = alice.Id,
                    MarathonId = action.Id,
                    JoinedAt = DateTime.UtcNow.AddDays(-5),
                    CompletedMoviesCount = actionMovieCount,
                    TriviaAccuracy = 91.7,
                    FinishedAt = DateTime.UtcNow.AddDays(-2),
                });

                progressions.Add(new MarathonProgress
                {
                    UserId = user1.Id,
                    MarathonId = action.Id,
                    JoinedAt = DateTime.UtcNow.AddDays(-3),
                    CompletedMoviesCount = 2,
                    TriviaAccuracy = 66.7,
                    FinishedAt = null,
                });
            }

            if (thriller != null)
            {
                progressions.Add(new MarathonProgress
                {
                    UserId = bob.Id,
                    MarathonId = thriller.Id,
                    JoinedAt = DateTime.UtcNow.AddDays(-2),
                    CompletedMoviesCount = 1,
                    TriviaAccuracy = 75.0,
                    FinishedAt = null,
                });

                progressions.Add(new MarathonProgress
                {
                    UserId = alice.Id,
                    MarathonId = thriller.Id,
                    JoinedAt = DateTime.UtcNow.AddDays(-1),
                    CompletedMoviesCount = 0,
                    TriviaAccuracy = 0,
                    FinishedAt = null,
                });
            }

            if (elite != null)
            {
                progressions.Add(new MarathonProgress
                {
                    UserId = user1.Id,
                    MarathonId = elite.Id,
                    JoinedAt = DateTime.UtcNow.AddDays(-1),
                    CompletedMoviesCount = 1,
                    TriviaAccuracy = 100.0,
                    FinishedAt = null,
                });
            }

            _context.MarathonProgressions.AddRange(progressions);
            await _context.SaveChangesAsync();
        }
    }
}

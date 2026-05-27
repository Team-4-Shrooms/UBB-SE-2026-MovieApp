using System.Net;
using System.Net.Http.Json;
using MovieApp.WebApi.DTOs;

namespace MovieApp.Tests.Integration
{
    public sealed class MovieEndpointsIntegrationTests : IClassFixture<MovieAppWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public MovieEndpointsIntegrationTests(MovieAppWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllMovies_ReturnsHttp200WithMovieList()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/movies");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            List<MovieDto>? movies = await response.Content.ReadFromJsonAsync<List<MovieDto>>();

            Assert.NotNull(movies);
            Assert.NotEmpty(movies);
        }

        [Fact]
        public async Task GetAllMovies_ReturnsCatalogMoviesWithRequiredFields()
        {
            List<MovieDto>? movies = await _httpClient.GetFromJsonAsync<List<MovieDto>>("/api/movies");

            Assert.NotNull(movies);
            Assert.NotEmpty(movies);

            foreach (MovieDto movie in movies)
            {
                Assert.True(movie.Id > 0, $"Movie '{movie.Title}' has invalid Id.");
                Assert.False(string.IsNullOrWhiteSpace(movie.Title), $"Movie with Id {movie.Id} has empty Title.");
                Assert.False(string.IsNullOrWhiteSpace(movie.Genre), $"Movie '{movie.Title}' has empty Genre.");
                Assert.True(movie.Rating >= 0, $"Movie '{movie.Title}' has invalid Rating.");
                Assert.NotNull(movie.PosterUrl);
                Assert.NotNull(movie.Synopsis);
            }
        }

        [Fact]
        public async Task GetAllMovies_ReturnsMoviesThatCanBeFilteredByGenreClientSide()
        {
            List<MovieDto>? movies = await _httpClient.GetFromJsonAsync<List<MovieDto>>("/api/movies");

            Assert.NotNull(movies);
            Assert.NotEmpty(movies);

            string existingGenre = movies
                .Select(movie => movie.Genre)
                .First(genre => !string.IsNullOrWhiteSpace(genre));

            List<MovieDto> filteredMovies = movies
                .Where(movie => movie.Genre.Equals(existingGenre, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Assert.NotEmpty(filteredMovies);
            Assert.All(filteredMovies, movie =>
                Assert.Equal(existingGenre, movie.Genre, ignoreCase: true));
        }

        [Fact]
        public async Task GetMovieById_ExistingMovie_ReturnsHttp200()
        {
            List<MovieDto>? movies = await _httpClient.GetFromJsonAsync<List<MovieDto>>("/api/movies");

            Assert.NotNull(movies);
            Assert.NotEmpty(movies);

            MovieDto existingMovie = movies.First();

            HttpResponseMessage response = await _httpClient.GetAsync($"/api/movies/{existingMovie.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetMovieById_ExistingMovie_ReturnsCorrectMovieData()
        {
            List<MovieDto>? movies = await _httpClient.GetFromJsonAsync<List<MovieDto>>("/api/movies");

            Assert.NotNull(movies);
            Assert.NotEmpty(movies);

            MovieDto expectedMovie = movies.First();

            MovieDto? actualMovie = await _httpClient.GetFromJsonAsync<MovieDto>(
                $"/api/movies/{expectedMovie.Id}");

            Assert.NotNull(actualMovie);
            Assert.Equal(expectedMovie.Id, actualMovie.Id);
            Assert.Equal(expectedMovie.Title, actualMovie.Title);
            Assert.Equal(expectedMovie.Genre, actualMovie.Genre);
            Assert.Equal(expectedMovie.Rating, actualMovie.Rating);
            Assert.Equal(expectedMovie.PosterUrl, actualMovie.PosterUrl);
            Assert.Equal(expectedMovie.Synopsis, actualMovie.Synopsis);
        }

        [Fact]
        public async Task GetMovieById_NonExistingMovie_ReturnsNoContent()
        {
            List<MovieDto>? movies = await _httpClient.GetFromJsonAsync<List<MovieDto>>("/api/movies");

            Assert.NotNull(movies);
            Assert.NotEmpty(movies);

            int nonExistingMovieId = movies.Max(movie => movie.Id) + 1;

            HttpResponseMessage response = await _httpClient.GetAsync($"/api/movies/{nonExistingMovieId}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}

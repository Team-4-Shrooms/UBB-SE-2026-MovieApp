using System.Net;
using System.Net.Http.Json;
using MovieApp.WebApi.DTOs;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.Tests.Integration
{
    public sealed class InventoryEndpointsIntegrationTests : IClassFixture<MovieAppWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public InventoryEndpointsIntegrationTests(MovieAppWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetOwnedMovies_ExistingUser_ReturnsHttp200WithItemList()
        {
            int userId = 1;

            HttpResponseMessage response = await _httpClient.GetAsync(
                $"/api/inventory/users/{userId}/movies");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            List<MovieDto>? movies = await response.Content.ReadFromJsonAsync<List<MovieDto>>();

            Assert.NotNull(movies);
        }

        [Fact]
        public async Task GetOwnedMovies_AfterAddingOwnedMovie_ReturnsHttp200WithPurchasedMovieAndCorrectData()
        {
            int userId = 1;

            List<MovieDto>? catalogMovies = await _httpClient.GetFromJsonAsync<List<MovieDto>>("/api/movies");

            Assert.NotNull(catalogMovies);
            Assert.NotEmpty(catalogMovies);

            List<MovieDto>? inventoryBeforeAdd = await _httpClient.GetFromJsonAsync<List<MovieDto>>(
                $"/api/inventory/users/{userId}/movies");

            Assert.NotNull(inventoryBeforeAdd);

            MovieDto movieToAdd = catalogMovies.First(movie =>
                inventoryBeforeAdd.All(ownedMovie => ownedMovie.Id != movie.Id));

            AddOwnedMovieRequestBody requestBody = new AddOwnedMovieRequestBody
            {
                UserId = userId,
                MovieId = movieToAdd.Id
            };

            HttpResponseMessage addResponse = await _httpClient.PostAsJsonAsync(
                "/api/inventory/ownedmovies",
                requestBody);

            Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);

            HttpResponseMessage inventoryResponse = await _httpClient.GetAsync(
                $"/api/inventory/users/{userId}/movies");

            Assert.Equal(HttpStatusCode.OK, inventoryResponse.StatusCode);

            List<MovieDto>? inventoryAfterAdd = await inventoryResponse.Content.ReadFromJsonAsync<List<MovieDto>>();

            Assert.NotNull(inventoryAfterAdd);

            MovieDto ownedMovie = inventoryAfterAdd.Single(movie => movie.Id == movieToAdd.Id);

            Assert.Equal(movieToAdd.Id, ownedMovie.Id);
            Assert.Equal(movieToAdd.Title, ownedMovie.Title);
            Assert.Equal(movieToAdd.Genre, ownedMovie.Genre);
            Assert.Equal(movieToAdd.Rating, ownedMovie.Rating);
            Assert.Equal(movieToAdd.PosterUrl, ownedMovie.PosterUrl);
            Assert.Equal(movieToAdd.Synopsis, ownedMovie.Synopsis);
        }
    }
}

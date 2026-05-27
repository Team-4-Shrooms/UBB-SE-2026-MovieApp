using System.Net;
using System.Net.Http.Json;
using MovieApp.WebApi.DTOs;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.Tests.Integration
{
    /// <summary>
    /// Integration tests for the ActiveSales WebAPI endpoints.
    /// </summary>
    public sealed class ActiveSalesEndpointsIntegrationTests : IClassFixture<MovieAppWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public ActiveSalesEndpointsIntegrationTests(MovieAppWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetCurrentSales_SeededDatabase_ReturnsOkStatusCode()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/active-sales/current");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // The endpoint returns Dictionary<int, decimal> (movieId -> bestDiscountPercent),
        // not a list of ActiveSaleDto.

        [Fact]
        public async Task GetCurrentSales_SeededDatabase_ReturnsNonEmptyList()
        {
            Dictionary<int, decimal>? currentSales = await _httpClient.GetFromJsonAsync<Dictionary<int, decimal>>("/api/active-sales/current");

            Assert.NotNull(currentSales);
            Assert.NotEmpty(currentSales!);
        }

        [Fact]
        public async Task GetCurrentSales_SeededDatabase_AllSalesHaveMovieReference()
        {
            Dictionary<int, decimal>? currentSales = await _httpClient.GetFromJsonAsync<Dictionary<int, decimal>>("/api/active-sales/current");

            bool allHaveValidMovieId = currentSales!.All(entry => entry.Key > 0);

            Assert.True(allHaveValidMovieId);
        }

        [Fact]
        public async Task GetCurrentSales_SeededDatabase_AllSalesHavePositiveDiscount()
        {
            Dictionary<int, decimal>? currentSales = await _httpClient.GetFromJsonAsync<Dictionary<int, decimal>>("/api/active-sales/current");

            bool allHavePositiveDiscount = currentSales!.All(entry => entry.Value > 0);

            Assert.True(allHavePositiveDiscount);
        }
    }
}

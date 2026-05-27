using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MovieApp.WebApi.DTOs;

namespace MovieApp.Tests.Integration
{
    public sealed class EquipmentEndpointsIntegrationTests : IClassFixture<MovieAppWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public EquipmentEndpointsIntegrationTests(MovieAppWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();

            // FIX: Uses "Test" to match the exact string registered in MovieAppWebApplicationFactory
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");
        }

        [Fact]
        public async Task GetAvailableEquipment_ReturnsHttp200WithValidMarketplaceListings()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/equipment/available");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            List<EquipmentDto>? availableEquipment =
                await response.Content.ReadFromJsonAsync<List<EquipmentDto>>();

            Assert.NotNull(availableEquipment);
            Assert.NotEmpty(availableEquipment);

            foreach (EquipmentDto equipment in availableEquipment)
            {
                Assert.True(equipment.Id > 0);
                Assert.False(string.IsNullOrWhiteSpace(equipment.Title));
                Assert.True(equipment.Price >= 0);
                Assert.NotNull(equipment.Seller);
            }
        }

        [Fact]
        public async Task PurchaseEquipment_ValidPurchase_UpdatesWalletInventoryAndMarketplace()
        {
            int buyerId = 1;

            decimal balanceBeforePurchase =
                await _httpClient.GetFromJsonAsync<decimal>($"/api/users/{buyerId}/balance");

            List<EquipmentDto>? availableBeforePurchase =
                await _httpClient.GetFromJsonAsync<List<EquipmentDto>>("/api/equipment/available");

            Assert.NotNull(availableBeforePurchase);
            Assert.NotEmpty(availableBeforePurchase);

            EquipmentDto equipmentToPurchase = availableBeforePurchase.First();

            HttpResponseMessage purchaseResponse = await _httpClient.PostAsJsonAsync(
                $"/api/equipment/{equipmentToPurchase.Id}/purchase",
                new
                {
                    BuyerId = buyerId,
                    Price = equipmentToPurchase.Price,
                    Address = "Integration Test Address"
                });

            Assert.Equal(HttpStatusCode.OK, purchaseResponse.StatusCode);

            decimal balanceAfterPurchase =
                await _httpClient.GetFromJsonAsync<decimal>($"/api/users/{buyerId}/balance");

            Assert.Equal(balanceBeforePurchase - equipmentToPurchase.Price, balanceAfterPurchase);

            List<EquipmentDto>? ownedEquipment =
                await _httpClient.GetFromJsonAsync<List<EquipmentDto>>(
                    $"/api/inventory/users/{buyerId}/equipment");

            Assert.NotNull(ownedEquipment);
            Assert.Contains(ownedEquipment, equipment => equipment.Id == equipmentToPurchase.Id);

            List<EquipmentDto>? availableAfterPurchase =
                await _httpClient.GetFromJsonAsync<List<EquipmentDto>>("/api/equipment/available");

            Assert.NotNull(availableAfterPurchase);
            Assert.DoesNotContain(availableAfterPurchase, equipment => equipment.Id == equipmentToPurchase.Id);
        }
    }
}

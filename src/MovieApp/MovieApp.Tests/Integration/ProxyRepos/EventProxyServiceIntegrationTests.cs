using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class EventProxyServiceIntegrationTests
{

    [Fact]
    public async Task GetAvailableEventsAsync_SeededDatabase_ReturnsNonEmptyList()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        EventProxyService eventRepository = new(testContext.ApiClient);

        List<MovieEvent> allEvents = await eventRepository.GetAvailableEventsAsync();

        Assert.NotEmpty(allEvents);
    }

    [Fact]
    public async Task PurchaseTicketAsync_ExistingUserAndEvent_SetsUserHasTicketTrue()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        EventProxyService eventRepository = new(testContext.ApiClient);
        List<MovieEvent> allEvents = await eventRepository.GetAvailableEventsAsync();
        int eventId = allEvents[0].Id;

        await eventRepository.PurchaseTicketAsync(ProxyRepoSeedIds.SeededUserId, eventId);

        bool userHasTicket = await eventRepository.UserHasTicketAsync(ProxyRepoSeedIds.SeededUserId, eventId);

        Assert.True(userHasTicket);
    }

    [Fact]
    public async Task UserHasTicket_BeforePurchase_ReturnsFalse()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        EventProxyService eventService = new(testContext.ApiClient);
        List<MovieEvent> events = await eventService.GetAvailableEventsAsync();
        int eventId = events[0].Id;

        bool hasTicket = await eventService.UserHasTicketAsync(ProxyRepoSeedIds.SeededUserId, eventId);

        Assert.False(hasTicket);
    }

    [Fact]
    public async Task GetEventsByMovieIdAsync_SeededMovieId_ReturnsNonNullList()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        EventProxyService eventService = new(testContext.ApiClient);

        List<MovieEvent> events = await eventService.GetEventsByMovieIdAsync(ProxyRepoSeedIds.SeededMovieId);

        Assert.NotNull(events);
    }

    [Fact]
    public async Task GetEventByIdAsync_SeededEvent_ReturnsEventWithMatchingId()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        EventProxyService eventService = new(testContext.ApiClient);
        List<MovieEvent> events = await eventService.GetAvailableEventsAsync();
        int eventId = events[0].Id;

        MovieEvent? found = await eventService.GetEventByIdAsync(eventId);

        Assert.NotNull(found);
        Assert.Equal(eventId, found.Id);
    }


    [Fact]
    public async Task GetAllEvents_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync("api/events");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllEventsByMovieId_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync(
            $"api/events?movieId={ProxyRepoSeedIds.SeededMovieId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PurchaseTicket_ValidUserAndEvent_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        EventProxyService eventService = new(testContext.ApiClient);
        List<MovieEvent> events = await eventService.GetAvailableEventsAsync();
        int eventId = events[0].Id;

        HttpResponseMessage response = await testContext.HttpClient.PostAsJsonAsync(
            $"api/events/{eventId}/purchase-ticket",
            new { UserId = ProxyRepoSeedIds.SeededUserId });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetEventById_SeededEvent_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        EventProxyService eventService = new(testContext.ApiClient);
        List<MovieEvent> events = await eventService.GetAvailableEventsAsync();
        int eventId = events[0].Id;

        HttpResponseMessage response = await testContext.HttpClient.GetAsync($"api/events/{eventId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UserHasTicketEndpoint_AfterPurchase_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        EventProxyService eventService = new(testContext.ApiClient);
        List<MovieEvent> events = await eventService.GetAvailableEventsAsync();
        int eventId = events[0].Id;

        await eventService.PurchaseTicketAsync(ProxyRepoSeedIds.SeededUserId, eventId);

        HttpResponseMessage response = await testContext.HttpClient.GetAsync(
            $"api/events/{eventId}/tickets/{ProxyRepoSeedIds.SeededUserId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task EventsAndScreenings_HaveSeparateRoutes_BothReturn200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage eventsResponse = await testContext.HttpClient.GetAsync("api/events");
        HttpResponseMessage screeningsResponse = await testContext.HttpClient.GetAsync("api/screenings");

        Assert.Equal(HttpStatusCode.OK, eventsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, screeningsResponse.StatusCode);
    }

    [Fact]
    public async Task GetAllEvents_JsonShape_ContainsEventFieldsNotScreeningFields()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        string json = await testContext.HttpClient.GetStringAsync("api/events");
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        if (root.GetArrayLength() > 0)
        {
            JsonElement first = root[0];
            Assert.True(first.TryGetProperty("title", out _), "Event JSON must contain 'title'");
            Assert.False(first.TryGetProperty("screeningTime", out _), "Event JSON must not contain 'screeningTime'");
            Assert.False(first.TryGetProperty("roomId", out _), "Event JSON must not contain 'roomId'");
        }
    }

    [Fact]
    public async Task GetAllScreenings_JsonShape_ContainsScreeningFieldsNotEventFields()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        string json = await testContext.HttpClient.GetStringAsync("api/screenings");
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        if (root.GetArrayLength() > 0)
        {
            JsonElement first = root[0];
            Assert.True(first.TryGetProperty("screeningTime", out _), "Screening JSON must contain 'screeningTime'");
            Assert.False(first.TryGetProperty("date", out _), "Screening JSON must not contain 'date' (event-only field)");
        }
    }
}

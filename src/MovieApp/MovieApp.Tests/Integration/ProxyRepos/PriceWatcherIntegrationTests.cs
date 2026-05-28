using System.Net;
using System.Net.Http.Json;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy.Services;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class PriceWatcherIntegrationTests
{
    [Fact]
    public async Task AddWatch_ValidWatcher_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        PriceWatcher watcher = new()
        {
            EventId = 501,
            EventTitle = "Test Movie Event",
            TargetPrice = 9.99m,
        };

        HttpResponseMessage response = await testContext.HttpClient.PostAsJsonAsync(
            "api/price-watchers", watcher);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AddWatch_ValidWatcher_ReturnsTrueInBody()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        PriceWatcher watcher = new()
        {
            EventId = 502,
            EventTitle = "Test Movie Event",
            TargetPrice = 15.00m,
        };

        HttpResponseMessage response = await testContext.HttpClient.PostAsJsonAsync(
            "api/price-watchers", watcher);

        bool? added = await response.Content.ReadFromJsonAsync<bool>();
        Assert.True(added);
    }

    [Fact]
    public async Task AddWatchAsync_ViaProxy_ReturnsTrueOnSuccess()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        PriceWatcherProxyService priceWatcherService = new(testContext.ApiClient);

        PriceWatcher watcher = new()
        {
            EventId = 503,
            EventTitle = "Proxy Test Event",
            TargetPrice = 12.50m,
        };

        bool added = await priceWatcherService.AddWatchAsync(watcher);

        Assert.True(added);
    }


    [Fact]
    public async Task RemoveWatch_ExistingWatcher_ReturnsHttp204()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        PriceWatcher watcher = new()
        {
            EventId = 601,
            EventTitle = "Event To Delete",
            TargetPrice = 5.00m,
        };

        HttpResponseMessage addResponse = await testContext.HttpClient.PostAsJsonAsync(
            "api/price-watchers", watcher);
        addResponse.EnsureSuccessStatusCode();

        HttpResponseMessage deleteResponse = await testContext.HttpClient.DeleteAsync(
            $"api/price-watchers/{watcher.EventId}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task RemoveWatchAsync_ViaProxy_CompletesWithoutException()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        PriceWatcherProxyService priceWatcherService = new(testContext.ApiClient);

        PriceWatcher watcher = new()
        {
            EventId = 602,
            EventTitle = "Proxy Delete Event",
            TargetPrice = 7.50m,
        };

        await priceWatcherService.AddWatchAsync(watcher);

        await priceWatcherService.RemoveWatchAsync(watcher.EventId);
    }

    [Fact]
    public async Task AddThenRemoveWatch_WatcherNoLongerPresent()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        PriceWatcherProxyService priceWatcherService = new(testContext.ApiClient);

        PriceWatcher watcher = new()
        {
            EventId = 701,
            EventTitle = "Round-Trip Event",
            TargetPrice = 20.00m,
        };

        await priceWatcherService.AddWatchAsync(watcher);

        bool watchingBefore = await priceWatcherService.IsWatchingAsync(watcher.EventId);
        Assert.True(watchingBefore);

        await priceWatcherService.RemoveWatchAsync(watcher.EventId);

        bool watchingAfter = await priceWatcherService.IsWatchingAsync(watcher.EventId);
        Assert.False(watchingAfter);
    }

    [Fact]
    public async Task GetAllWatchedEvents_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync("api/price-watchers");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllWatchedEventsAsync_ViaProxy_ReturnsNonNullList()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        PriceWatcherProxyService priceWatcherService = new(testContext.ApiClient);

        List<PriceWatcher> watchers = await priceWatcherService.GetAllWatchedEventsAsync();

        Assert.NotNull(watchers);
    }

    [Fact]
    public async Task GetAllWatchedEvents_AfterAddingOne_ContainsAddedWatcher()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        PriceWatcherProxyService priceWatcherService = new(testContext.ApiClient);

        PriceWatcher watcher = new()
        {
            EventId = 801,
            EventTitle = "Persisted Event",
            TargetPrice = 25.00m,
        };

        await priceWatcherService.AddWatchAsync(watcher);

        List<PriceWatcher> watchers = await priceWatcherService.GetAllWatchedEventsAsync();

        Assert.Contains(watchers, w => w.EventId == watcher.EventId);
    }

    [Fact]
    public async Task IsWatchingAsync_BeforeAdding_ReturnsFalse()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        PriceWatcherProxyService priceWatcherService = new(testContext.ApiClient);

        bool watching = await priceWatcherService.IsWatchingAsync(99999);

        Assert.False(watching);
    }

    [Fact]
    public async Task IsWatchingAsync_AfterAdding_ReturnsTrue()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        PriceWatcherProxyService priceWatcherService = new(testContext.ApiClient);

        PriceWatcher watcher = new()
        {
            EventId = 901,
            EventTitle = "IsWatching Test Event",
            TargetPrice = 18.00m,
        };

        await priceWatcherService.AddWatchAsync(watcher);

        bool watching = await priceWatcherService.IsWatchingAsync(watcher.EventId);

        Assert.True(watching);
    }
}

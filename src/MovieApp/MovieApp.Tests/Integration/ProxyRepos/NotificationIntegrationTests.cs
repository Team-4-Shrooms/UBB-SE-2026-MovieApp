using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MovieApp.DataLayer;
using MovieApp.DataLayer.Models;
using MovieApp.Proxy.Services;

namespace MovieApp.Tests.Integration.ProxyRepos;

public sealed class NotificationIntegrationTests
{

    [Fact]
    public async Task GetNotificationsByUser_SeededUser_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync(
            $"api/notifications/{ProxyRepoSeedIds.SeededUserId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetNotificationsByUser_SeededUser_ReturnsNonNullList()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        NotificationProxyService notificationService = new(testContext.ApiClient);

        IReadOnlyList<Notification> notifications =
            await notificationService.GetNotificationsByUserAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.NotNull(notifications);
    }


    [Fact]
    public async Task GetUnreadNotifications_SeededUser_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.GetAsync(
            $"api/notifications/{ProxyRepoSeedIds.SeededUserId}/unread");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUnreadNotifications_FreshDatabase_CountIsNonNegative()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        NotificationProxyService notificationService = new(testContext.ApiClient);

        IReadOnlyList<Notification> unread =
            await notificationService.GetUnreadNotificationsAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.True(unread.Count >= 0);
    }


    [Fact]
    public async Task MarkAllRead_SeededUser_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        HttpResponseMessage response = await testContext.HttpClient.PostAsJsonAsync(
            $"api/notifications/read-all/{ProxyRepoSeedIds.SeededUserId}",
            new { });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task MarkAllRead_AfterAddingUnreadNotification_UnreadCountBecomesZero()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        NotificationProxyService notificationService = new(testContext.ApiClient);

        using (IServiceScope scope = testContext.Factory.Services.CreateScope())
        {
            AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Notifications.Add(new Notification
            {
                UserId = ProxyRepoSeedIds.SeededUserId,
                EventId = 1,
                Type = "PriceDrop",
                Message = "Integration test notification",
                State = NotificationState.Unread,
                CreatedAt = DateTime.UtcNow,
            });
            await context.SaveChangesAsync();
        }

        await notificationService.MarkAllReadAsync(ProxyRepoSeedIds.SeededUserId);

        IReadOnlyList<Notification> unread =
            await notificationService.GetUnreadNotificationsAsync(ProxyRepoSeedIds.SeededUserId);

        Assert.Empty(unread);
    }

    [Fact]
    public async Task MarkAllRead_ForOneUser_DoesNotClearOtherUsersNotifications()
    {
        using ProxyRepoIntegrationTestContext testContext = new();
        NotificationProxyService notificationService = new(testContext.ApiClient);
        const int otherUserId = 2;

        using (IServiceScope scope = testContext.Factory.Services.CreateScope())
        {
            AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Notifications.AddRange(
                new Notification
                {
                    UserId = ProxyRepoSeedIds.SeededUserId,
                    EventId = 1,
                    Type = "PriceDrop",
                    Message = "Notification for user 1",
                    State = NotificationState.Unread,
                    CreatedAt = DateTime.UtcNow,
                },
                new Notification
                {
                    UserId = otherUserId,
                    EventId = 1,
                    Type = "PriceDrop",
                    Message = "Notification for user 2",
                    State = NotificationState.Unread,
                    CreatedAt = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();
        }

        await notificationService.MarkAllReadAsync(ProxyRepoSeedIds.SeededUserId);

        using (IServiceScope scope = testContext.Factory.Services.CreateScope())
        {
            AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            int otherUnreadCount = context.Notifications
                .Count(n => n.UserId == otherUserId && n.State == NotificationState.Unread);
            Assert.True(otherUnreadCount > 0);
        }
    }


    [Fact]
    public async Task MarkRead_ExistingNotification_ReturnsHttp200()
    {
        using ProxyRepoIntegrationTestContext testContext = new();

        int notificationId;
        using (IServiceScope scope = testContext.Factory.Services.CreateScope())
        {
            AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Notification notification = new()
            {
                UserId = ProxyRepoSeedIds.SeededUserId,
                EventId = 1,
                Type = "PriceDrop",
                Message = "To be read",
                State = NotificationState.Unread,
                CreatedAt = DateTime.UtcNow,
            };
            context.Notifications.Add(notification);
            await context.SaveChangesAsync();
            notificationId = notification.Id;
        }

        HttpResponseMessage response = await testContext.HttpClient.PostAsJsonAsync(
            $"api/notifications/{notificationId}/read",
            new { });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

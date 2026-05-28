using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebApi.Endpoints;


namespace MovieApp.Tests.Controllers;

public sealed class NotificationsControllerTests
{

    private sealed class FakeNotificationService : INotificationService
    {
        public List<Notification> UserNotifications { get; init; } = new();
        public List<Notification> UnreadNotifications { get; init; } = new();
        public bool MarkAllReadCalled { get; private set; }
        public int MarkAllReadCalledForUserId { get; private set; }

        public Task<IReadOnlyList<Notification>> GetNotificationsByUserAsync(int userId, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<Notification>>(UserNotifications);

        public Task<IReadOnlyList<Notification>> GetUnreadNotificationsAsync(int userId, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<Notification>>(UnreadNotifications);

        public Task MarkAllReadAsync(int userId, CancellationToken ct = default)
        {
            MarkAllReadCalled = true;
            MarkAllReadCalledForUserId = userId;
            UnreadNotifications.Clear();
            return Task.CompletedTask;
        }

        public Task MarkReadAsync(int notificationId, CancellationToken ct = default) => Task.CompletedTask;
        public Task MarkAsReadOrRemoveAsync(int notificationId, CancellationToken ct = default) => Task.CompletedTask;
        public Task RemoveNotificationAsync(int notificationId, CancellationToken ct = default) => Task.CompletedTask;
        public Task GeneratePriceDropNotificationAsync(int eventId, string eventTitle, CancellationToken ct = default) => Task.CompletedTask;
        public Task GenerateSeatsAvailableNotificationAsync(int eventId, string eventTitle, CancellationToken ct = default) => Task.CompletedTask;
        public Task NotifyPriceDropAsync(int eventId, decimal oldPrice, decimal newPrice, CancellationToken ct = default) => Task.CompletedTask;
        public Task NotifySeatsAvailableAsync(int eventId, int newCapacity, CancellationToken ct = default) => Task.CompletedTask;
        public Task<int> GetUnreadCountAsync(int userId, CancellationToken ct = default) => Task.FromResult(UnreadNotifications.Count);
    }


    [Fact]
    public async Task MarkAllRead_WhenCalled_ReturnsOkResult()
    {
        FakeNotificationService service = new();
        NotificationsEndpointsController controller = new(service);

        IActionResult result = await controller.MarkAllRead(userId: 1);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task MarkAllRead_WhenCalled_DelegatesMarkAllReadToService()
    {
        FakeNotificationService service = new();
        NotificationsEndpointsController controller = new(service);

        await controller.MarkAllRead(userId: 1);

        Assert.True(service.MarkAllReadCalled);
        Assert.Equal(1, service.MarkAllReadCalledForUserId);
    }

    [Fact]
    public async Task MarkAllRead_ForSpecificUser_PassesCorrectUserIdToService()
    {
        FakeNotificationService service = new();
        NotificationsEndpointsController controller = new(service);

        await controller.MarkAllRead(userId: 42);

        Assert.Equal(42, service.MarkAllReadCalledForUserId);
    }

    [Fact]
    public async Task GetNotificationsByUser_ServiceReturnsEmpty_ReturnsOkResult()
    {
        FakeNotificationService service = new();
        NotificationsEndpointsController controller = new(service);

        IActionResult result = await controller.GetNotificationsByUser(userId: 1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetNotificationsByUser_ServiceReturnsNotifications_ResultIsNonNull()
    {
        FakeNotificationService service = new()
        {
            UserNotifications =
            {
                new Notification { Id = 1, UserId = 1, EventId = 1, Type = "PriceDrop", Message = "Price dropped!" },
            },
        };
        NotificationsEndpointsController controller = new(service);

        IActionResult result = await controller.GetNotificationsByUser(userId: 1);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }


    [Fact]
    public async Task GetUnreadNotifications_ServiceReturnsEmpty_ReturnsOkResult()
    {
        FakeNotificationService service = new();
        NotificationsEndpointsController controller = new(service);

        IActionResult result = await controller.GetUnreadNotifications(userId: 1);

        Assert.IsType<OkObjectResult>(result);
    }


    [Fact]
    public async Task MarkRead_WhenCalled_ReturnsOkResult()
    {
        FakeNotificationService service = new();
        NotificationsEndpointsController controller = new(service);

        IActionResult result = await controller.MarkRead(notificationId: 1);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task MarkReadOrRemove_WhenCalled_ReturnsOkResult()
    {
        FakeNotificationService service = new();
        NotificationsEndpointsController controller = new(service);

        IActionResult result = await controller.MarkReadOrRemove(notificationId: 1);

        Assert.IsType<OkResult>(result);
    }


    [Fact]
    public async Task RemoveNotification_WhenCalled_ReturnsOkResult()
    {
        FakeNotificationService service = new();
        NotificationsEndpointsController controller = new(service);

        IActionResult result = await controller.RemoveNotification(notificationId: 1);

        Assert.IsType<OkResult>(result);
    }
}

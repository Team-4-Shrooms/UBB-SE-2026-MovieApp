using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services;

public class NotificationProxyService : INotificationService
{
    private readonly ApiClient _apiClient;

    public NotificationProxyService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IReadOnlyList<Notification>> GetNotificationsByUserAsync(int userIdentifier, CancellationToken cancellationToken = default)
    {
        var notifications = await _apiClient.GetAsync<List<Notification>>($"api/notifications/{userIdentifier}");
        return notifications ?? new List<Notification>();
    }

    // Kept to satisfy the interface contract; delegates to GetNotificationsByUserAsync
    // since both methods are semantically identical and share the same endpoint.
    public async Task<IReadOnlyList<Notification>> GetNotificationsByUserIdAsync(int userIdentifier, CancellationToken cancellationToken = default)
    {
        return await GetNotificationsByUserAsync(userIdentifier, cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetUnreadAsync(int userIdentifier, CancellationToken cancellationToken = default)
    {
        var unreadNotifications = await _apiClient.GetAsync<List<Notification>>($"api/notifications/{userIdentifier}/unread");
        return unreadNotifications ?? new List<Notification>();
    }

    public async Task RemoveNotificationAsync(int notificationIdentifier, CancellationToken cancellationToken = default)
    {
        await _apiClient.DeleteAsync($"api/notifications/{notificationIdentifier}");
    }

    public async Task MarkReadAsync(int notificationIdentifier, CancellationToken cancellationToken = default)
    {
        await _apiClient.PostAsync($"api/notifications/{notificationIdentifier}/read", new { });
    }

    public async Task MarkAsReadOrRemoveAsync(int notificationIdentifier, CancellationToken cancellationToken = default)
    {
        await _apiClient.PostAsync($"api/notifications/{notificationIdentifier}/mark-read-or-remove", new { });
    }

    public async Task MarkAllReadAsync(int userIdentifier, CancellationToken cancellationToken = default)
    {
        await _apiClient.PostAsync($"api/notifications/read-all/{userIdentifier}", new { });
    }

    public async Task GeneratePriceDropNotificationAsync(int eventIdentifier, string eventTitle, CancellationToken cancellationToken = default)
    {
        await _apiClient.PostAsync(
            "api/notifications/generate/price-drop",
            new { EventId = eventIdentifier, EventTitle = eventTitle });
    }

    public async Task GenerateSeatsAvailableNotificationAsync(int eventIdentifier, string eventTitle, CancellationToken cancellationToken = default)
    {
        await _apiClient.PostAsync(
            "api/notifications/generate/seats-available",
            new { EventId = eventIdentifier, EventTitle = eventTitle });
    }

    public async Task NotifyPriceDropAsync(int eventIdentifier, decimal oldPrice, decimal newPrice, CancellationToken cancellationToken = default)
    {
        await _apiClient.PostAsync(
            "api/notifications/notify/price-drop",
            new { EventId = eventIdentifier, OldPrice = oldPrice, NewPrice = newPrice });
    }

    public async Task NotifySeatsAvailableAsync(int eventIdentifier, int newCapacity, CancellationToken cancellationToken = default)
    {
        await _apiClient.PostAsync(
            "api/notifications/notify/seats-available",
            new { EventId = eventIdentifier, NewCapacity = newCapacity });
    }
}

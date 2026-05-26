using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services
{
    public sealed class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IFavoriteEventRepository _favoriteEventRepository;
        private readonly IEventRepository _eventRepository;

        public NotificationService(
            INotificationRepository notificationRepository,
            IFavoriteEventRepository favoriteEventRepository,
            IEventRepository eventRepository)
        {
            _notificationRepository = notificationRepository;
            _favoriteEventRepository = favoriteEventRepository;
            _eventRepository = eventRepository;
        }

        public async Task GeneratePriceDropNotificationAsync(int eventIdentifier, string eventTitle, CancellationToken cancellationToken = default)
        {
            await GenerateNotificationForFavoritesAsync(
                eventIdentifier,
                "PRICE_DROP",
                $"The ticket price for '{eventTitle}' has dropped!",
                cancellationToken);
        }

        public async Task GenerateSeatsAvailableNotificationAsync(int eventIdentifier, string eventTitle, CancellationToken cancellationToken = default)
        {
            await GenerateNotificationForFavoritesAsync(
                eventIdentifier,
                "SEATS_AVAILABLE",
                $"Seats are now available for '{eventTitle}'!",
                cancellationToken);
        }

        public Task<IReadOnlyList<Notification>> GetNotificationsByUserAsync(int userIdentifier, CancellationToken cancellationToken = default)
        {
            return _notificationRepository.FindByUserAsync(userIdentifier, cancellationToken);
        }

        public Task RemoveNotificationAsync(int notificationIdentifier, CancellationToken cancellationToken = default)
        {
            return _notificationRepository.RemoveAsync(notificationIdentifier, cancellationToken);
        }

        public async Task NotifyPriceDropAsync(int eventIdentifier, decimal oldPrice, decimal newPrice, CancellationToken cancellationToken = default)
        {
            if (newPrice >= oldPrice)
            {
                return;
            }

            MovieEvent? eventDetails = await _eventRepository.GetEventByIdAsync(eventIdentifier);
            if (eventDetails is null)
            {
                return;
            }

            IReadOnlyList<FavoriteEvent> favorites = await _favoriteEventRepository.FindByEventAsync(eventIdentifier, cancellationToken);
            foreach (FavoriteEvent favoriteEventLink in favorites)
            {
                IReadOnlyList<Notification> notifications = await _notificationRepository.FindByUserAsync(favoriteEventLink.UserId, cancellationToken);
                bool alreadyNotified = notifications.Any(notification =>
                    notification.EventId == eventIdentifier
                    && notification.Type == "PriceDrop"
                    && notification.State == NotificationState.Unread);

                if (alreadyNotified)
                {
                    continue;
                }

                await _notificationRepository.AddAsync(
                    new Notification
                    {
                        Id = 0,
                        UserId = favoriteEventLink.UserId,
                        EventId = eventIdentifier,
                        Type = "PriceDrop",
                        Message = $"The price for '{eventDetails.Movie?.Title ?? "event"}' has dropped to {newPrice:C}!",
                        State = NotificationState.Unread,
                        CreatedAt = DateTime.UtcNow,
                    },
                    cancellationToken);
            }
        }

        public async Task NotifySeatsAvailableAsync(int eventIdentifier, int newCapacity, CancellationToken cancellationToken = default)
        {
            MovieEvent? eventDetails = await _eventRepository.GetEventByIdAsync(eventIdentifier);
            if (eventDetails is null)
            {
                return;
            }

            IReadOnlyList<FavoriteEvent> favorites = await _favoriteEventRepository.FindByEventAsync(eventIdentifier, cancellationToken);
            foreach (FavoriteEvent favoriteEventLink in favorites)
            {
                IReadOnlyList<Notification> notifications = await _notificationRepository.FindByUserAsync(favoriteEventLink.UserId, cancellationToken);
                bool alreadyNotified = notifications.Any(notification =>
                    notification.EventId == eventIdentifier
                    && notification.Type == "SeatsAvailable"
                    && notification.State == NotificationState.Unread);

                if (alreadyNotified)
                {
                    continue;
                }

                await _notificationRepository.AddAsync(
                    new Notification
                    {
                        Id = 0,
                        UserId = favoriteEventLink.UserId,
                        EventId = eventIdentifier,
                        Type = "SeatsAvailable",
                        Message = $"Seats are now available for '{eventDetails.Movie?.Title ?? "event"}'!",
                        State = NotificationState.Unread,
                        CreatedAt = DateTime.UtcNow,
                    },
                    cancellationToken);
            }
        }

        public Task MarkAsReadOrRemoveAsync(int notificationIdentifier, CancellationToken cancellationToken = default)
        {
            return _notificationRepository.MarkReadAsync(notificationIdentifier, cancellationToken);
        }

        public Task<IReadOnlyList<Notification>> GetUnreadNotificationsAsync(int userIdentifier, CancellationToken cancellationToken = default)
        {
            return _notificationRepository.GetUnreadByUserAsync(userIdentifier, cancellationToken);
        }

        public Task MarkReadAsync(int notificationIdentifier, CancellationToken cancellationToken = default)
        {
            return _notificationRepository.MarkReadAsync(notificationIdentifier, cancellationToken);
        }

        public Task MarkAllReadAsync(int userIdentifier, CancellationToken cancellationToken = default)
        {
            return _notificationRepository.MarkAllReadAsync(userIdentifier, cancellationToken);
        }

        public Task<int> GetUnreadCountAsync(int userIdentifier, CancellationToken cancellationToken = default)
        {
            return _notificationRepository.GetUnreadCountAsync(userIdentifier, cancellationToken);
        }

        private async Task GenerateNotificationForFavoritesAsync(int eventIdentifier, string type, string message, CancellationToken cancellationToken)
        {
            IReadOnlyList<int> favoritedUserIds = await _favoriteEventRepository.GetUsersByFavoriteEventAsync(eventIdentifier, cancellationToken);

            foreach (int userIdentifier in favoritedUserIds)
            {
                IReadOnlyList<Notification> notifications = await _notificationRepository.FindByUserAsync(userIdentifier, cancellationToken);
                Notification? recentNotification = notifications.FirstOrDefault(notification =>
                    notification.EventId == eventIdentifier && notification.Type == type);

                if (recentNotification is not null && recentNotification.State == NotificationState.Unread)
                {
                    continue;
                }

                await _notificationRepository.AddAsync(
                    new Notification
                    {
                        Id = 0,
                        UserId = userIdentifier,
                        EventId = eventIdentifier,
                        Type = type,
                        Message = message,
                        State = NotificationState.Unread,
                        CreatedAt = DateTime.UtcNow,
                    },
                    cancellationToken);
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Interfaces;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Repositories
{
    public sealed class NotificationRepository : INotificationRepository
    {
        private readonly IMovieAppDbContext _context;

        public NotificationRepository(IMovieAppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveAsync(int notificationIdentifier, CancellationToken cancellationToken = default)
        {
            Notification? notification = await _context.Notifications
                .FindAsync(new object[] { notificationIdentifier }, cancellationToken);

            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IReadOnlyList<Notification>> FindByUserAsync(int userIdentifier, CancellationToken cancellationToken = default)
        {
            List<Notification> notifications = await _context.Notifications
                .Where(notification => notification.UserId == userIdentifier)
                .OrderByDescending(notification => notification.CreatedAt)
                .ToListAsync(cancellationToken);
            return notifications;
        }

        public async Task<IReadOnlyList<Notification>> GetUnreadByUserAsync(int userIdentifier, CancellationToken cancellationToken = default)
        {
            List<Notification> unreadNotifications = await _context.Notifications
                .Where(notification => notification.UserId == userIdentifier && notification.State == NotificationState.Unread)
                .OrderByDescending(notification => notification.CreatedAt)
                .ToListAsync(cancellationToken);
            return unreadNotifications;
        }

        public async Task MarkReadAsync(int notificationIdentifier, CancellationToken cancellationToken = default)
        {
            Notification? notification = await _context.Notifications
                .FindAsync(new object[] { notificationIdentifier }, cancellationToken);

            if (notification != null)
            {
                notification.State = NotificationState.Read;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task MarkAllReadAsync(int userIdentifier, CancellationToken cancellationToken = default)
        {
            List<Notification> unreadNotifications = await _context.Notifications
                .Where(notification => notification.UserId == userIdentifier && notification.State == NotificationState.Unread)
                .ToListAsync(cancellationToken);

            foreach (Notification notification in unreadNotifications)
            {
                notification.State = NotificationState.Read;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> GetUnreadCountAsync(int userIdentifier, CancellationToken cancellationToken = default)
        {
            int count = await _context.Notifications
                .CountAsync(notification => notification.UserId == userIdentifier && notification.State == NotificationState.Unread, cancellationToken);
            return count;
        }
    }
}

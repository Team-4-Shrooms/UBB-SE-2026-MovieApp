using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using MovieApp.DataLayer.Models;
using MovieApp.Features.Shared.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Features.Notification.ViewModels
{
    public class NotificationViewModel : INotifyPropertyChanged
    {
        private readonly INotificationService _service;
        private int _unreadCount;

        public ObservableCollection<Notification> Notifications { get; } = new ObservableCollection<Notification>();

        public int UnreadCount
        {
            get => _unreadCount;
            private set
            {
                _unreadCount = value;
                OnPropertyChanged(nameof(UnreadCount));
                OnPropertyChanged(nameof(BadgeVisibility));
            }
        }

        public Visibility BadgeVisibility =>
            _unreadCount > 0 ? Visibility.Visible : Visibility.Collapsed;

        public IAsyncRelayCommand LoadNotificationsCommand { get; }

        public IAsyncRelayCommand<int> MarkReadCommand { get; }

        public IAsyncRelayCommand MarkAllReadCommand { get; }

        public NotificationViewModel(INotificationService service)
        {
            _service = service;
            LoadNotificationsCommand = new AsyncRelayCommand(LoadNotificationsAsync);
            MarkReadCommand = new AsyncRelayCommand<int>(MarkReadAsync);
            MarkAllReadCommand = new AsyncRelayCommand(MarkAllReadAsync);
        }

        private async Task LoadNotificationsAsync()
        {
            IReadOnlyList<Notification> notifications =
                await _service.GetNotificationsByUserAsync(SessionManager.CurrentUserID);
            Notifications.Clear();
            foreach (Notification notification in notifications)
            {
                Notifications.Add(notification);
            }
            RefreshUnreadCount();
        }

        private async Task MarkReadAsync(int notificationId)
        {
            await _service.MarkReadAsync(notificationId);
            Notification? item = Notifications.FirstOrDefault(n => n.Id == notificationId);
            if (item != null)
            {
                Notifications.Remove(item);
            }
            RefreshUnreadCount();
        }

        private async Task MarkAllReadAsync()
        {
            await _service.MarkAllReadAsync(SessionManager.CurrentUserID);
            Notifications.Clear();
            UnreadCount = 0;
        }

        private void RefreshUnreadCount()
        {
            UnreadCount = Notifications.Count(n => n.State == NotificationState.Unread);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MovieApp.DataLayer.Models;
using MovieApp.Features.Notification.ViewModels;

namespace MovieApp.Features.Notification.Views
{
    public sealed partial class NotificationPage : Page
    {
        public NotificationViewModel ViewModel { get; } =
            App.Services.GetRequiredService<NotificationViewModel>();

        public NotificationPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadNotificationsCommand.ExecuteAsync(null);
        }

        private async void MarkRead_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Notification notification)
            {
                await ViewModel.MarkReadCommand.ExecuteAsync(notification.Id);
            }
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MovieApp.Features.PriceWatcher.ViewModels;

namespace MovieApp.Features.PriceWatcher.Views
{
    public sealed partial class PriceWatcherPage : Page
    {
        public PriceWatcherViewModel ViewModel { get; } =
            App.Services.GetRequiredService<PriceWatcherViewModel>();

        public PriceWatcherPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadWatchersAsync();
        }

        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int eventId)
            {
                await ViewModel.RemoveWatchAsync(eventId);
            }
        }
    }
}

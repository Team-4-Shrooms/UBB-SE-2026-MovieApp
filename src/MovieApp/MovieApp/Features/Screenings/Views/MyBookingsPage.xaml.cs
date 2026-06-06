using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MovieApp.Features.Screenings.ViewModels;

namespace MovieApp.Features.Screenings.Views;

public sealed partial class MyBookingsPage : Page
{
    public MyBookingsViewModel ViewModel { get; }

    public MyBookingsPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<MyBookingsViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadAsync();
    }

    private void BackButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
            Frame.GoBack();
    }

    private async void CancelBooking_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int bookingId)
            await ViewModel.CancelBookingAsync(bookingId);
    }
}

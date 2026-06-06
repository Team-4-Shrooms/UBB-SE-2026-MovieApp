using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using MovieApp.Features.Screenings.ViewModels;

namespace MovieApp.Features.Screenings.Views;

public sealed partial class ScreeningPage : Page
{
    public ScreeningViewModel ViewModel { get; }

    public ScreeningPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<ScreeningViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadScreeningsAsync();
    }

    private void MyBookings_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Frame.Navigate(typeof(MyBookingsPage));
    }
}

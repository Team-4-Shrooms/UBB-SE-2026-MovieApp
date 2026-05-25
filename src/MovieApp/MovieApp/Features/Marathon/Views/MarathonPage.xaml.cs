namespace MovieApp.Features.Marathon.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MovieApp.Features.Marathon.ViewModels;

public sealed partial class MarathonPage : Page
{
    public MarathonViewModel ViewModel { get; } =
        App.Services.GetRequiredService<MarathonViewModel>();

    public MarathonPage()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadMarathonsCommand.ExecuteAsync(null);
    }
}

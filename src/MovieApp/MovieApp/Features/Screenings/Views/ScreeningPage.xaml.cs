using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using MovieApp.Features.Screenings.ViewModels;

namespace MovieApp.Features.Screenings.Views
{
    public sealed partial class ScreeningPage : Page
    {
        public ScreeningViewModel ViewModel { get; }

        public ScreeningPage()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<ScreeningViewModel>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs newEvent)
        {
            base.OnNavigatedTo(newEvent);
            await ViewModel.LoadScreeningsAsync();
        }
    }
}

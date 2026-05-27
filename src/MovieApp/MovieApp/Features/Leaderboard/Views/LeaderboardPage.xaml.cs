using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using MovieApp.Features.Leaderboard.ViewModels;

namespace MovieApp.Features.Leaderboard.Views
{
    public sealed partial class LeaderboardPage : Page
    {
        public LeaderboardViewModel ViewModel { get; }

        public LeaderboardPage()
        {
            ViewModel = App.Services.GetRequiredService<LeaderboardViewModel>();
            this.InitializeComponent();
        }
    }
}

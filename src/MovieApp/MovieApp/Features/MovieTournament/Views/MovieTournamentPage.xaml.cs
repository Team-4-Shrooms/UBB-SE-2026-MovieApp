using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MovieApp.Logic.Features.MovieTournament;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace MovieApp.Features.MovieTournament.Views
{
    /// <summary>
    /// Host page for the movie tournament feature.
    /// On load, navigates the inner frame to the appropriate sub-page
    /// based on the current tournament state.
    /// </summary>
    public sealed partial class MovieTournamentPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovieTournamentPage"/> class
        /// and registers the loaded event handler.
        /// </summary>
        public MovieTournamentPage()
        {
            this.InitializeComponent();
            this.Loaded += this.OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            ITournamentLogicService tournamentLogicService = Ioc.Default.GetRequiredService<ITournamentLogicService>();
            int userId = 1;

            MatchPair? currentMatch = await tournamentLogicService.GetCurrentMatchAsync(userId);
            if (currentMatch != null)
            {
                this.TournamentFrame.Navigate(typeof(TournamentMatchPage));
            }
            else if (await tournamentLogicService.IsTournamentCompleteAsync(userId))
            {
                this.TournamentFrame.Navigate(typeof(TournamentWinnerPage));
            }
            else
            {
                this.TournamentFrame.Navigate(typeof(TournamentSetupPage));
            }
        }
    }
}

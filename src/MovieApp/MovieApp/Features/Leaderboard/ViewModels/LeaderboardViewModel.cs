using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Features.Leaderboard.ViewModels
{
    /// <summary>
    /// View model for the leaderboard page. Fetches the global ranking from the
    /// badge proxy service and exposes it sorted by total score.
    /// </summary>
    public partial class LeaderboardViewModel : ObservableObject
    {
        private readonly IBadgeService _badgeService;

        [ObservableProperty]
        private ObservableCollection<UserStats> _ranking = new();

        public LeaderboardViewModel(IBadgeService badgeService)
        {
            _badgeService = badgeService;
            _ = LoadAsync();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            IList<UserStats> entries = await _badgeService.GetLeaderboardAsync();

            IEnumerable<UserStats> ordered = entries
                .OrderByDescending(stats => stats.TotalPoints)
                .ThenByDescending(stats => stats.WeeklyScore);

            Ranking.Clear();
            foreach (UserStats stats in ordered)
            {
                Ranking.Add(stats);
            }
        }
    }
}

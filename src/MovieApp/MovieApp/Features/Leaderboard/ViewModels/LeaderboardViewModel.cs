using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using Windows.UI;

namespace MovieApp.Features.Leaderboard.ViewModels
{
    public sealed class RankedEntry
    {
        public int Rank { get; init; }
        public UserStats Stats { get; init; } = null!;

        public string RankDisplay => Rank.ToString();
        public bool IsTopThree => Rank <= 3;

        public SolidColorBrush RankForeground => Rank switch
        {
            1 => new SolidColorBrush(Color.FromArgb(255, 201, 165, 90)),   // Gold
            2 => new SolidColorBrush(Color.FromArgb(255, 192, 192, 192)),  // Silver
            3 => new SolidColorBrush(Color.FromArgb(255, 205, 127, 50)),   // Bronze
            _ => new SolidColorBrush(Color.FromArgb(255, 138, 128, 116))   // TextSecondary
        };

        public SolidColorBrush RankBackground => Rank switch
        {
            1 => new SolidColorBrush(Color.FromArgb(40,  201, 165, 90)),
            2 => new SolidColorBrush(Color.FromArgb(30,  192, 192, 192)),
            3 => new SolidColorBrush(Color.FromArgb(30,  205, 127, 50)),
            _ => new SolidColorBrush(Color.FromArgb(255, 37, 35, 32))
        };

        public SolidColorBrush PointsForeground => Rank <= 3
            ? new SolidColorBrush(Color.FromArgb(255, 201, 165, 90))
            : new SolidColorBrush(Color.FromArgb(255, 245, 240, 232));
    }

    public partial class LeaderboardViewModel : ObservableObject
    {
        private readonly IBadgeService _badgeService;

        [ObservableProperty]
        private ObservableCollection<RankedEntry> _ranking = new();

        public LeaderboardViewModel(IBadgeService badgeService)
        {
            _badgeService = badgeService;
            _ = LoadAsync();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            IList<UserStats> entries = await _badgeService.GetLeaderboardAsync();

            var ordered = entries
                .OrderByDescending(s => s.TotalPoints)
                .ThenByDescending(s => s.WeeklyScore)
                .ToList();

            Ranking.Clear();
            for (int i = 0; i < ordered.Count; i++)
                Ranking.Add(new RankedEntry { Rank = i + 1, Stats = ordered[i] });
        }
    }
}

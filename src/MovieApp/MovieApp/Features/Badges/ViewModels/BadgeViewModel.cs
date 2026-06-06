using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using Windows.UI;

namespace MovieApp.Features.Badges.ViewModels
{
    public sealed class BadgeCardViewModel
    {
        public Badge Badge { get; }
        public bool IsEarned { get; }
        public string StarIcon => IsEarned ? "★" : "☆";
        public string EarnedLabel => IsEarned ? "EARNED" : "";

        public SolidColorBrush StarForeground => IsEarned
            ? new SolidColorBrush(Color.FromArgb(255, 201, 165, 90))
            : new SolidColorBrush(Color.FromArgb(255, 138, 128, 116));

        public SolidColorBrush CardBorderBrush => IsEarned
            ? new SolidColorBrush(Color.FromArgb(255, 201, 165, 90))
            : new SolidColorBrush(Color.FromArgb(255, 46, 43, 38));

        public SolidColorBrush CardBackground => IsEarned
            ? new SolidColorBrush(Color.FromArgb(255, 30, 27, 22))
            : new SolidColorBrush(Color.FromArgb(255, 28, 26, 23));

        public BadgeCardViewModel(Badge badge, bool isEarned)
        {
            Badge = badge;
            IsEarned = isEarned;
        }
    }

    public partial class BadgeViewModel : ObservableObject
    {
        private readonly IBadgeService _badgeService;
        private readonly IUserStatsService _userStatsService;
        private readonly ICurrentUserService _currentUserService;

        [ObservableProperty]
        private ObservableCollection<BadgeCardViewModel> _allBadgeCards = new();

        [ObservableProperty]
        private UserStats? _stats;

        [ObservableProperty]
        private string _username = "User";

        [ObservableProperty]
        private string _avatarLetter = "U";

        [ObservableProperty]
        private int _collectionPercent;

        [ObservableProperty]
        private int _earnedCount;

        [ObservableProperty]
        private int _totalBadgeCount;

        public BadgeViewModel(
            IBadgeService badgeService,
            IUserStatsService userStatsService,
            ICurrentUserService currentUserService)
        {
            _badgeService = badgeService;
            _userStatsService = userStatsService;
            _currentUserService = currentUserService;
            _ = LoadAsync();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            int userId = _currentUserService.UserId;

            var earned = await _badgeService.GetUserBadgesAsync(userId);
            var all    = await _badgeService.GetAllBadgesAsync();
            Stats      = await _userStatsService.GetByUserIdAsync(userId);

            var earnedIds = earned.Select(b => b.Badge?.BadgeId ?? 0).ToHashSet();

            EarnedCount       = earned.Count;
            TotalBadgeCount   = all.Count;
            CollectionPercent = all.Count > 0
                ? (int)System.Math.Round((double)earned.Count / all.Count * 100)
                : 0;

            Username    = Stats?.User?.Username ?? $"User #{userId}";
            AvatarLetter = Username.Length > 0 ? Username[0].ToString().ToUpper() : "?";

            AllBadgeCards.Clear();
            foreach (var badge in all.OrderBy(b => b.Name))
                AllBadgeCards.Add(new BadgeCardViewModel(badge, earnedIds.Contains(badge.BadgeId)));
        }
    }
}

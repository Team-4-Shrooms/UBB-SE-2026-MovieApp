using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Features.Badges.ViewModels
{
    /// <summary>
    /// View model for the badge profile page. Loads the logged-in user's earned badges
    /// and their stats from the proxy services.
    /// </summary>
    public partial class BadgeViewModel : ObservableObject
    {
        private readonly IBadgeService _badgeService;
        private readonly IUserStatsService _userStatsService;
        private readonly ICurrentUserService _currentUserService;

        [ObservableProperty]
        private ObservableCollection<UserBadge> _badges = new();

        [ObservableProperty]
        private UserStats? _stats;

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

            System.Collections.Generic.List<UserBadge> userBadges =
                await _badgeService.GetUserBadgesAsync(userId);

            Badges.Clear();
            foreach (UserBadge userBadge in userBadges)
            {
                Badges.Add(userBadge);
            }

            Stats = await _userStatsService.GetByUserIdAsync(userId);
        }
    }
}

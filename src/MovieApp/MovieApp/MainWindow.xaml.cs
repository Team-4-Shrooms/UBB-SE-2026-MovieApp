using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MovieApp.Features.Events.Views;
using MovieApp.Features.Inventory.Views;
using MovieApp.Features.Marketplace.Views;
using MovieApp.Features.Marathon.Views;
using MovieApp.Features.MovieCatalog.Views;
using MovieApp.Features.MovieSwipe.Views;
using MovieApp.Features.MovieTournament.Views;
using MovieApp.Features.PersonalityMatch.Views;
using MovieApp.Features.ReelsEditing.Views;
using MovieApp.Features.ReelsFeed.Views;
using MovieApp.Features.ReelsUpload.Views;
using MovieApp.Features.TrailerScraping.Views;
using MovieApp.Features.Notification.ViewModels;
using MovieApp.Features.Notification.Views;
using MovieApp.Features.SlotMachine.Views;
using MovieApp.Features.Wallet.Views;
using MovieApp.Features.Ambassadors.Views;
using MovieApp.Features.TriviaWheel.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MovieApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        public NotificationViewModel NotificationViewModel { get; } =
            App.Services.GetRequiredService<NotificationViewModel>();

        private static readonly Dictionary<string, Type> PageMap = new()
        {

            ["MovieCatalog"] = typeof(MovieCatalogPage),
            ["Events"] = typeof(MovieEventsPage),
            ["Inventory"] = typeof(InventoryPage),
            ["Marketplace"] = typeof(MarketplacePage),
            ["Wallet"] = typeof(WalletPage),
            ["ReelsUpload"] = typeof(ReelsUploadPage),
            ["TrailerScraping"] = typeof(TrailerScrapingPage),
            ["ReelsEditing"] = typeof(ReelsEditingPage),
            ["MovieSwipe"] = typeof(MovieSwipeView),
            ["MovieTournament"] = typeof(MovieTournamentPage),
            ["PersonalityMatch"] = typeof(PersonalityMatchPage),
            ["ReelsFeed"] = typeof(ReelsFeedPage),
            ["Notification"] = typeof(NotificationPage),
            ["SlotMachine"] = typeof(SlotMachinePage),
            ["Marathon"] = typeof(MarathonPage),
            ["Ambassadors"] = typeof(AmbassadorPage),
            ["TriviaWheel"] = typeof(TriviaWheelPage),
        };

        public MainWindow()
        {
            InitializeComponent();
            ContentFrame.Navigate(typeof(Page));

        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item &&
                item.Tag is string tag &&
                PageMap.TryGetValue(tag, out Type? pageType))
            {
                ContentFrame.Navigate(pageType);
            }
        }
    }
}

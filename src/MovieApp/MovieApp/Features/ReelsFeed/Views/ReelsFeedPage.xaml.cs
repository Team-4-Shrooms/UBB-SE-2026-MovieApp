using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MovieApp.DataLayer.Models;
using MovieApp.Features.ReelsFeed.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace MovieApp.Features.ReelsFeed.Views
{
    public sealed partial class ReelsFeedPage : Page
    {
        public ReelsFeedViewModel ViewModel { get; }

        public ReelsFeedPage()
        {
            this.ViewModel = Ioc.Default.GetRequiredService<ReelsFeedViewModel>();
            this.InitializeComponent();
            this.Loaded   += this.ReelsFeedPage_Loaded;
            this.Unloaded += this.ReelsFeedPage_Unloaded;
        }

        private void ReelsFeedPage_Unloaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel.OnNavigatingAway();

            for (int i = 0; i < this.ViewModel.ReelQueue.Count; i++)
            {
                var container = this.FeedFlipView.ContainerFromIndex(i) as DependencyObject;
                if (container == null) continue;
                this.FindVisualChild<ReelItemView>(container)?.DisposeMediaPlayer();
            }
        }

        private async void ReelsFeedPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.ReelQueue.Count == 0)
            {
                await this.ViewModel.LoadFeedAsync();
                // Low-priority so FlipView realizes its first container before we play into it
                this.DispatcherQueue.TryEnqueue(
                    Microsoft.UI.Dispatching.DispatcherQueuePriority.Low,
                    this.TriggerPlaybackForCurrent);
            }
        }

        private async void RetryButton_Click(object sender, RoutedEventArgs e)
            => await this.ViewModel.LoadFeedAsync();

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems.First() is Reel selectedReel)
                this.ViewModel.ScrollNext(selectedReel);

            this.DispatcherQueue.TryEnqueue(
                Microsoft.UI.Dispatching.DispatcherQueuePriority.Low,
                this.TriggerPlaybackForCurrent);
        }

        private void TriggerPlaybackForCurrent()
        {
            if (this.FeedFlipView == null || this.ViewModel.ReelQueue.Count == 0)
                return;

            int selected = this.FeedFlipView.SelectedIndex;

            for (int i = 0; i < this.ViewModel.ReelQueue.Count; i++)
            {
                var container = this.FeedFlipView.ContainerFromIndex(i) as DependencyObject;
                if (container == null) continue;

                var view = this.FindVisualChild<ReelItemView>(container);
                if (view == null) continue;

                if (i == selected)
                {
                    var reel = this.ViewModel.ReelQueue[i];
                    view.SetPlaybackItem(reel.VideoUrl, this.ViewModel.BuildPlaybackItem(reel.VideoUrl));
                    view.PlayVideo();
                }
                else
                {
                    view.PauseVideo();
                }
            }
        }

        private T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typed) return typed;
                var nested = this.FindVisualChild<T>(child);
                if (nested != null) return nested;
            }
            return null;
        }
    }
}

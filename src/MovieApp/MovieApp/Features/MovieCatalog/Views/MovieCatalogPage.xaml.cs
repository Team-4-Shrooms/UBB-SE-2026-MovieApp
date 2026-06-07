using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Features.Marketplace.ViewModels;
using MovieApp.Features.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using MovieApp.WebApi.Data;

namespace MovieApp.Features.MovieCatalog.Views
{
    public sealed class MovieCatalogItem
    {
        public Movie Movie { get; }
        public int Id => Movie.Id;
        public string Title => Movie.Title;
        public string? ImageUrl => Movie.PosterUrl;
        public decimal Rating => Movie.Rating;
        public int ReviewCount { get; }

        public string RatingAndReviewCountText => $"Ratings ({ReviewCount}): {Rating:0.#} / 5";
        public string GenreYearTag => string.IsNullOrEmpty(Movie.PrimaryGenre)
            ? $"{Movie.ReleaseYear}"
            : $"{Movie.PrimaryGenre} · {Movie.ReleaseYear}";

        public bool IsOnSale => Movie.HasActiveSale;

        public string OriginalPriceText => $"€{Movie.Price:F2}";
        public string CurrentPriceText => $"€{Movie.EffectivePrice:F2}";

        public Microsoft.UI.Xaml.Visibility SaleVisibility => IsOnSale
            ? Microsoft.UI.Xaml.Visibility.Visible
            : Microsoft.UI.Xaml.Visibility.Collapsed;

        public Microsoft.UI.Xaml.Media.SolidColorBrush PriceColor =>
            new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 201, 165, 90));

        public MovieCatalogItem(Movie movie, int reviewCount)
        {
            Movie = movie;
            ReviewCount = reviewCount;
        }
    }

    public sealed partial class MovieCatalogPage : Page
    {
        private readonly IMovieRepository _movieRepo = App.Services.GetRequiredService<IMovieRepository>();
        private readonly IActiveSalesRepository _salesRepo = App.Services.GetRequiredService<IActiveSalesRepository>();
        private readonly IReviewRepository _reviewRepo = App.Services.GetRequiredService<IReviewRepository>();
        private readonly AppDbContext _context = App.Services.GetRequiredService<AppDbContext>();

        private List<Movie> _sourceMovies = new();
        private Dictionary<int, int> _reviewCountByMovieId = new();

        public FlashSaleViewModel FlashSaleVM { get; } = App.Services.GetRequiredService<FlashSaleViewModel>();
        private bool _showOnlySales;
        private FlashSaleViewModel? _flashSaleVm;

        public MovieCatalogPage()
        {
            InitializeComponent();
            GenreFilter.Items.Add("All genres");
            GenreFilter.SelectedIndex = 0;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is MovieCatalogNavArgs args)
                _showOnlySales = args.ShowOnlySales;

            await LoadDiscountedMoviesAsync();

            SortCombo.SelectedIndex = 0;
            ApplyFilterAndSort();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs @event)
        {
            base.OnNavigatedFrom(@event);
            if (_flashSaleVm != null)
                _flashSaleVm.PropertyChanged -= FlashSaleVm_PropertyChanged!;
        }

        private void FilterButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
            => ApplyFilterAndSort();

        private void ApplyFilterAndSort()
        {
            string query = (SearchBox.Text ?? "").Trim().ToLower();
            string selectedGenre = (GenreFilter.SelectedItem as string) ?? "All genres";
            double minRating = double.IsNaN(MinRatingBox.Value) ? 0 : MinRatingBox.Value;

            IEnumerable<Movie> list = _sourceMovies;

            if (!string.IsNullOrEmpty(query))
                list = list.Where(m => m.Title.ToLower().Contains(query));

            if (selectedGenre != "All genres")
                list = list.Where(m => m.PrimaryGenre == selectedGenre);

            if (minRating > 0)
                list = list.Where(m => (double)m.Rating >= minRating);

            list = SortCombo.SelectedIndex switch
            {
                1 => list.OrderByDescending(m => m.EffectivePrice),
                2 => list.OrderByDescending(m => m.Rating),
                3 => list.OrderBy(m => m.Rating),
                _ => list.OrderBy(m => m.EffectivePrice),
            };

            MoviesGrid.ItemsSource = list
                .Select(m => new MovieCatalogItem(m, _reviewCountByMovieId.TryGetValue(m.Id, out int rc) ? rc : 0))
                .ToList();
        }

        private void MovieCard_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is not Border border) return;
            border.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 201, 165, 90));
            border.Background = new SolidColorBrush(Color.FromArgb(255, 37, 35, 32));
            border.RenderTransform = new ScaleTransform { ScaleX = 1.03, ScaleY = 1.03 };
        }

        private void MovieCard_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (sender is not Border border) return;
            border.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 46, 43, 38));
            border.Background = new SolidColorBrush(Color.FromArgb(255, 28, 26, 23));
            border.RenderTransform = new ScaleTransform { ScaleX = 1, ScaleY = 1 };
        }

        private void MoviesGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is MovieCatalogItem item)
                NavigateToDetail(item.Movie);
        }

        private void ViewButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int movieId)
            {
                var source = MoviesGrid.ItemsSource as List<MovieCatalogItem>;
                var item = source?.FirstOrDefault(m => m.Id == movieId);
                if (item != null)
                    NavigateToDetail(item.Movie);
            }
        }

        private void NavigateToDetail(Movie movie)
            => Frame.Navigate(typeof(Features.MovieDetail.Views.MovieDetailPage), new MovieDetailNavArgs { Movie = movie });

        private async Task LoadDiscountedMoviesAsync()
        {
            var allMovies = await _movieRepo.SearchMoviesAsync("", 100);
            var currentSales = await _salesRepo.GetCurrentSalesAsync();

            foreach (var movie in allMovies)
            {
                var sale = currentSales.FirstOrDefault(s => s.Movie.Id == movie.Id);
                movie.ActiveSaleDiscountPercent = sale?.DiscountPercentage;
            }

            _context.ChangeTracker.Clear();

            var movieIds = allMovies.Select(m => m.Id).ToList();
            _reviewCountByMovieId = await _reviewRepo.GetReviewCountsAsync(movieIds);

            var onSaleIds = currentSales.Select(s => s.Movie.Id).ToHashSet();
            _sourceMovies = _showOnlySales
                ? allMovies.Where(m => onSaleIds.Contains(m.Id)).ToList()
                : allMovies;

            // Populate genre filter
            var genres = _sourceMovies
                .Select(m => m.PrimaryGenre)
                .Where(g => !string.IsNullOrEmpty(g))
                .Distinct()
                .OrderBy(g => g)
                .ToList();

            GenreFilter.Items.Clear();
            GenreFilter.Items.Add("All genres");
            foreach (var genre in genres)
                GenreFilter.Items.Add(genre);
            GenreFilter.SelectedIndex = 0;

            if (currentSales.Any())
            {
                var latestSale = currentSales.OrderByDescending(s => s.EndTime).First();
                if (_flashSaleVm == null)
                {
                    _flashSaleVm = new FlashSaleViewModel(latestSale.EndTime, () =>
                    {
                        DispatcherQueue.TryEnqueue(() => ApplyCatalogDeactivation(false));
                    });
                    _flashSaleVm.PropertyChanged += FlashSaleVm_PropertyChanged!;
                }
                ApplyCatalogDeactivation(true);
            }
            else if (_showOnlySales)
            {
                ApplyCatalogDeactivation(false);
            }
            else
            {
                ApplyCatalogDeactivation(true);
            }
        }

        private void FlashSaleVm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(FlashSaleViewModel.IsActive)) return;

            DispatcherQueue.TryEnqueue(async () =>
            {
                ApplyCatalogDeactivation(_flashSaleVm?.IsActive ?? false);
                if (_flashSaleVm?.IsActive ?? false)
                {
                    await LoadDiscountedMoviesAsync();
                    ApplyFilterAndSort();
                }
            });
        }

        private void ApplyCatalogDeactivation(bool isSaleActive)
        {
            if (isSaleActive)
            {
                FlashSaleEndedText.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                MoviesGrid.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                MoviesGrid.IsEnabled = true;
                MoviesGrid.Opacity = 1.0;
                return;
            }

            MoviesGrid.ItemsSource = null;
            _sourceMovies = new List<Movie>();
            FlashSaleEndedText.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            MoviesGrid.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }
}

namespace MovieApp.Features.PriceWatcher.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

public sealed class PriceWatcherViewModel : INotifyPropertyChanged
{
    private readonly IPriceWatcherService _priceWatcherService;
    private readonly IMovieService _movieService;

    private string _searchText = string.Empty;
    private Movie? _selectedMovie;
    private double _newTargetPrice;
    private bool _isLoading;
    private string _errorMessage = string.Empty;
    private string _successMessage = string.Empty;

    public PriceWatcherViewModel(IPriceWatcherService priceWatcherService, IMovieService movieService)
    {
        _priceWatcherService = priceWatcherService;
        _movieService = movieService;

        LoadWatchersCommand = new AsyncRelayCommand(LoadWatchersAsync);
        AddWatchCommand = new AsyncRelayCommand(AddWatchAsync);
        RemoveWatchCommand = new AsyncRelayCommand<int>(RemoveWatchAsync);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<PriceWatcher> Watchers { get; } = new();
    public ObservableCollection<Movie> SearchSuggestions { get; } = new();

    public IAsyncRelayCommand LoadWatchersCommand { get; }
    public IAsyncRelayCommand AddWatchCommand { get; }
    public IAsyncRelayCommand<int> RemoveWatchCommand { get; }

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public Movie? SelectedMovie
    {
        get => _selectedMovie;
        set => SetProperty(ref _selectedMovie, value);
    }

    /// <summary>
    /// Backed by <see cref="double"/> so it can be bound directly to NumberBox.Value;
    /// converted to <see cref="decimal"/> at submit time.
    /// </summary>
    public double NewTargetPrice
    {
        get => _newTargetPrice;
        set => SetProperty(ref _newTargetPrice, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public string SuccessMessage
    {
        get => _successMessage;
        private set => SetProperty(ref _successMessage, value);
    }

    public async Task LoadWatchersAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            List<PriceWatcher> result = await _priceWatcherService.GetAllWatchedEventsAsync();

            Watchers.Clear();
            foreach (PriceWatcher watcher in result)
            {
                Watchers.Add(watcher);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load watchers: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task SearchMoviesAsync(string text)
    {
        SelectedMovie = null;
        SearchSuggestions.Clear();

        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        try
        {
            List<Movie> results = await _movieService.SearchMoviesAsync(text);
            foreach (Movie movie in results)
            {
                SearchSuggestions.Add(movie);
            }
        }
        catch
        {
            // silently swallow search errors — suggestions are best-effort
        }
    }

    public void SelectMovie(Movie movie)
    {
        SelectedMovie = movie;
        SearchText = movie.Title;
    }

    public async Task AddWatchAsync()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        if (SelectedMovie is null)
        {
            ErrorMessage = "Please select a movie or event from the dropdown.";
            return;
        }

        if (NewTargetPrice <= 0)
        {
            ErrorMessage = "Target price must be greater than zero.";
            return;
        }

        PriceWatcher newWatch = new PriceWatcher
        {
            EventId = SelectedMovie.Id,
            EventTitle = SelectedMovie.Title,
            TargetPrice = (decimal)NewTargetPrice,
        };

        try
        {
            bool added = await _priceWatcherService.AddWatchAsync(newWatch);
            if (added)
            {
                SuccessMessage = $"Watcher added for '{newWatch.EventTitle}'.";
                ClearForm();
                await LoadWatchersAsync();
            }
            else
            {
                ErrorMessage = "The server refused to add the watcher (possibly a duplicate).";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to add watcher: {ex.Message}";
        }
    }

    public async Task RemoveWatchAsync(int id)
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            await _priceWatcherService.RemoveWatchAsync(id);
            SuccessMessage = $"Watcher #{id} removed.";
            await LoadWatchersAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to remove watcher: {ex.Message}";
        }
    }

    private void ClearForm()
    {
        SearchText = string.Empty;
        SelectedMovie = null;
        SearchSuggestions.Clear();
        NewTargetPrice = 0;
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

namespace MovieApp.Features.PriceWatcher.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

/// <summary>
/// View model for the Price Watcher page. Loads watchers from the DB (via the
/// registered <see cref="IPriceWatcherService"/> — backed by PriceWatcherProxyService
/// in the Desktop DI container) and exposes commands to add or remove a watcher.
/// </summary>
public sealed class PriceWatcherViewModel : INotifyPropertyChanged
{
    private readonly IPriceWatcherService _priceWatcherService;

    private string _newWatchTitle = string.Empty;
    private double _newTargetPrice;
    private double _newMovieId;
    private bool _isLoading;
    private string _errorMessage = string.Empty;
    private string _successMessage = string.Empty;

    public PriceWatcherViewModel(IPriceWatcherService priceWatcherService)
    {
        _priceWatcherService = priceWatcherService;

        LoadWatchersCommand = new AsyncRelayCommand(LoadWatchersAsync);
        AddWatchCommand = new AsyncRelayCommand(() => AddWatchAsync((int)NewMovieId));
        RemoveWatchCommand = new AsyncRelayCommand<int>(RemoveWatchAsync);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<PriceWatcher> Watchers { get; } = new();

    public IAsyncRelayCommand LoadWatchersCommand { get; }
    public IAsyncRelayCommand AddWatchCommand { get; }
    public IAsyncRelayCommand<int> RemoveWatchCommand { get; }

    /// <summary>
    /// Backed by <see cref="double"/> so it can be bound directly to NumberBox.Value.
    /// </summary>
    public double NewMovieId
    {
        get => _newMovieId;
        set => SetProperty(ref _newMovieId, value);
    }

    public string NewWatchTitle
    {
        get => _newWatchTitle;
        set => SetProperty(ref _newWatchTitle, value);
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

    /// <summary>
    /// Loads the full list of price watchers from the database via the proxy service.
    /// </summary>
    public async Task LoadWatchersAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            System.Collections.Generic.List<PriceWatcher> result =
                await _priceWatcherService.GetAllWatchedEventsAsync();

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

    /// <summary>
    /// Adds a new price watcher for the given movie / event id and refreshes the list
    /// so the database state is reflected immediately in the UI.
    /// </summary>
    public async Task AddWatchAsync(int movieId)
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        if (movieId <= 0)
        {
            ErrorMessage = "Please enter a valid movie id.";
            return;
        }

        if (NewTargetPrice <= 0)
        {
            ErrorMessage = "Target price must be greater than zero.";
            return;
        }

        PriceWatcher newWatch = new PriceWatcher
        {
            EventId = movieId,
            EventTitle = string.IsNullOrWhiteSpace(NewWatchTitle)
                ? $"Movie #{movieId}"
                : NewWatchTitle,
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

    /// <summary>
    /// Removes the price watcher identified by <paramref name="id"/> and refreshes the list.
    /// </summary>
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
        NewMovieId = 0;
        NewWatchTitle = string.Empty;
        NewTargetPrice = 0;
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!System.Collections.Generic.EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

namespace MovieApp.Features.Marathon.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Features.Shared.Models;

/// <summary>
/// View model for the marathons page. Manages marathon listing, enrollment, and progress.
/// </summary>
public sealed class MarathonViewModel : INotifyPropertyChanged
{
    private readonly IMarathonService _marathonService;

    private MarathonProgress? _progress;
    private bool _isEnrolled;
    private bool _isLoading;
    private Marathon? _selectedMarathon;

    public MarathonViewModel(IMarathonService marathonService)
    {
        _marathonService = marathonService;

        LoadMarathonsCommand = new AsyncRelayCommand(LoadMarathonsAsync);
        EnrollCommand = new AsyncRelayCommand(EnrollAsync, CanEnroll);
        LoadProgressCommand = new AsyncRelayCommand(LoadProgressAsync, CanLoadProgress);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Gets the collection of available marathons.</summary>
    public ObservableCollection<Marathon> Marathons { get; } = new();

    /// <summary>Gets or sets the current user's marathon progress.</summary>
    public MarathonProgress? Progress
    {
        get => _progress;
        private set
        {
            _progress = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CompletionPercentage));
            OnPropertyChanged(nameof(ProgressText));
        }
    }

    /// <summary>Gets or sets a value indicating whether the user is enrolled in the selected marathon.</summary>
    public bool IsEnrolled
    {
        get => _isEnrolled;
        private set
        {
            _isEnrolled = value;
            OnPropertyChanged();
            EnrollCommand.NotifyCanExecuteChanged();
        }
    }

    /// <summary>Gets or sets a value indicating whether data is loading.</summary>
    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    /// <summary>Gets or sets the currently selected marathon.</summary>
    public Marathon? SelectedMarathon
    {
        get => _selectedMarathon;
        set
        {
            _selectedMarathon = value;
            OnPropertyChanged();
            EnrollCommand.NotifyCanExecuteChanged();
            LoadProgressCommand.NotifyCanExecuteChanged();
        }
    }

    /// <summary>Gets the completion percentage for the progress bar (0.0 to 1.0).</summary>
    public double CompletionPercentage
    {
        get
        {
            if (Progress is null)
            {
                return 0;
            }

            int total = Marathons.Count > 0 ? Marathons.Count : 1;
            return (double)Progress.CompletedMoviesCount / total;
        }
    }

    /// <summary>Gets a text summary of the current progress.</summary>
    public string ProgressText => Progress is null
        ? "Not enrolled"
        : Progress.IsCompleted
            ? $"Completed — {Progress.CompletedMoviesCount} movies verified"
            : $"{Progress.CompletedMoviesCount} movies verified";

    public IAsyncRelayCommand LoadMarathonsCommand { get; }
    public IAsyncRelayCommand EnrollCommand { get; }
    public IAsyncRelayCommand LoadProgressCommand { get; }

    /// <summary>Loads the list of available marathons.</summary>
    public async Task LoadMarathonsAsync()
    {
        IsLoading = true;
        try
        {
            var marathons = await _marathonService.GetWeeklyMarathonsAsync(SessionManager.CurrentUserID);
            Marathons.Clear();
            foreach (var marathon in marathons)
            {
                Marathons.Add(marathon);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>Enrolls the current user in the selected marathon.</summary>
    public async Task EnrollAsync()
    {
        if (SelectedMarathon is null)
        {
            return;
        }

        IsLoading = true;
        try
        {
            bool success = await _marathonService.StartMarathonAsync(SelectedMarathon.Id);
            if (success)
            {
                IsEnrolled = true;
                await LoadProgressAsync();
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>Loads the current user's progress for the selected marathon.</summary>
    public async Task LoadProgressAsync()
    {
        if (SelectedMarathon is null)
        {
            return;
        }

        Progress = await _marathonService.GetUserProgressAsync(SessionManager.CurrentUserID, SelectedMarathon.Id);
        IsEnrolled = Progress is not null;
    }

    private bool CanEnroll() => SelectedMarathon is not null && !IsEnrolled;
    private bool CanLoadProgress() => SelectedMarathon is not null;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

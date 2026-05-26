namespace MovieApp.Features.Marathon.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Models;

/// <summary>
/// View model for the marathons page. Manages marathon selection, joining, leaderboards, and movie progress.
/// </summary>
public sealed class MarathonViewModel : INotifyPropertyChanged
{
    private readonly IMarathonService? _marathonService;

    private int _userId;
    private Marathon? _selectedMarathon;
    private IReadOnlyList<LeaderboardEntry> _leaderboard = new List<LeaderboardEntry>();
    private MarathonProgress? _currentProgress;
    private bool _isLocked;
    private bool _isJoined;
    private bool _isLoading;
    private bool _hasSelection;
    private bool _isDataAvailable;

    public MarathonViewModel()
    {
    }

    public MarathonViewModel(IMarathonService marathonService)
    {
        _marathonService = marathonService;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<MarathonDisplayItem> MarathonDisplayItems { get; } = new();
    public ObservableCollection<Marathon> Marathons { get; } = new();
    public ObservableCollection<MarathonMovieItem> Movies { get; } = new();

    public bool IsDataAvailable
    {
        get => _isDataAvailable;
        private set { _isDataAvailable = value; OnPropertyChanged(); }
    }

    public Marathon? SelectedMarathon
    {
        get => _selectedMarathon;
        private set => SetProperty(ref _selectedMarathon, value);
    }

    public IReadOnlyList<LeaderboardEntry> Leaderboard
    {
        get => _leaderboard;
        private set => SetProperty(ref _leaderboard, value);
    }

    public MarathonProgress? CurrentProgress
    {
        get => _currentProgress;
        private set
        {
            SetProperty(ref _currentProgress, value);
            OnPropertyChanged(nameof(ProgressText));
            OnPropertyChanged(nameof(CompletionPercentage));
        }
    }

    public bool IsLocked
    {
        get => _isLocked;
        private set => SetProperty(ref _isLocked, value);
    }

    public bool IsJoined
    {
        get => _isJoined;
        private set
        {
            SetProperty(ref _isJoined, value);
            OnPropertyChanged(nameof(ShowJoinButton));
            OnPropertyChanged(nameof(ShowMovieList));
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public bool HasSelection
    {
        get => _hasSelection;
        private set => SetProperty(ref _hasSelection, value);
    }

    public bool ShowJoinButton => SelectedMarathon is not null && !IsJoined && !IsLocked;
    public bool ShowMovieList => SelectedMarathon is not null && IsJoined;

    public string ProgressText => CurrentProgress is null
        ? "Not joined yet"
        : CurrentProgress.IsCompleted
            ? $"Completed — {CurrentProgress.CompletedMoviesCount} movies verified"
            : $"{CurrentProgress.CompletedMoviesCount} of {Movies.Count} movies verified";

    public double CompletionPercentage
    {
        get
        {
            if (CurrentProgress is null || Movies.Count == 0)
            {
                return 0;
            }

            return (double)CurrentProgress.CompletedMoviesCount / Movies.Count;
        }
    }

    public async Task LoadAsync(int userId)
    {
        _userId = userId;

        if (_marathonService is null)
        {
            IsDataAvailable = false;
            return;
        }

        IsDataAvailable = true;
        IEnumerable<Marathon> marathons = await _marathonService.GetWeeklyMarathonsAsync(userId);

        Marathons.Clear();
        MarathonDisplayItems.Clear();
        DateTime weekEnd = GetSundayEnd();

        foreach (Marathon marathon in marathons)
        {
            Marathons.Add(marathon);

            int participantCount = await _marathonService.GetParticipantCountAsync(marathon.Id);
            MarathonProgress? progress = await _marathonService.GetUserProgressAsync(userId, marathon.Id);
            int totalMovies = await _marathonService.GetMarathonMovieCountAsync(marathon.Id);

            MarathonDisplayItems.Add(new MarathonDisplayItem
            {
                Marathon = marathon,
                ParticipantCount = participantCount,
                UserAccuracy = progress?.TriviaAccuracy ?? 0,
                IsJoinedByUser = progress is not null,
                UserMoviesVerified = progress?.CompletedMoviesCount ?? 0,
                TotalMovies = totalMovies,
                WeekEnd = weekEnd,
            });
        }
    }

    public async Task SelectMarathonAsync(Marathon marathon)
    {
        SelectedMarathon = marathon;
        HasSelection = true;
        IsLoading = true;

        try
        {
            CurrentProgress = await _marathonService!.GetCurrentProgressAsync(marathon.Id);
            IsJoined = CurrentProgress is not null;
            IsLocked = false;

            if (marathon.PrerequisiteMarathonId is int prerequisiteMarathonId)
            {
                bool prereqDone = await _marathonService!.IsPrerequisiteCompletedAsync(
                    _userId,
                    prerequisiteMarathonId);
                IsLocked = !prereqDone;
            }

            IEnumerable<LeaderboardEntry> leaderboard = await _marathonService!.GetLeaderboardWithUsernamesAsync(marathon.Id);
            Leaderboard = leaderboard.ToList();

            if (IsJoined)
            {
                await LoadMoviesAsync(marathon.Id);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task<bool> JoinMarathonAsync(int marathonId)
    {
        bool success = await _marathonService!.StartMarathonAsync(marathonId);
        if (!success)
        {
            return false;
        }

        CurrentProgress = await _marathonService!.GetCurrentProgressAsync(marathonId);
        IsJoined = true;
        await LoadMoviesAsync(marathonId);
        return true;
    }

    public async Task RefreshAfterMovieLoggedAsync()
    {
        int marathonId = SelectedMarathon!.Id;
        CurrentProgress = await _marathonService!.GetCurrentProgressAsync(marathonId);

        IEnumerable<LeaderboardEntry> leaderboard =
            await _marathonService!.GetLeaderboardWithUsernamesAsync(marathonId);
        Leaderboard = leaderboard.ToList();

        await LoadMoviesAsync(marathonId);
    }

    public async Task LogMovieAsync(int marathonId, int movieId, int correctAnswers)
    {
        if (_marathonService is null)
        {
            throw new InvalidOperationException("Marathon service is not available.");
        }

        await _marathonService.LogMovieAsync(marathonId, movieId, correctAnswers);
    }

    private static DateTime GetSundayEnd()
    {
        DateTime now = DateTime.UtcNow;
        int daysFromMonday = ((int)now.DayOfWeek + 6) % 7;
        DateTime monday = now.Date.AddDays(-daysFromMonday);
        return monday.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);
    }

    private async Task LoadMoviesAsync(int marathonId)
    {
        if (_marathonService is null)
        {
            throw new InvalidOperationException("Marathon service is not available.");
        }

        IEnumerable<Movie> movies = await _marathonService.GetMoviesForMarathonAsync(marathonId);

        int verifiedCount = CurrentProgress?.CompletedMoviesCount ?? 0;
        Movies.Clear();
        IReadOnlyList<Movie> movieList = movies.ToList();

        for (int i = 0; i < movieList.Count; i++)
        {
            Movies.Add(new MarathonMovieItem
            {
                MovieId = movieList[i].Id,
                Title = movieList[i].Title,
                IsVerified = i < verifiedCount,
            });
        }

        OnPropertyChanged(nameof(ProgressText));
        OnPropertyChanged(nameof(CompletionPercentage));
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

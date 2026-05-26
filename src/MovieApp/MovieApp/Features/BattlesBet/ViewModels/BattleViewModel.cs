namespace MovieApp.Features.BattlesBet.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MovieApp.DataLayer.Models;
using MovieApp.Features.Shared.Models;
using MovieApp.Logic.Interfaces.Services;

/// <summary>
/// ViewModel for the battle arena page showing active battles, betting, and demo controls.
/// </summary>
public class BattleViewModel : INotifyPropertyChanged
{
    private readonly IBattleService _battleService;

    private Battle? _activeBattle;
    private bool _hasBattle;
    private bool _isBattleActive;
    private bool _showBetForm;
    private double _betAmount;
    private int _selectedBetMovieId;
    private int _totalPoints;
    private string _statusMessage = string.Empty;
    private BattleBet? _userBet;
    private bool _hasBet;
    private string _winnerMovieName = string.Empty;
    private bool _isProcessing;
    private Movie? _selectedBetMovie;

    public BattleViewModel(IBattleService battleService)
    {
        _battleService = battleService;

        LoadBattleCommand = new AsyncRelayCommand(() => LoadBattleAsync());
        ShowBetFormCommand = new RelayCommand(() => ShowBetForm = true);
        PlaceBetCommand = new AsyncRelayCommand(PlaceBetAsync);
        ForceSettleCommand = new AsyncRelayCommand(ForceSettleAsync);
        ResetDemoCommand = new AsyncRelayCommand(ResetDemoAsync);

        _ = LoadBattleAsync();
    }

    public ObservableCollection<Movie> BetMovieOptions { get; } = new();

    public Battle? ActiveBattle
    {
        get => _activeBattle;
        set => SetProperty(ref _activeBattle, value);
    }

    public bool HasBattle
    {
        get => _hasBattle;
        set
        {
            if (SetProperty(ref _hasBattle, value))
            {
                OnPropertyChanged(nameof(IsBattleFinished));
            }
        }
    }

    public bool IsBattleActive
    {
        get => _isBattleActive;
        set
        {
            if (SetProperty(ref _isBattleActive, value))
            {
                OnPropertyChanged(nameof(CanBet));
                OnPropertyChanged(nameof(IsBattleFinished));
            }
        }
    }

    public bool CanBet => IsBattleActive && !HasBet;

    public bool IsBattleFinished => HasBattle && !IsBattleActive;

    public bool ShowBetForm
    {
        get => _showBetForm;
        set => SetProperty(ref _showBetForm, value);
    }

    public double BetAmount
    {
        get => _betAmount;
        set => SetProperty(ref _betAmount, value);
    }

    public int SelectedBetMovieId
    {
        get => _selectedBetMovieId;
        set => SetProperty(ref _selectedBetMovieId, value);
    }

    public Movie? SelectedBetMovie
    {
        get => _selectedBetMovie;
        set
        {
            if (SetProperty(ref _selectedBetMovie, value))
            {
                SelectedBetMovieId = value?.Id ?? 0;
            }
        }
    }

    public int TotalPoints
    {
        get => _totalPoints;
        set => SetProperty(ref _totalPoints, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public BattleBet? UserBet
    {
        get => _userBet;
        set => SetProperty(ref _userBet, value);
    }

    public bool HasBet
    {
        get => _hasBet;
        set
        {
            if (SetProperty(ref _hasBet, value))
            {
                OnPropertyChanged(nameof(CanBet));
            }
        }
    }

    public string WinnerMovieName
    {
        get => _winnerMovieName;
        set => SetProperty(ref _winnerMovieName, value);
    }

    public bool IsProcessing
    {
        get => _isProcessing;
        set => SetProperty(ref _isProcessing, value);
    }

    public ICommand LoadBattleCommand { get; }

    public ICommand ShowBetFormCommand { get; }

    public ICommand PlaceBetCommand { get; }

    public ICommand ForceSettleCommand { get; }

    public ICommand ResetDemoCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task LoadBattleAsync(bool settleExpired = true)
    {
        StatusMessage = string.Empty;
        ShowBetForm = false;

        int userId = SessionManager.CurrentUserID;

        if (settleExpired)
        {
            await _battleService.SettleExpiredBattlesAsync();
        }

        ActiveBattle = await _battleService.GetCurrentBattleForUserAsync(userId);
        HasBattle = ActiveBattle != null;
        IsBattleActive = ActiveBattle?.Status == "Active";

        if (ActiveBattle != null)
        {
            BetMovieOptions.Clear();
            if (ActiveBattle.FirstMovie != null)
            {
                BetMovieOptions.Add(ActiveBattle.FirstMovie);
            }

            if (ActiveBattle.SecondMovie != null)
            {
                BetMovieOptions.Add(ActiveBattle.SecondMovie);
            }

            UserBet = await _battleService.GetBetAsync(userId, ActiveBattle.BattleId);
            HasBet = UserBet != null;

            if (IsBattleFinished)
            {
                try
                {
                    int winId = await _battleService.DetermineWinnerAsync(ActiveBattle.BattleId);
                    WinnerMovieName = winId == ActiveBattle.FirstMovie?.Id
                        ? ActiveBattle.FirstMovie?.Title ?? "Movie 1"
                        : ActiveBattle.SecondMovie?.Title ?? "Movie 2";
                }
                catch
                {
                    WinnerMovieName = "Unknown";
                }
            }
            else
            {
                WinnerMovieName = string.Empty;
            }
        }
    }

    private async Task PlaceBetAsync()
    {
        if (ActiveBattle == null || SelectedBetMovieId <= 0 || BetAmount <= 0)
        {
            StatusMessage = "Please select a movie and enter a valid bet amount.";
            return;
        }

        try
        {
            int userId = SessionManager.CurrentUserID;
            await _battleService.PlaceBetAsync(userId, ActiveBattle.BattleId, SelectedBetMovieId, (int)BetAmount);
            StatusMessage = $"Bet of {(int)BetAmount} points placed successfully!";
            ShowBetForm = false;
            await LoadBattleAsync(settleExpired: false);
        }
        catch (InvalidOperationException exception)
        {
            StatusMessage = exception.Message;
        }
    }

    private async Task ForceSettleAsync()
    {
        if (ActiveBattle == null || !IsBattleActive)
        {
            StatusMessage = "No active battle to settle.";
            return;
        }

        IsProcessing = true;
        try
        {
            await _battleService.ForceSettleBattleAsync(ActiveBattle.BattleId);
            StatusMessage = "Battle settled! Points have been distributed.";
            await LoadBattleAsync(settleExpired: false);
        }
        catch (InvalidOperationException exception)
        {
            StatusMessage = exception.Message;
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private async Task ResetDemoAsync()
    {
        IsProcessing = true;
        StatusMessage = string.Empty;
        try
        {
            await _battleService.ResetAllBattlesForDemoAsync();
            await _battleService.CreateDemoBattleAsync();
            StatusMessage = "Demo reset! A new battle has been created — place your bet!";
            await LoadBattleAsync(settleExpired: false);
        }
        catch (InvalidOperationException exception)
        {
            StatusMessage = exception.Message;
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

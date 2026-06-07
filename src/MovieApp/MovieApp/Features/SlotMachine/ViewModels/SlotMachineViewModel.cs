using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MovieApp.DataLayer.Models;
using MovieApp.Features.Shared.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Models;

namespace MovieApp.Features.SlotMachine.ViewModels
{
    public class SlotMachineViewModel : INotifyPropertyChanged
    {
        private readonly ISlotMachineService _service;

        private SlotMachineResult? _lastResult;
        private bool _isSpinning;
        private int _availableSpins;
        private int _bonusSpins;
        private int _loginStreak;
        private Genre _selectedGenre = new Genre();
        private Actor _selectedActor = new Actor();
        private Director _selectedDirector = new Director();

        public SlotMachineResult? LastResult
        {
            get => _lastResult;
            private set
            {
                _lastResult = value;
                OnPropertyChanged(nameof(LastResult));
                OnPropertyChanged(nameof(IsJackpot));
                OnPropertyChanged(nameof(JackpotMovieTitle));
                OnPropertyChanged(nameof(JackpotDiscountPercentage));
            }
        }

        public ObservableCollection<SlotMachineResult> History { get; } = new ObservableCollection<SlotMachineResult>();

        public ObservableCollection<MovieEvent> MatchingEvents { get; } = new ObservableCollection<MovieEvent>();

        public bool IsSpinning
        {
            get => _isSpinning;
            private set
            {
                _isSpinning = value;
                OnPropertyChanged(nameof(IsSpinning));
                SpinCommand.NotifyCanExecuteChanged();
            }
        }

        public int AvailableSpins
        {
            get => _availableSpins;
            private set
            {
                _availableSpins = value;
                OnPropertyChanged(nameof(AvailableSpins));
                SpinCommand.NotifyCanExecuteChanged();
            }
        }

        public int BonusSpins
        {
            get => _bonusSpins;
            private set
            {
                _bonusSpins = value;
                OnPropertyChanged(nameof(BonusSpins));
                SpinCommand.NotifyCanExecuteChanged();
            }
        }

        public int LoginStreak
        {
            get => _loginStreak;
            private set
            {
                _loginStreak = value;
                OnPropertyChanged(nameof(LoginStreak));
            }
        }

        public Genre SelectedGenre
        {
            get => _selectedGenre;
            private set
            {
                _selectedGenre = value;
                OnPropertyChanged(nameof(SelectedGenre));
            }
        }

        public Actor SelectedActor
        {
            get => _selectedActor;
            private set
            {
                _selectedActor = value;
                OnPropertyChanged(nameof(SelectedActor));
            }
        }

        public Director SelectedDirector
        {
            get => _selectedDirector;
            private set
            {
                _selectedDirector = value;
                OnPropertyChanged(nameof(SelectedDirector));
            }
        }

        public bool IsJackpot => _lastResult?.JackpotMovie != null;

        public string JackpotMovieTitle => _lastResult?.JackpotMovie?.Title ?? string.Empty;

        public int JackpotDiscountPercentage => _lastResult?.DiscountPercentage ?? 0;

        public IAsyncRelayCommand SpinCommand { get; }

        public IAsyncRelayCommand LoadStateCommand { get; }

        public SlotMachineViewModel(ISlotMachineService service)
        {
            _service = service;
            SpinCommand = new AsyncRelayCommand(ExecuteSpinAsync, CanSpin);
            LoadStateCommand = new AsyncRelayCommand(ExecuteLoadStateAsync);
        }

        private bool CanSpin() => !_isSpinning && _availableSpins > 0;

        public async Task ExecuteLoadStateAsync()
        {
            UserSpinData spinData = await _service.GetUserSpinStateAsync(SessionManager.CurrentUserID);
            AvailableSpins = spinData.DailySpinsRemaining + spinData.BonusSpins;
            BonusSpins = spinData.BonusSpins;
            LoginStreak = spinData.LoginStreak;
            SelectedGenre = await _service.GetRandomGenreAsync();
            SelectedActor = await _service.GetRandomActorAsync();
            SelectedDirector = await _service.GetRandomDirectorAsync();
        }

        private async Task ExecuteSpinAsync()
        {
            IsSpinning = true;
            try
            {
                SlotMachineResult result = await _service.SpinAsync(SessionManager.CurrentUserID);
                SelectedGenre = result.Genre;
                SelectedActor = result.Actor;
                SelectedDirector = result.Director;
                LastResult = result;
                MatchingEvents.Clear();
                foreach (MovieEvent evt in result.MatchingEvents)
                {
                    MatchingEvents.Add(evt);
                }
                History.Insert(0, result);
                UserSpinData refreshed = await _service.GetUserSpinStateAsync(SessionManager.CurrentUserID);
                AvailableSpins = refreshed.DailySpinsRemaining + refreshed.BonusSpins;
                BonusSpins = refreshed.BonusSpins;
            }
            finally
            {
                IsSpinning = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

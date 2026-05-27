using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Models;

namespace MovieApp.Features.Screenings.ViewModels
{
    public partial class ScreeningViewModel : ObservableObject
    {
        private readonly IScreeningService _screeningService;
        private readonly IBookingService _bookingService;

        [ObservableProperty]
        private ObservableCollection<Screening> _screenings = new();

        [ObservableProperty]
        private Screening? _selectedScreening;

        [ObservableProperty]
        private ObservableCollection<Seat> _seats = new();

        [ObservableProperty]
        private Seat? _selectedSeat;

        public ScreeningViewModel(
            IScreeningService screeningService,
            IBookingService bookingService)
        {
            _screeningService = screeningService;
            _bookingService = bookingService;
        }

        [RelayCommand]
        public async Task LoadScreeningsAsync()
        {
            try
            {
                var result = await _screeningService.GetAllScreeningsAsync();
                Screenings.Clear();
                foreach (var screening in result)
                {
                    Screenings.Add(screening);
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading screenings: {exception.Message}");
            }
        }

        partial void OnSelectedScreeningChanged(Screening? value)
        {
            if (value != null)
            {
                _ = LoadSeatMapAsync();
            }
            else
            {
                Seats.Clear();
            }
        }

        [RelayCommand]
        public async Task LoadSeatMapAsync()
        {
            if (SelectedScreening == null) return;

            try
            {
                var result = await _screeningService.GetAvailableSeatsAsync(SelectedScreening.Id);
                Seats.Clear();
                foreach (var seat in result)
                {
                    Seats.Add(seat);
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading seat map: {exception.Message}");
            }
        }

        [RelayCommand]
        public async Task BookSeatAsync()
        {
            if (SelectedScreening == null || SelectedSeat == null) return;

            try
            {
                var currentUserId = MovieApp.Features.Shared.Models.SessionManager.CurrentUserID;
                var seatsToBook = new System.Collections.Generic.List<(int Row, int Column)> { (SelectedSeat.Row, SelectedSeat.Column) };
                
                var success = await _bookingService.BookSeatsAsync(SelectedScreening.Id, currentUserId, seatsToBook);
                if (success)
                {
                    SelectedSeat.IsAvailable = false;
                    await LoadSeatMapAsync();
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error booking seat: {exception.Message}");
            }
        }
    }
}

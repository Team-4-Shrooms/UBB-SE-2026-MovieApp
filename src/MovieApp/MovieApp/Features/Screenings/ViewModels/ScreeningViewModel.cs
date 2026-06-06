using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebDTOs.DTOs;

namespace MovieApp.Features.Screenings.ViewModels;

public partial class ScreeningViewModel : ObservableObject
{
    private readonly IScreeningService _screeningService;
    private readonly IBookingService   _bookingService;

    [ObservableProperty]
    private ObservableCollection<ScreeningCardViewModel> _screenings = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DetailVisibility))]
    [NotifyPropertyChangedFor(nameof(EmptyVisibility))]
    private ScreeningCardViewModel? _selectedCard;

    [ObservableProperty]
    private ScreeningDetailsDto? _selectedScreening;

    [ObservableProperty]
    private ObservableCollection<SeatRowViewModel> _seatRows = new();

    [ObservableProperty]
    private string _selectedSeatsSummary = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalCostDisplay))]
    private decimal _totalCost;

    public string TotalCostDisplay => $"{TotalCost:F2} lei";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusMessageVisibility))]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public bool CanBook => SeatRows.SelectMany(r => r.Seats).Any(s => s.IsSelected);

    public Visibility DetailVisibility  => SelectedCard != null ? Visibility.Visible : Visibility.Collapsed;
    public Visibility EmptyVisibility   => SelectedCard == null ? Visibility.Visible : Visibility.Collapsed;
    public Visibility StatusMessageVisibility =>
        string.IsNullOrEmpty(StatusMessage) ? Visibility.Collapsed : Visibility.Visible;

    public ScreeningViewModel(IScreeningService screeningService, IBookingService bookingService)
    {
        _screeningService = screeningService;
        _bookingService   = bookingService;
    }

    [RelayCommand]
    public async Task LoadScreeningsAsync()
    {
        IsLoading = true;
        StatusMessage = string.Empty;
        try
        {
            var bare = await _screeningService.GetAllScreeningsAsync();
            Screenings.Clear();
            foreach (var s in bare)
            {
                var details = await _screeningService.GetScreeningDetailsAsync(s.Id);
                if (details is not null)
                    Screenings.Add(new ScreeningCardViewModel(details));
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load screenings: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedCardChanged(ScreeningCardViewModel? value)
    {
        SeatRows.Clear();
        SelectedSeatsSummary = string.Empty;
        TotalCost = 0;
        StatusMessage = string.Empty;
        SelectedScreening = value?.Details;

        if (value is null) return;

        var byRow = value.Details.Seats
            .GroupBy(s => s.Row)
            .OrderBy(g => g.Key);

        foreach (var row in byRow)
        {
            var seats = row.OrderBy(s => s.Column)
                          .Select(s => new SeatStatusViewModel(s, ToggleSeat));
            SeatRows.Add(new SeatRowViewModel(row.Key, seats));
        }
    }

    public void ToggleSeat(SeatStatusViewModel seat)
    {
        if (!seat.IsAvailable) return;
        seat.IsSelected = !seat.IsSelected;
        UpdateSelection();
    }

    private void UpdateSelection()
    {
        var selected = SeatRows.SelectMany(r => r.Seats).Where(s => s.IsSelected).ToList();
        SelectedSeatsSummary = selected.Count > 0
            ? string.Join(", ", selected.Select(s => s.DisplayLabel))
            : string.Empty;
        TotalCost = SelectedScreening is not null
            ? selected.Count * SelectedScreening.TicketPrice
            : 0;
        OnPropertyChanged(nameof(CanBook));
    }

    [RelayCommand]
    public async Task BookSeatsAsync()
    {
        if (SelectedScreening is null) return;

        var selected = SeatRows.SelectMany(r => r.Seats).Where(s => s.IsSelected).ToList();
        if (selected.Count == 0) return;

        IsLoading = true;
        StatusMessage = string.Empty;
        try
        {
            var userId = MovieApp.Features.Shared.Models.SessionManager.CurrentUserID;
            IReadOnlyList<(int Row, int Column)> pairs = selected
                .Select(s => (s.Row, s.Column))
                .ToList();

            var ok = await _bookingService.BookSeatsAsync(SelectedScreening.ScreeningId, userId, pairs);
            if (ok)
            {
                StatusMessage = $"Booked {selected.Count} seat(s) successfully!";
                var refreshed = await _screeningService.GetScreeningDetailsAsync(SelectedScreening.ScreeningId);
                if (refreshed is not null)
                {
                    var newCard = new ScreeningCardViewModel(refreshed);
                    var idx = Screenings.ToList().FindIndex(c => c.Details.ScreeningId == refreshed.ScreeningId);
                    if (idx >= 0) Screenings[idx] = newCard;
                    SelectedCard = newCard;
                }
            }
            else
            {
                StatusMessage = "Booking failed — some seats may no longer be available.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}

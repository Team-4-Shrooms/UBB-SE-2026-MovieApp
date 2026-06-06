using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Features.Screenings.ViewModels;

public partial class MyBookingsViewModel : ObservableObject
{
    private readonly IScreeningService _screeningService;
    private readonly IBookingService   _bookingService;

    [ObservableProperty]
    private ObservableCollection<BookingGroupViewModel> _bookingGroups = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public MyBookingsViewModel(IScreeningService screeningService, IBookingService bookingService)
    {
        _screeningService = screeningService;
        _bookingService   = bookingService;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        StatusMessage = string.Empty;
        try
        {
            var userId   = MovieApp.Features.Shared.Models.SessionManager.CurrentUserID;
            var bookings = await _bookingService.GetBookingsForUserAsync(userId);

            BookingGroups.Clear();
            var byScreening = bookings.GroupBy(b => b.ScreeningId);
            foreach (var group in byScreening)
            {
                var details = await _screeningService.GetScreeningDetailsAsync(group.Key);
                if (details is null) continue;
                var items = group.Select(b => new BookingItemViewModel(b));
                BookingGroups.Add(new BookingGroupViewModel(details, items));
            }

            if (BookingGroups.Count == 0)
                StatusMessage = "You have no bookings yet.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to load bookings: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task CancelBookingAsync(int bookingId)
    {
        IsLoading = true;
        StatusMessage = string.Empty;
        try
        {
            var userId = MovieApp.Features.Shared.Models.SessionManager.CurrentUserID;
            var ok     = await _bookingService.CancelBookingAsync(bookingId, userId);
            StatusMessage = ok ? "Booking cancelled." : "Could not cancel booking.";
            if (ok) await LoadAsync();
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

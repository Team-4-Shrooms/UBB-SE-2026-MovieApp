using System.Collections.Generic;
using MovieApp.DataLayer.Models;
using MovieApp.WebDTOs.DTOs;

namespace MovieApp.Features.Screenings.ViewModels;

public sealed class BookingItemViewModel
{
    public int    BookingId { get; }
    public string Label     { get; }

    public BookingItemViewModel(Booking booking)
    {
        BookingId = booking.Id;
        Label     = $"Row {booking.Row}, Seat {booking.Column}";
    }
}

public sealed class BookingGroupViewModel
{
    public ScreeningDetailsDto           Screening   { get; }
    public IReadOnlyList<BookingItemViewModel> Seats { get; }

    public int     Count        => Seats.Count;
    public decimal TotalCost    => Count * Screening.TicketPrice;
    public string  TotalDisplay => $"€{TotalCost:F2}";
    public string  TimeDisplay  => Screening.ScreeningTime.ToString("ddd d MMM yyyy • HH:mm");

    public BookingGroupViewModel(ScreeningDetailsDto screening, IEnumerable<BookingItemViewModel> seats)
    {
        Screening = screening;
        Seats     = new List<BookingItemViewModel>(seats);
    }
}

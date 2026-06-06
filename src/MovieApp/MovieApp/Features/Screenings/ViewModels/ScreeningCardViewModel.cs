using MovieApp.WebDTOs.DTOs;

namespace MovieApp.Features.Screenings.ViewModels;

public sealed class ScreeningCardViewModel
{
    public ScreeningDetailsDto Details { get; }

    public string MovieTitle     => Details.MovieTitle;
    public string MoviePosterUrl => Details.MoviePosterUrl;
    public string EventTitle     => Details.EventTitle;
    public string EventLocation  => Details.EventLocation;
    public string RoomName       => Details.RoomName;
    public string TimeDisplay    => Details.ScreeningTime.ToString("ddd d MMM yyyy • HH:mm");
    public string PriceDisplay   => $"{Details.TicketPrice:F2} lei";

    public ScreeningCardViewModel(ScreeningDetailsDto details) => Details = details;
}

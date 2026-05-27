using System.Collections.Generic;
using MovieApp.WebDTOs.DTOs;

namespace MovieApp.Web.Models;

public sealed class CheckoutViewModel
{
    public ScreeningDetailsDto Screening { get; set; } = null!;
    public List<SeatViewModel> Seats { get; set; } = new();
}

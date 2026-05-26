using System.Collections.Generic;
using MovieApp.DataLayer.Models;
using MovieApp.WebDTOs.DTOs;

namespace MovieApp.Web.Models;

public sealed class MyBookingGroupViewModel
{
    public ScreeningDetailsDto Details { get; set; } = null!;
    public List<Booking> Bookings { get; set; } = new();
}

namespace MovieApp.DataLayer.Models;

using System;

public sealed class Booking
{
    public int Id { get; set; }

    public int ScreeningId { get; set; }

    public int UserId { get; set; }

    public int Row { get; set; }

    public int Column { get; set; }

    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
}

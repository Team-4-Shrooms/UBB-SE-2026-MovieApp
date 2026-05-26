namespace MovieApp.WebDTOs.DTOs;

using System;
using System.Collections.Generic;

public sealed class ScreeningDetailsDto
{
    public int ScreeningId { get; set; }

    public DateTime ScreeningTime { get; set; }

    public int MovieId { get; set; }

    public string MovieTitle { get; set; } = string.Empty;

    public string MoviePosterUrl { get; set; } = string.Empty;

    public int EventId { get; set; }

    public string EventTitle { get; set; } = string.Empty;

    public string EventLocation { get; set; } = string.Empty;

    public decimal TicketPrice { get; set; }

    public int RoomId { get; set; }

    public string RoomName { get; set; } = string.Empty;

    public int Rows { get; set; }

    public int Columns { get; set; }

    public List<SeatStatusDto> Seats { get; set; } = new();
}

public sealed class SeatStatusDto
{
    public int Row { get; set; }

    public int Column { get; set; }

    public bool IsAvailable { get; set; }
}

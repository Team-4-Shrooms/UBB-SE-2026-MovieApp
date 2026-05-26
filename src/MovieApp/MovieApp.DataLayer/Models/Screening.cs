
namespace MovieApp.DataLayer.Models;

using System;

public sealed class Screening
{
    required public int Id { get; init; }

    required public int EventId { get; init; }

    required public int MovieId { get; init; }

    required public int RoomId { get; init; }

    required public DateTime ScreeningTime { get; init; }

    [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "decimal(18,2)")]
    public decimal TicketPrice { get; set; }
}

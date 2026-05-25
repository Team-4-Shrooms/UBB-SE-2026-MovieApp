
namespace MovieApp.DataLayer.Models;

using System;

public sealed class Screening
{
    required public int Id { get; init; }

    required public int EventId { get; init; }

    required public int MovieId { get; init; }

    required public int RoomId { get; init; }

    required public DateTime ScreeningTime { get; init; }
}

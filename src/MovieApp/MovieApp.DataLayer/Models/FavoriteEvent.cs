namespace MovieApp.DataLayer.Models;

public sealed class FavoriteEvent
{
    public int Id { get; set; }

    required public int UserId { get; init; }

    required public int EventId { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

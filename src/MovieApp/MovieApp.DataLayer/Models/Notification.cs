namespace MovieApp.DataLayer.Models;

using System;
public sealed class Notification
{
    public int Id { get; set; }

    required public int UserId { get; init; }

    required public int EventId { get; init; }

    required public string Type { get; init; }

    required public string Message { get; init; }

    public NotificationState State { get; set; } = NotificationState.Unread;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

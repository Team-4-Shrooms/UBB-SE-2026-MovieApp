using System.ComponentModel.DataAnnotations.Schema;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Models;

public sealed class MarathonProgress
{
    required public int UserId { get; init; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    required public int MarathonId { get; init; }

    public DateTime JoinedAt { get; set; } = DateTime.Now;

    public double TriviaAccuracy { get; set; }

    public int CompletedMoviesCount { get; set; }

    public DateTime? FinishedAt { get; set; }

    public bool IsCompleted => this.FinishedAt.HasValue;
}

namespace MovieApp.DataLayer.Models;

using System;
public sealed class LeaderboardEntry
{
    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public LeaderboardEntry()
    {
    }

    required public int UserId { get; init; }

    required public string Username { get; init; }

    public int CompletedMoviesCount { get; init; }

    public double TriviaAccuracy { get; init; }

    public DateTime? FinishedAt { get; init; }
}

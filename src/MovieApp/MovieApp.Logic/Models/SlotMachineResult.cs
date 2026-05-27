namespace MovieApp.Logic.Models;

using MovieApp.DataLayer.Models;

public sealed class SlotMachineResult
{
    /// <summary>
    /// Gets or sets Selected genre reel.
    /// </summary>
    public Genre Genre { get; set; } = new Genre();

    /// <summary>
    /// Gets or sets Selected actor reel.
    /// </summary>
    public Actor Actor { get; set; } = new Actor();

    /// <summary>
    /// Gets or sets Selected director reel.
    /// </summary>
    public Director Director { get; set; } = new Director();

    /// <summary>
    /// Gets or sets Events matching the combination.
    /// </summary>
    public List<MovieEvent> MatchingEvents { get; set; } = new List<MovieEvent>();

    /// <summary>
    /// Gets or sets IDs of events that contain the jackpot movie (for UI highlighting).
    /// </summary>
    public HashSet<int> JackpotEventIds { get; set; } = new HashSet<int>();

    /// <summary>
    /// Gets or Sets Movie triggering jackpot (nullable).
    /// </summary>
    public Movie? JackpotMovie { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether jackpot discount applied.
    /// </summary>
    public bool JackpotDiscountApplied { get; set; }

    /// <summary>
    /// Gets or Sets Discount applied if jackpot (0 if none).
    /// </summary>
    public int DiscountPercentage { get; set; }
}

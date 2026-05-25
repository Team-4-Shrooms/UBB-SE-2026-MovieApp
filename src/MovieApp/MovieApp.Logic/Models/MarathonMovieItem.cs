namespace MovieApp.Logic.Models;

/// <summary>
/// Represents a movie within a marathon, including its verification state.
/// </summary>
public sealed class MarathonMovieItem
{
    /// <summary>Gets or sets the unique identifier of the movie.</summary>
    public int MovieId { get; set; }

    /// <summary>Gets or sets the title of the movie.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets a value indicating whether the movie has been verified by the user.</summary>
    public bool IsVerified { get; set; }

    /// <summary>Gets a value indicating whether the movie can be logged (not yet verified).</summary>
    public bool CanLog => !IsVerified;

    /// <summary>Gets the opacity for the log button based on verification state.</summary>
    public double CanLogOpacity => CanLog ? 1.0 : 0.0;

    /// <summary>Gets the opacity for the verified checkmark based on verification state.</summary>
    public double IsVerifiedOpacity => IsVerified ? 1.0 : 0.0;

    /// <summary>Gets the status text for the movie.</summary>
    public string StatusText => IsVerified ? "Verified" : "Not verified yet";
}

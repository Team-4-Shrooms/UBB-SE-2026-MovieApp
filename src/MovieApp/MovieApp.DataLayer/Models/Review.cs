namespace MovieApp.DataLayer.Models;
public class Review
{
    public int ReviewId { get; set; }
    public float StarRating { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsExtraReview { get; set; }
    public int CinematographyRating { get; set; }
    public string? CinematographyText { get; set; }
    public int ActingRating { get; set; }
    public string? ActingText { get; set; }
    public int CgiRating { get; set; }
    public string? CgiText { get; set; }
    public int PlotRating { get; set; }
    public string? PlotText { get; set; }
    public int SoundRating { get; set; }
    public string? SoundText { get; set; }
    public User? User { get; set; }
    public Movie? Movie { get; set; }
}

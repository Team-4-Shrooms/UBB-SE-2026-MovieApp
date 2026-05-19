namespace MovieApp.DataLayer.Models;

public sealed class MarathonDisplayItem
{
    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public MarathonDisplayItem() { }

    private const double RatingDivisor = 20.0;
    private const double MaximumRatingValue = 5.0;
    private const int LargeCapacityValue = 999999;
    private const int SystemCreatorId = 0;
    private const decimal FreeTicketPrice = 0;

    required public Marathon Marathon { get; init; }
    public int ParticipantCount { get; init; }
    public double UserAccuracy { get; init; }
    public bool IsJoinedByUser { get; init; }
    public int UserMoviesVerified { get; init; }
    public int TotalMovies { get; init; }
    public DateTime WeekEnd { get; init; }

    public MovieEvent ToEvent()
    {
        var locationDescription = this.Marathon.PrerequisiteMarathonId.HasValue
            ? "Elite Marathon"
            : "Standard Marathon";

        var calculatedRating = this.IsJoinedByUser ? this.UserAccuracy / RatingDivisor : 0.0;
        calculatedRating = Math.Round(Math.Min(calculatedRating, MaximumRatingValue), 1);

        return new MovieEvent
        {
            Id = this.Marathon.Id,
            Title = this.Marathon.Title,
            Description = this.Marathon.Description ?? "A weekly themed movie marathon.",
            Date = this.WeekEnd,
            Location = locationDescription,
            TicketPrice = FreeTicketPrice,
            Capacity = LargeCapacityValue,
            // MovieEvent has no Rating/Enrollment/CreatorUserId — omit or extend if needed
        };
    }
}

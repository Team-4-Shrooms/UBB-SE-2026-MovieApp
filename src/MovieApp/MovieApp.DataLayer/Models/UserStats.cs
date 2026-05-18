namespace MovieApp.DataLayer.Models;
public class UserStats
{
    public int UserStatsId { get; set; }
    public int TotalPoints { get; set; }
    public int WeeklyScore { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
}

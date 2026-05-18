namespace MovieApp.DataLayer.Models;

public class UserBadge
{
    public int UserBadgeId { get; set; }
    public User? User { get; set; }
    public Badge? Badge { get; set; }
}


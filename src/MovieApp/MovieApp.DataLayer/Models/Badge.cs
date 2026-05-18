using MovieApp.DataLayer.Models;

public class Badge
{
    public int BadgeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CriteriaValue { get; set; }
    public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
}

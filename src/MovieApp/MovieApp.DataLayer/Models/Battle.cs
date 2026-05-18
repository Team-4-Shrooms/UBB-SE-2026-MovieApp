namespace MovieApp.DataLayer.Models;
public class Battle
{
    public int BattleId { get; set; }
    public double InitialRatingFirstMovie { get; set; }
    public double InitialRatingSecondMovie { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Active";
    public Movie? FirstMovie { get; set; }
    public Movie? SecondMovie { get; set; }
    public ICollection<BattleBet> Bets { get; set; } = new List<BattleBet>();
}

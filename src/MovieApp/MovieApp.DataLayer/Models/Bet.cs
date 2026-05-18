namespace MovieApp.DataLayer.Models;
public class Bet
{
    public int BetId { get; set; }
    public int Amount { get; set; }
    public User? User { get; set; }
    public Battle? Battle { get; set; }
    public Movie? Movie { get; set; }
}

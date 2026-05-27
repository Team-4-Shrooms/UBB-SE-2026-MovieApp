namespace MovieApp.Web.Models;

public sealed class SeatViewModel
{
    public int Row { get; set; }
    public int Col { get; set; }
    public bool IsAvailable { get; set; }
}

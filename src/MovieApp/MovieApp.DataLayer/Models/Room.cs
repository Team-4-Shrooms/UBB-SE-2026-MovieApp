namespace MovieApp.DataLayer.Models;

public sealed class Room
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Rows { get; set; }

    public int Columns { get; set; }

    public int Capacity => this.Rows * this.Columns;
}

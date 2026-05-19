namespace MovieApp.DataLayer.Models;
public static class Room
{
    public const int MinRows = 10;
    public const int MaxRows = 15;
    public const int MinColumns = 12;
    public const int MaxColumns = 20;

    public static (int Rows, int Columns) For(int screeningId)
    {
        int seed = screeningId <= 0 ? 1 : screeningId;
        int rowSpan = MaxRows - MinRows + 1;
        int colSpan = MaxColumns - MinColumns + 1;
        int rows = MinRows + (seed * 7 % rowSpan);
        int columns = MinColumns + (seed * 13 % colSpan);
        return (rows, columns);
    }
}

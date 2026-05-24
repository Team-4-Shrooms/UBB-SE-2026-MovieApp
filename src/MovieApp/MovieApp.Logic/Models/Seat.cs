namespace MovieApp.Logic.Models;

using System.ComponentModel;
using System.Runtime.CompilerServices;


public enum SeatQuality
{
    /// <summary>
    /// Restricted view or less comfortable seating.
    /// </summary>
    Poor,

    /// <summary>
    /// Standard viewing experience.
    /// </summary>
    Standard,

    /// <summary>
    /// Prime viewing location with the best visibility.
    /// </summary>
    Optimal,
}

public sealed class Seat : INotifyPropertyChanged
{
    private const string ColorPoor = "#FF4D4D";
    private const string ColorOptimal = "#4CAF50";
    private const string ColorStandard = "#FFC107";
    private const string ColorDefault = "#E0E0E0";

    private bool isAvailable = true;
    private bool isSelected;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int Row { get; set; }

    public int Column { get; set; }

    public SeatQuality Quality { get; set; }

    public bool IsSweetSpot { get; set; }

    public bool IsAvailable
    {
        get => this.isAvailable;
        set
        {
            if (this.isAvailable != value)
            {
                this.isAvailable = value;
                this.OnPropertyChanged();
            }
        }
    }

    public bool IsSelected
    {
        get => this.isSelected;
        set
        {
            if (this.isSelected != value)
            {
                this.isSelected = value;
                this.OnPropertyChanged();
            }
        }
    }

    public string Label => $"R{this.Row}C{this.Column}";

    public string SeatColor => this.Quality switch
    {
        SeatQuality.Poor => ColorPoor,
        SeatQuality.Optimal => ColorOptimal,
        SeatQuality.Standard => ColorStandard,
        _ => ColorDefault
    };

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using MovieApp.WebDTOs.DTOs;
using Windows.UI;

namespace MovieApp.Features.Screenings.ViewModels;

public partial class SeatStatusViewModel : ObservableObject
{
    public int  Row       { get; }
    public int  Column    { get; }
    public bool IsAvailable { get; }
    public string DisplayLabel => $"R{Row}C{Column}";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SeatBackground))]
    [NotifyPropertyChangedFor(nameof(SeatForeground))]
    private bool _isSelected;

    public IRelayCommand ToggleCommand { get; }

    public SolidColorBrush SeatBackground
    {
        get
        {
            if (!IsAvailable) return new SolidColorBrush(Color.FromArgb(255, 64, 60, 56));
            if (IsSelected)   return new SolidColorBrush(Color.FromArgb(255, 201, 165, 90));
            return new SolidColorBrush(Color.FromArgb(255, 37, 35, 32));
        }
    }

    public SolidColorBrush SeatForeground =>
        IsSelected ? new SolidColorBrush(Colors.Black)
                   : new SolidColorBrush(Color.FromArgb(255, 245, 240, 232));

    public SeatStatusViewModel(SeatStatusDto dto, Action<SeatStatusViewModel> toggleCallback)
    {
        Row      = dto.Row;
        Column   = dto.Column;
        IsAvailable = dto.IsAvailable;
        ToggleCommand = new RelayCommand(() => toggleCallback(this));
    }
}

using System.Collections.Generic;

namespace MovieApp.Features.Screenings.ViewModels;

public sealed class SeatRowViewModel
{
    public int RowNumber { get; }
    public IReadOnlyList<SeatStatusViewModel> Seats { get; }

    public SeatRowViewModel(int rowNumber, IEnumerable<SeatStatusViewModel> seats)
    {
        RowNumber = rowNumber;
        Seats = new List<SeatStatusViewModel>(seats);
    }
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using System;

namespace MovieApp.Logic.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo;

        public BookingService(IBookingRepository bookingRepo)
        {
            _bookingRepo = bookingRepo;
        }

        public Task<IReadOnlyList<Booking>> GetBookingsForScreeningAsync(int screeningId, CancellationToken cancellationToken = default)
        {
            return _bookingRepo.GetByScreeningAsync(screeningId, cancellationToken);
        }

        public Task<bool> BookSeatsAsync(int screeningId, int userId, IReadOnlyList<(int Row, int Column)> seats, CancellationToken cancellationToken = default)
        {
            return _bookingRepo.ReserveAsync(screeningId, userId, seats, cancellationToken);
        }

        public Task<bool> CancelBookingAsync(int bookingId, int userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.Proxy.Services
{
    public class BookingProxyService : IBookingService
    {
        private readonly ApiClient _apiClient;

        public BookingProxyService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IReadOnlyList<Booking>> GetBookingsForScreeningAsync(int screeningId, CancellationToken cancellationToken = default)
        {
            var result = await _apiClient.GetAsync<List<Booking>>($"api/screenings/{screeningId}/bookings");
            return result ?? new List<Booking>();
        }

        public async Task<bool> BookSeatsAsync(int screeningId, int userId, IReadOnlyList<(int Row, int Column)> seats, CancellationToken cancellationToken = default)
        {
            var body = new BookSeatsRequestBody
            {
                UserId = userId,
                Seats = seats.Select(s => new SeatRequest { Row = s.Row, Column = s.Column }).ToList()
            };
            
            try
            {
                await _apiClient.PostAsync($"api/screenings/{screeningId}/book", body);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CancelBookingAsync(int bookingId, int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                await _apiClient.PostAsync($"api/bookings/{bookingId}/cancel", new { UserId = userId });
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

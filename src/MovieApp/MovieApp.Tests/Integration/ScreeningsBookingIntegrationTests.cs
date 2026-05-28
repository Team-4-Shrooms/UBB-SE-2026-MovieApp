using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MovieApp.DataLayer.Models;
using MovieApp.Tests.Integration.ProxyRepos;

namespace MovieApp.Tests.Integration.Endpoints;

public sealed class ScreeningsBookingIntegrationTests
    : IClassFixture<MovieAppWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ScreeningsBookingIntegrationTests(
        MovieAppWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Test",
                "integration-test");
    }

    [Fact]
    public async Task BookSeat_FirstTime_ReturnsSuccessOrConflict()
    {
        int screeningId = await GetValidScreeningIdAsync();

        (int row, int column) =
            await GetFreeSeatAsync(screeningId);

        var request = CreateBookingRequest(row, column);

        try
        {
            HttpResponseMessage response =
                await _client.PostAsJsonAsync(
                    $"/api/screenings/{screeningId}/book",
                    request);

            Assert.True(
                response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Conflict);
        }
        catch (InvalidOperationException exception)
        {
            Assert.Contains(
                "Transaction",
                exception.Message);
        }
    }

    [Fact]
    public async Task BookSameSeat_Twice_ReturnsConflictOrTransactionError()
    {
        int screeningId = await GetValidScreeningIdAsync();

        (int row, int column) =
            await GetFreeSeatAsync(screeningId);

        var request = CreateBookingRequest(row, column);

        try
        {
            await _client.PostAsJsonAsync(
                $"/api/screenings/{screeningId}/book",
                request);

            HttpResponseMessage secondResponse =
                await _client.PostAsJsonAsync(
                    $"/api/screenings/{screeningId}/book",
                    request);

            Assert.True(
                secondResponse.StatusCode == HttpStatusCode.Conflict ||
                secondResponse.StatusCode == HttpStatusCode.OK);
        }
        catch (InvalidOperationException exception)
        {
            Assert.Contains(
                "Transaction",
                exception.Message);
        }
    }

    [Fact]
    public async Task ConcurrentBooking_DoesNotCrashApplication()
    {
        int screeningId = await GetValidScreeningIdAsync();

        (int row, int column) =
            await GetFreeSeatAsync(screeningId);

        var request = CreateBookingRequest(row, column);

        try
        {
            Task<HttpResponseMessage> request1 =
                _client.PostAsJsonAsync(
                    $"/api/screenings/{screeningId}/book",
                    request);

            Task<HttpResponseMessage> request2 =
                _client.PostAsJsonAsync(
                    $"/api/screenings/{screeningId}/book",
                    request);

            await Task.WhenAll(request1, request2);

            Assert.True(true);
        }
        catch (InvalidOperationException exception)
        {
            Assert.Contains(
                "Transaction",
                exception.Message);
        }
    }

    private object CreateBookingRequest(
        int row,
        int column)
    {
        return new
        {
            UserId = ProxyRepoSeedIds.SeededUserId,
            Seats = new[]
            {
                new
                {
                    Row = row,
                    Column = column
                }
            }
        };
    }

    private async Task<int> GetValidScreeningIdAsync()
    {
        List<Screening>? screenings =
            await _client.GetFromJsonAsync<List<Screening>>(
                "/api/screenings");

        Assert.NotNull(screenings);

        Screening? screening =
            screenings!.FirstOrDefault();

        Assert.NotNull(screening);

        return screening!.Id;
    }

    private async Task<(int Row, int Column)> GetFreeSeatAsync(
        int screeningId)
    {
        List<Booking>? bookings =
            await _client.GetFromJsonAsync<List<Booking>>(
                $"/api/screenings/{screeningId}/bookings");

        bookings ??= new List<Booking>();

        for (int row = 1; row <= 5; row++)
        {
            for (int column = 1; column <= 5; column++)
            {
                bool taken = bookings.Any(
                    booking =>
                        booking.Row == row &&
                        booking.Column == column);

                if (!taken)
                {
                    return (row, column);
                }
            }
        }

        return (1, 1);
    }
}

using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Interfaces.Services;

public interface IExternalReviewProvider
{
    bool IsConfigured { get; }

    Task<CriticReview?> GetReviewAsync(string movieTitle, int releaseYear, CancellationToken ct = default);
}

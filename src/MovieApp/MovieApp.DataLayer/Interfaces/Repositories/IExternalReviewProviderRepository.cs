using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces.Repositories;

public interface IExternalReviewProviderRepository
{
    Task<CriticReview?> GetReviewAsync(string movieTitle, int releaseYear, CancellationToken ct = default);
}


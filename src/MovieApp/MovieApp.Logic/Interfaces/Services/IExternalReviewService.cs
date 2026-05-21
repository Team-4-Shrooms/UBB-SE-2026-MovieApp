using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Interfaces.Services;

public interface IExternalReviewService
{
    Task<List<CriticReview>> GetExternalReviewsAsync(string movieTitle, int releaseYear, CancellationToken ct = default);
    List<(string Word, int Count)> AnalyseLexicon(List<CriticReview> reviews);
}

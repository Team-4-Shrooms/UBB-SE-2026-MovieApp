using System.Threading;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Logic.Services;

public sealed class ExternalReviewService : IExternalReviewService
{
    private static readonly HashSet<string> s_stopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "the", "a", "an", "and", "or", "but", "in", "on", "at", "to", "for", "of", "with",
        "this", "that", "is", "it", "by", "as", "from", "was", "were", "are", "be", "been",
        "has", "have", "had", "not", "no", "but", "if", "then", "than", "so", "such",
    };

    private readonly IEnumerable<IExternalReviewProvider> _providers;

    public ExternalReviewService(IEnumerable<IExternalReviewProvider> providers)
    {
        _providers = providers;
    }

    public async Task<List<CriticReview>> GetExternalReviewsAsync(string movieTitle, int releaseYear, CancellationToken ct = default)
    {
        var configuredProviders = _providers.Where(p => p.IsConfigured);

        var tasks = configuredProviders.Select(async provider =>
        {
            try
            {
                return await provider.GetReviewAsync(movieTitle, releaseYear, ct);
            }
            catch
            {
                return null;
            }
        });

        var results = await Task.WhenAll(tasks);
        return results.Where(r => r != null).Cast<CriticReview>().ToList();
    }

    public List<(string Word, int Count)> AnalyseLexicon(List<CriticReview> reviews)
    {
        var wordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var review in reviews)
        {
            var content = $"{review.Snippet} {review.Headline}";
            var words = content.Split(new[] { ' ', '.', ',', '!', '?', ';', ':', '(', ')', '\'', '"' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                var clean = word.ToLowerInvariant().Trim();
                if (clean.Length < 3 || s_stopWords.Contains(clean))
                {
                    continue;
                }

                if (!wordCounts.TryAdd(clean, 1))
                {
                    wordCounts[clean]++;
                }
            }
        }

        return wordCounts.OrderByDescending(kv => kv.Value).Take(10).Select(kv => (kv.Key, kv.Value)).ToList();
    }
}

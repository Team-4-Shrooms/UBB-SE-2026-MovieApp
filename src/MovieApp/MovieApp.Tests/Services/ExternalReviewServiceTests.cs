using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Services;

namespace MovieApp.Tests.Services
{
    public sealed class ExternalReviewServiceTests
    {
        [Fact]
        public async Task GetExternalReviewsAsync_OneConfiguredProvider_ReturnsItsReview()
        {
            CriticReview expected = new() { Source = "OMDb", Headline = "h", Snippet = "s", Url = "u" };
            FakeProvider configured = new(isConfigured: true, result: expected);
            FakeProvider unconfigured = new(isConfigured: false, result: null);

            ExternalReviewService service = new(new IExternalReviewProvider[] { configured, unconfigured });

            List<CriticReview> reviews = await service.GetExternalReviewsAsync("Inception", 2010);

            Assert.Single(reviews);
            Assert.Same(expected, reviews[0]);
        }

        [Fact]
        public async Task GetExternalReviewsAsync_UnconfiguredProvider_IsNotInvoked()
        {
            FakeProvider unconfigured = new(isConfigured: false, result: null);

            ExternalReviewService service = new(new IExternalReviewProvider[] { unconfigured });

            await service.GetExternalReviewsAsync("Inception", 2010);

            Assert.Equal(0, unconfigured.CallCount);
        }

        [Fact]
        public async Task GetExternalReviewsAsync_AllProvidersUnconfigured_ReturnsEmpty()
        {
            ExternalReviewService service = new(new IExternalReviewProvider[]
            {
                new FakeProvider(isConfigured: false, result: null),
                new FakeProvider(isConfigured: false, result: null),
            });

            List<CriticReview> reviews = await service.GetExternalReviewsAsync("Inception", 2010);

            Assert.Empty(reviews);
        }

        [Fact]
        public async Task GetExternalReviewsAsync_ProviderThrows_OthersStillReturn()
        {
            CriticReview good = new() { Source = "Guardian", Headline = "g", Snippet = "s", Url = "u" };
            ExternalReviewService service = new(new IExternalReviewProvider[]
            {
                new ThrowingProvider(),
                new FakeProvider(isConfigured: true, result: good),
            });

            List<CriticReview> reviews = await service.GetExternalReviewsAsync("Inception", 2010);

            Assert.Single(reviews);
            Assert.Same(good, reviews[0]);
        }

        [Fact]
        public async Task GetExternalReviewsAsync_AllProvidersThrow_ReturnsEmptyNoException()
        {
            ExternalReviewService service = new(new IExternalReviewProvider[]
            {
                new ThrowingProvider(),
                new ThrowingProvider(),
            });

            List<CriticReview> reviews = await service.GetExternalReviewsAsync("Inception", 2010);

            Assert.Empty(reviews);
        }

        [Fact]
        public async Task GetExternalReviewsAsync_ProviderReturnsNull_NullIsFilteredOut()
        {
            CriticReview good = new() { Source = "OMDb", Headline = "g", Snippet = "s", Url = "u" };
            ExternalReviewService service = new(new IExternalReviewProvider[]
            {
                new FakeProvider(isConfigured: true, result: null),
                new FakeProvider(isConfigured: true, result: good),
            });

            List<CriticReview> reviews = await service.GetExternalReviewsAsync("Inception", 2010);

            Assert.Single(reviews);
            Assert.Same(good, reviews[0]);
        }

        [Fact]
        public async Task GetExternalReviewsAsync_MultipleConfiguredProviders_AggregatesAll()
        {
            CriticReview a = new() { Source = "OMDb", Headline = "a" };
            CriticReview b = new() { Source = "NYT", Headline = "b" };
            CriticReview c = new() { Source = "Guardian", Headline = "c" };
            ExternalReviewService service = new(new IExternalReviewProvider[]
            {
                new FakeProvider(isConfigured: true, result: a),
                new FakeProvider(isConfigured: true, result: b),
                new FakeProvider(isConfigured: true, result: c),
            });

            List<CriticReview> reviews = await service.GetExternalReviewsAsync("Inception", 2010);

            Assert.Equal(3, reviews.Count);
            Assert.Contains(a, reviews);
            Assert.Contains(b, reviews);
            Assert.Contains(c, reviews);
        }

        [Fact]
        public void AnalyseLexicon_CountsRepeatedKeywordsIgnoringStopWordsAndShortTokens()
        {
            List<CriticReview> reviews = new()
            {
                new CriticReview { Headline = "Brilliant brilliant cinematography", Snippet = "The plot is brilliant and the acting is the best" },
                new CriticReview { Headline = "Plot twists", Snippet = "Acting was outstanding" },
            };

            ExternalReviewService service = new(Array.Empty<IExternalReviewProvider>());

            List<(string Word, int Count)> top = service.AnalyseLexicon(reviews);

            (string Word, int Count) brilliant = top.Single(t => t.Word == "brilliant");
            Assert.Equal(3, brilliant.Count);
            Assert.DoesNotContain(top, t => t.Word == "the" || t.Word == "is");
        }

        private sealed class FakeProvider : IExternalReviewProvider
        {
            private readonly CriticReview? _result;

            public FakeProvider(bool isConfigured, CriticReview? result)
            {
                IsConfigured = isConfigured;
                _result = result;
            }

            public bool IsConfigured { get; }

            public int CallCount { get; private set; }

            public Task<CriticReview?> GetReviewAsync(string movieTitle, int releaseYear, CancellationToken ct = default)
            {
                CallCount++;
                return Task.FromResult(_result);
            }
        }

        private sealed class ThrowingProvider : IExternalReviewProvider
        {
            public bool IsConfigured => true;

            public Task<CriticReview?> GetReviewAsync(string movieTitle, int releaseYear, CancellationToken ct = default)
                => throw new InvalidOperationException("simulated provider failure");
        }
    }
}

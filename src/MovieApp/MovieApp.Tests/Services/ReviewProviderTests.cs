using System.Net.Http;
using Microsoft.Extensions.Options;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Services;

namespace MovieApp.Tests.Services
{
    public sealed class ReviewProviderTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("placeholder")]
        [InlineData("PLACEHOLDER")]
        public void Omdb_IsConfigured_FalseForMissingOrPlaceholderKey(string key)
        {
            OmdbReviewProvider provider = BuildOmdb(key, new StubCache(string.Empty));
            Assert.False(provider.IsConfigured);
        }

        [Theory]
        [InlineData("")]
        [InlineData("placeholder")]
        public void Nyt_IsConfigured_FalseForMissingOrPlaceholderKey(string key)
        {
            NytReviewProvider provider = BuildNyt(nytKey: key, omdbKey: "real-omdb", new StubCache(string.Empty));
            Assert.False(provider.IsConfigured);
        }

        [Theory]
        [InlineData("")]
        [InlineData("placeholder")]
        public void Guardian_IsConfigured_FalseForMissingOrPlaceholderKey(string key)
        {
            GuardianReviewProvider provider = BuildGuardian(key, new StubCache(string.Empty));
            Assert.False(provider.IsConfigured);
        }

        [Fact]
        public void Omdb_IsConfigured_TrueForRealKey()
        {
            OmdbReviewProvider provider = BuildOmdb("57b3a80a", new StubCache(string.Empty));
            Assert.True(provider.IsConfigured);
        }

        [Fact]
        public async Task Omdb_UnconfiguredKey_ReturnsNullWithoutCallingCache()
        {
            StubCache cache = new(string.Empty);
            OmdbReviewProvider provider = BuildOmdb("placeholder", cache);

            CriticReview? result = await provider.GetReviewAsync("Inception", 2010);

            Assert.Null(result);
            Assert.Equal(0, cache.CallCount);
        }

        [Fact]
        public async Task Omdb_EmptyMovieTitle_ReturnsNull()
        {
            OmdbReviewProvider provider = BuildOmdb("real-key", new StubCache("{}"));

            CriticReview? result = await provider.GetReviewAsync("  ", 2010);

            Assert.Null(result);
        }

        [Fact]
        public async Task Omdb_CacheReturnsEmpty_ReturnsNull()
        {
            OmdbReviewProvider provider = BuildOmdb("real-key", new StubCache(string.Empty));

            CriticReview? result = await provider.GetReviewAsync("Inception", 2010);

            Assert.Null(result);
        }

        [Fact]
        public async Task Omdb_CacheReturnsValidJsonWithRating_ReturnsParsedReview()
        {
            const string Json = """
                {
                  "Ratings": [
                    { "Source": "Internet Movie Database", "Value": "8.8/10" },
                    { "Source": "Rotten Tomatoes", "Value": "87%" }
                  ]
                }
                """;
            OmdbReviewProvider provider = BuildOmdb("real-key", new StubCache(Json));

            CriticReview? result = await provider.GetReviewAsync("Inception", 2010);

            Assert.NotNull(result);
            Assert.Equal("Internet Movie Database", result!.Source);
            Assert.Equal(8.8, result.Score, 1);
            Assert.Contains("Inception", result.Snippet);
        }

        [Fact]
        public async Task Omdb_CacheReturnsJsonWithNoRatings_ReturnsNull()
        {
            OmdbReviewProvider provider = BuildOmdb("real-key", new StubCache("{\"Ratings\":[]}"));

            CriticReview? result = await provider.GetReviewAsync("Inception", 2010);

            Assert.Null(result);
        }

        [Fact]
        public async Task Omdb_CacheReturnsInvalidJson_ReturnsNullDoesNotThrow()
        {
            OmdbReviewProvider provider = BuildOmdb("real-key", new StubCache("not json"));

            CriticReview? result = await provider.GetReviewAsync("Inception", 2010);

            Assert.Null(result);
        }

        [Fact]
        public async Task Omdb_CacheThrows_ReturnsNullDoesNotThrow()
        {
            OmdbReviewProvider provider = BuildOmdb("real-key", new ThrowingCache());

            CriticReview? result = await provider.GetReviewAsync("Inception", 2010);

            Assert.Null(result);
        }

        [Fact]
        public async Task Guardian_CacheReturnsMatchingResult_ReturnsReview()
        {
            const string Json = """
                {
                  "response": {
                    "results": [
                      {
                        "webTitle": "Inception review – Christopher Nolan's mind-bender",
                        "webUrl": "https://example.com/inception",
                        "fields": { "trailText": "Inception is a 2010 thriller." }
                      }
                    ]
                  }
                }
                """;
            GuardianReviewProvider provider = BuildGuardian("real-key", new StubCache(Json));

            CriticReview? result = await provider.GetReviewAsync("Inception", 2010);

            Assert.NotNull(result);
            Assert.Equal("The Guardian", result!.Source);
            Assert.Contains("Inception", result.Headline);
        }

        [Fact]
        public async Task Guardian_NoMatch_ReturnsNull()
        {
            const string Json = """
                {
                  "response": {
                    "results": [
                      { "webTitle": "Completely unrelated", "webUrl": "x", "fields": { "trailText": "nothing relevant" } }
                    ]
                  }
                }
                """;
            GuardianReviewProvider provider = BuildGuardian("real-key", new StubCache(Json));

            CriticReview? result = await provider.GetReviewAsync("Inception", 2010);

            Assert.Null(result);
        }

        [Fact]
        public async Task Nyt_UnconfiguredKey_ReturnsNullWithoutCallingCache()
        {
            StubCache cache = new(string.Empty);
            NytReviewProvider provider = BuildNyt(nytKey: "placeholder", omdbKey: "real-omdb", cache);

            CriticReview? result = await provider.GetReviewAsync("Inception", 2010);

            Assert.Null(result);
            Assert.Equal(0, cache.CallCount);
        }

        private static OmdbReviewProvider BuildOmdb(string apiKey, ICacheService cache)
        {
            ExternalReviewsOptions opts = new() { Omdb = new() { ApiKey = apiKey } };
            return new OmdbReviewProvider(new HttpClient(), cache, Options.Create(opts));
        }

        private static NytReviewProvider BuildNyt(string nytKey, string omdbKey, ICacheService cache)
        {
            ExternalReviewsOptions opts = new()
            {
                Nyt = new() { ApiKey = nytKey },
                Omdb = new() { ApiKey = omdbKey },
            };
            return new NytReviewProvider(new HttpClient(), cache, Options.Create(opts));
        }

        private static GuardianReviewProvider BuildGuardian(string apiKey, ICacheService cache)
        {
            ExternalReviewsOptions opts = new() { Guardian = new() { ApiKey = apiKey } };
            return new GuardianReviewProvider(new HttpClient(), cache, Options.Create(opts));
        }

        private sealed class StubCache : ICacheService
        {
            private readonly string _payload;

            public StubCache(string payload) => _payload = payload;

            public int CallCount { get; private set; }

            public Task<string> FetchOrCacheAsync(string cacheKey, string url, HttpClient client, CancellationToken ct = default)
            {
                CallCount++;
                return Task.FromResult(_payload);
            }
        }

        private sealed class ThrowingCache : ICacheService
        {
            public Task<string> FetchOrCacheAsync(string cacheKey, string url, HttpClient client, CancellationToken ct = default)
                => throw new HttpRequestException("simulated network failure");
        }
    }
}

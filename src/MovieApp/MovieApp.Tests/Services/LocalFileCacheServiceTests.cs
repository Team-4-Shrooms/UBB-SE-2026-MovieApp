using System.IO;
using System.Net;
using System.Net.Http;
using MovieApp.Logic.Services;

namespace MovieApp.Tests.Services
{
    public sealed class LocalFileCacheServiceTests
    {
        [Fact]
        public async Task FetchOrCacheAsync_FreshFileExists_ReturnsCachedContentWithoutHttp()
        {
            using TempCacheDir dir = new();
            string cacheKey = $"unit_{Guid.NewGuid():N}";
            string cachePath = Path.Combine(dir.Path, $"{cacheKey}.json");
            await File.WriteAllTextAsync(cachePath, "cached-body");

            CountingHandler handler = new(HttpStatusCode.OK, "live-body");
            HttpClient client = new(handler);
            LocalFileCacheService cache = new();

            // The service writes to AppContext.BaseDirectory/ApiCache, so seed there too:
            string serviceCachePath = Path.Combine(AppContext.BaseDirectory, "ApiCache", $"{cacheKey}.json");
            Directory.CreateDirectory(Path.GetDirectoryName(serviceCachePath)!);
            await File.WriteAllTextAsync(serviceCachePath, "cached-body");

            try
            {
                string result = await cache.FetchOrCacheAsync(cacheKey, "https://example.com", client);

                Assert.Equal("cached-body", result);
                Assert.Equal(0, handler.CallCount);
            }
            finally
            {
                File.Delete(serviceCachePath);
            }
        }

        [Fact]
        public async Task FetchOrCacheAsync_NoCacheFile_FetchesAndCaches()
        {
            string cacheKey = $"unit_{Guid.NewGuid():N}";
            string serviceCachePath = Path.Combine(AppContext.BaseDirectory, "ApiCache", $"{cacheKey}.json");
            if (File.Exists(serviceCachePath))
            {
                File.Delete(serviceCachePath);
            }

            CountingHandler handler = new(HttpStatusCode.OK, "live-body");
            HttpClient client = new(handler);
            LocalFileCacheService cache = new();

            try
            {
                string result = await cache.FetchOrCacheAsync(cacheKey, "https://example.com", client);

                Assert.Equal("live-body", result);
                Assert.Equal(1, handler.CallCount);
                Assert.True(File.Exists(serviceCachePath));
                Assert.Equal("live-body", await File.ReadAllTextAsync(serviceCachePath));
            }
            finally
            {
                if (File.Exists(serviceCachePath))
                {
                    File.Delete(serviceCachePath);
                }
            }
        }

        [Fact]
        public async Task FetchOrCacheAsync_HttpError_ReturnsEmptyStringNoThrow()
        {
            string cacheKey = $"unit_{Guid.NewGuid():N}";
            CountingHandler handler = new(HttpStatusCode.InternalServerError, "boom");
            HttpClient client = new(handler);
            LocalFileCacheService cache = new();

            string result = await cache.FetchOrCacheAsync(cacheKey, "https://example.com", client);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task FetchOrCacheAsync_HttpException_ReturnsEmptyStringNoThrow()
        {
            string cacheKey = $"unit_{Guid.NewGuid():N}";
            ThrowingHandler handler = new();
            HttpClient client = new(handler);
            LocalFileCacheService cache = new();

            string result = await cache.FetchOrCacheAsync(cacheKey, "https://example.com", client);

            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData("", "https://x")]
        [InlineData("key", "")]
        public async Task FetchOrCacheAsync_BlankInputs_ReturnsEmptyString(string key, string url)
        {
            LocalFileCacheService cache = new();
            string result = await cache.FetchOrCacheAsync(key, url, new HttpClient());
            Assert.Equal(string.Empty, result);
        }

        private sealed class CountingHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _status;
            private readonly string _body;

            public CountingHandler(HttpStatusCode status, string body)
            {
                _status = status;
                _body = body;
            }

            public int CallCount { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                CallCount++;
                return Task.FromResult(new HttpResponseMessage(_status) { Content = new StringContent(_body) });
            }
        }

        private sealed class ThrowingHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => throw new HttpRequestException("simulated");
        }

        private sealed class TempCacheDir : IDisposable
        {
            public TempCacheDir()
            {
                Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"cache-{Guid.NewGuid():N}");
                Directory.CreateDirectory(Path);
            }

            public string Path { get; }

            public void Dispose()
            {
                try
                {
                    if (Directory.Exists(Path))
                    {
                        Directory.Delete(Path, recursive: true);
                    }
                }
                catch
                {
                    // best-effort cleanup
                }
            }
        }
    }
}

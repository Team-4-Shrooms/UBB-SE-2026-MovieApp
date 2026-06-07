namespace MovieApp.Auth
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using MovieApp.Logic.Interfaces.Services;
    using MovieApp.Proxy;

    /// <summary>
    /// Obtains a JWT token from the WebApi at startup and provides it to proxy services.
    /// Also exposes the authenticated user's ID via <see cref="ICurrentUserService"/>.
    /// </summary>
    public sealed class WinUiAuthTokenProvider : IAuthTokenProvider, ICurrentUserService
    {
        private const string LoginEndpoint = "api/auth/login";
        private const string DefaultUsername = "admin";
        private const string DefaultPassword = "password123";
        private const string BaseUrl = "http://localhost:5100/";
        private static readonly TimeSpan LoginTimeout = TimeSpan.FromSeconds(5);

        private string? token;
        private int userId;

        /// <summary>
        /// Logs in to the WebApi and stores the JWT token and user ID.
        /// Must be called via Task.Run to avoid deadlocking the UI thread.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                using HttpClient client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
                using CancellationTokenSource cts = new CancellationTokenSource(LoginTimeout);

                HttpResponseMessage response = await client.PostAsJsonAsync(
                    LoginEndpoint,
                    new { Username = DefaultUsername, Password = DefaultPassword },
                    cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    LoginResponse? result = await response.Content.ReadFromJsonAsync<LoginResponse>(
                        cancellationToken: cts.Token);
                    if (result is not null)
                    {
                        this.token = result.Token;
                        this.userId = result.UserId;
                    }
                }
            }
            catch
            {
                // App can still start without a token; all API calls will return 401.
            }
        }

        /// <inheritdoc/>
        public string? GetToken() => this.token;

        /// <inheritdoc/>
        public Task RefreshAsync() => this.InitializeAsync();

        /// <inheritdoc/>
        public int UserId => this.userId;

        private sealed record LoginResponse(string Token, int UserId, DateTime ExpiresAt);
    }
}

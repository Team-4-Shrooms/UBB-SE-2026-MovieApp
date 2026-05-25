namespace MovieApp.Web.Auth
{
    using MovieApp.Logic.Interfaces.Services;

    /// <summary>
    /// Provides the current authenticated user ID
    /// from the shared JWT token store.
    /// </summary>
    public sealed class WebCurrentUserService : ICurrentUserService
    {
        private readonly JwtTokenStore _tokenStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebCurrentUserService"/> class.
        /// </summary>
        /// <param name="tokenStore">Shared JWT token store.</param>
        public WebCurrentUserService(JwtTokenStore tokenStore)
        {
            this._tokenStore = tokenStore;
        }

        /// <inheritdoc/>
        public int UserId => this._tokenStore.UserId;
    }
}

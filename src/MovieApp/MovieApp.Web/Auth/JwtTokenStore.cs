namespace MovieApp.Web.Auth
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using MovieApp.Logic.Interfaces.Services;
    using MovieApp.Proxy;

    public sealed class JwtTokenStore : IAuthTokenProvider, ICurrentUserService
    {
        private const string TokenKey = "jwt_token";
        private const string UserIdKey = "jwt_user_id";

        private readonly IHttpContextAccessor httpContextAccessor;

        public JwtTokenStore(IHttpContextAccessor httpContextAccessor)
            => this.httpContextAccessor = httpContextAccessor;

        private ISession Session => this.httpContextAccessor.HttpContext!.Session;

        public string? GetToken() => this.Session.GetString(TokenKey);

        public int UserId
        {
            get
            {
                // Session is the primary source (has the live JWT user ID)
                if (int.TryParse(this.Session.GetString(UserIdKey), out int sessionId) && sessionId > 0)
                    return sessionId;

                // Fall back to the cookie claim written at login — survives session expiry
                var claim = this.httpContextAccessor.HttpContext?.User
                    ?.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null && int.TryParse(claim.Value, out int claimId) && claimId > 0)
                    return claimId;

                return 0;
            }
        }

        public Task RefreshAsync() => Task.CompletedTask;

        public void SetToken(string token, int userId)
        {
            this.Session.SetString(TokenKey, token);
            this.Session.SetString(UserIdKey, userId.ToString());
        }

        public void Clear()
        {
            this.Session.Remove(TokenKey);
            this.Session.Remove(UserIdKey);
        }
    }
}

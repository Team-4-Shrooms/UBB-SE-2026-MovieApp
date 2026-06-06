namespace MovieApp.Web.Controllers
{
    using System.Net.Http.Json;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using MovieApp.Web.Auth;
    using MovieApp.Web.Models.Auth;

    public sealed class AuthController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private readonly JwtTokenStore tokenStore;

        public AuthController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            JwtTokenStore tokenStore)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.tokenStore = tokenStore;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (this.User.Identity?.IsAuthenticated == true)
            {
                return this.RedirectToAction("Index", "Home");
            }

            return this.View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            string baseUrl = this.configuration["WebApi:BaseUrl"]!.TrimEnd('/');
            HttpClient client = this.httpClientFactory.CreateClient();

            HttpResponseMessage response = await client.PostAsJsonAsync(
                $"{baseUrl}/api/auth/login",
                new { model.Username, model.Password });

            if (!response.IsSuccessStatusCode)
            {
                model.Error = "Invalid username or password.";
                return this.View(model);
            }

            LoginResponse? result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result is null)
            {
                model.Error = "Service unavailable, please try again.";
                return this.View(model);
            }

            this.tokenStore.SetToken(result.Token, result.UserId);
            await this.SignInWithCookieAsync(result.UserId, model.Username);
            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return this.View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                model.Error = "Passwords do not match.";
                return this.View(model);
            }

            string baseUrl = this.configuration["WebApi:BaseUrl"]!.TrimEnd('/');
            HttpClient client = this.httpClientFactory.CreateClient();

            HttpResponseMessage response = await client.PostAsJsonAsync(
                $"{baseUrl}/api/auth/register",
                new
                {
                    model.Username,
                    model.Email,
                    model.Password,
                    model.ConfirmPassword,
                });

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                string body = await response.Content.ReadAsStringAsync();
                model.Error = body.Contains("Username") ? "Username is already in use." : "Email is already in use.";
                return this.View(model);
            }

            if (!response.IsSuccessStatusCode)
            {
                model.Error = "Registration failed. Please try again.";
                return this.View(model);
            }

            LoginResponse? result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result is null)
            {
                model.Error = "Service unavailable, please try again.";
                return this.View(model);
            }

            this.tokenStore.SetToken(result.Token, result.UserId);
            await this.SignInWithCookieAsync(result.UserId, model.Username);
            return this.RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            this.tokenStore.Clear();
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> ForceLogout()
        {
            this.tokenStore.Clear();
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.RedirectToAction("Login");
        }

        private async Task SignInWithCookieAsync(int userId, string username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        private sealed record LoginResponse(string Token, int UserId, DateTime ExpiresAt);
    }
}

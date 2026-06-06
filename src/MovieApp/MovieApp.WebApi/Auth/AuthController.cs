namespace MovieApp.WebApi.Auth
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MovieApp.DataLayer.Interfaces.Repositories;
    using MovieApp.DataLayer.Models;

    /// <summary>
    /// Handles authentication — issues JWT tokens on successful login.
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly JwtTokenService tokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        public AuthController(IUserRepository userRepository, JwtTokenService tokenService)
        {
            this.userRepository = userRepository;
            this.tokenService = tokenService;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="request">Login credentials.</param>
        /// <returns>A JWT token with user ID and expiry, or 401 on failure.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return this.Unauthorized();
            }

            var user = await this.userRepository.GetUserByUsernameAsync(request.Username);
            if (user is null)
            {
                return this.Unauthorized();
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!passwordValid)
            {
                return this.Unauthorized();
            }

            (string token, DateTime expiresAt) = this.tokenService.GenerateToken(user.Id, user.Username);

            return this.Ok(new LoginResponse(token, user.Id, expiresAt));
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
        {
            if (request.Password != request.ConfirmPassword)
            {
                return this.BadRequest("Passwords do not match.");
            }

            bool usernameTaken = await this.userRepository.GetUserByUsernameAsync(request.Username) is not null;
            if (usernameTaken)
            {
                return this.Conflict("Username is already in use.");
            }

            bool emailTaken = await this.userRepository.GetUserByEmailAsync(request.Email) is not null;
            if (emailTaken)
            {
                return this.Conflict("Email is already in use.");
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hash,
                Balance = 0m,
            };

            User created = await this.userRepository.CreateUserAsync(newUser);
            (string token, DateTime expiresAt) = this.tokenService.GenerateToken(created.Id, created.Username);
            return this.Ok(new LoginResponse(token, created.Id, expiresAt));
        }
    }

    /// <summary>Login request body.</summary>
    public sealed record LoginRequest(string Username, string Password);

    /// <summary>Successful login response.</summary>
    public sealed record LoginResponse(string Token, int UserId, DateTime ExpiresAt);

    /// <summary>Register request body.</summary>
    public sealed record RegisterRequest(string Username, string Email, string Password, string ConfirmPassword);
}

namespace MovieApp.Web.Models.Auth
{
    public sealed class RegisterViewModel
    {
        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;

        public string? Error { get; set; }
    }
}

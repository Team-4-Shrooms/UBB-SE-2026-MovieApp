namespace MovieApp.DataLayer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public decimal Balance { get; set; }
        public string? AuthProvider { get; set; }
        public string? AuthSubject { get; set; }
        public string? StableId => (AuthProvider != null && AuthSubject != null)? $"{AuthProvider}:{AuthSubject}": null;

        // --- Navigation Properties (Crucial for EF Core) ---
        public UserProfile? Profile { get; set; }
        public ICollection<OwnedMovie> OwnedMovies { get; set; } = new List<OwnedMovie>();
        public ICollection<OwnedTicket> OwnedTickets { get; set; } = new List<OwnedTicket>();
        public ICollection<Equipment> EquipmentForSale { get; set; } = new List<Equipment>();
        public ICollection<Reel> CreatedReels { get; set; } = new List<Reel>();
        public ICollection<UserReelInteraction> ReelInteractions { get; set; } = new List<UserReelInteraction>();
        public ICollection<UserMoviePreference> MoviePreferences { get; set; } = new List<UserMoviePreference>();

        // Navigation collections for Transactions
        public ICollection<Transaction> Purchases { get; set; } = new List<Transaction>();
        public ICollection<Transaction> Sales { get; set; } = new List<Transaction>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<BattleBet> Bets { get; set; } = new List<BattleBet>();
        public UserStats? UserStats { get; set; }
        public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    }
}


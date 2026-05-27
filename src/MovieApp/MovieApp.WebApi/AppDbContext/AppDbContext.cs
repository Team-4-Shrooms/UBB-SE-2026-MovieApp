using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer;
using MovieApp.DataLayer.Interfaces;

namespace MovieApp.WebApi.Data
{
    public class AppDbContext : DbContext, IMovieAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Core Catalog
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Director> Directors { get; set; }

        // Commerce
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<MovieEvent> MovieEvents { get; set; }
        public DbSet<ActiveSale> ActiveSales { get; set; }
        public DbSet<MovieReview> MovieReviews { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<OwnedMovie> OwnedMovies { get; set; }
        public DbSet<OwnedTicket> OwnedTickets { get; set; }
        public DbSet<PriceWatcher> PriceWatchers { get; set; }

        // Social 
        public DbSet<Reel> Reels { get; set; }
        public DbSet<MusicTrack> MusicTracks { get; set; }
        public DbSet<ScrapeJob> ScrapeJobs { get; set; }
        public DbSet<ScrapeJobLog> ScrapeJobLogs { get; set; }
        public DbSet<UserMoviePreference> UserMoviePreferences { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserReelInteraction> UserReelInteractions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Comment> Comments { get; set; }

        // Events & Screenings 
        public DbSet<Event> Events { get; set; }
        public DbSet<Screening> Screenings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // Gamification 
        public DbSet<Battle> Battles { get; set; }
        public DbSet<BattleBet> BattleBets { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<UserStats> UserStats { get; set; }
        public DbSet<UserSpinData> UserSpinData { get; set; }
        public DbSet<TriviaQuestion> TriviaQuestions { get; set; }
        public DbSet<TriviaReward> TriviaRewards { get; set; }

        // Marathons
        public DbSet<Marathon> Marathons { get; set; }
        public DbSet<MarathonProgress> MarathonProgressions { get; set; }

        // Favorites
        public DbSet<FavoriteEvent> FavoriteEvents { get; set; }

        // Ambassador & Referrals
        public DbSet<AmbassadorProfile> AmbassadorProfiles { get; set; }
        public DbSet<ReferralLog> ReferralLogs { get; set; }

        public DbSet<Reward> Rewards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1-to-1 Relationships 

            modelBuilder.Entity<User>()
                .HasOne(user => user.Profile)
                .WithOne(userProfile => userProfile.User)
                .HasForeignKey<UserProfile>("UserId");

            modelBuilder.Entity<Movie>()
                .HasOne(movie => movie.ActiveSale)
                .WithOne(activeSale => activeSale.Movie)
                .HasForeignKey<ActiveSale>("MovieId");

            // Composite / Explicit PKs 

            modelBuilder.Entity<AmbassadorProfile>()
                .HasKey(ambassadorProfile => ambassadorProfile.UserId);
            modelBuilder.Entity<AmbassadorProfile>()
                .Property(ambassadorProfile => ambassadorProfile.UserId)
                .ValueGeneratedNever();

            modelBuilder.Entity<UserSpinData>()
                .HasKey(userSpinData => userSpinData.UserId);
            modelBuilder.Entity<UserSpinData>()
                .Property(userSpinData => userSpinData.UserId)
                .ValueGeneratedNever();

            modelBuilder.Entity<MarathonProgress>()
                .HasKey(marathonProgress => new { marathonProgress.UserId, marathonProgress.MarathonId });

            modelBuilder.Entity<PriceWatcher>()
                .HasKey(priceWatcher => priceWatcher.EventId);
            modelBuilder.Entity<PriceWatcher>()
                .Property(priceWatcher => priceWatcher.EventId)
                .ValueGeneratedNever();

            // Cascade Delete

            modelBuilder.Entity<Transaction>()
                .HasOne(transaction => transaction.Buyer)
                .WithMany(user => user.Purchases)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(transaction => transaction.Seller)
                .WithMany(user => user.Sales)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MovieReview>()
                .HasOne(movieReview => movieReview.User)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserReelInteraction>()
                .HasOne(userReelInteraction => userReelInteraction.User)
                .WithMany(user => user.ReelInteractions)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OwnedMovie>()
                .HasOne(ownedMovie => ownedMovie.User)
                .WithMany(user => user.OwnedMovies)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OwnedTicket>()
                .HasOne(ownedTicket => ownedTicket.User)
                .WithMany(user => user.OwnedTickets)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserMoviePreference>()
                .HasOne(userMoviePreference => userMoviePreference.User)
                .WithMany(user => user.MoviePreferences)
                .OnDelete(DeleteBehavior.Restrict);

            // Review
            modelBuilder.Entity<Review>()
                .HasOne(review => review.User)
                .WithMany(user => user.Reviews)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(review => review.Movie)
                .WithMany(movie => movie.Reviews)
                .OnDelete(DeleteBehavior.Restrict);

            // Comment
            modelBuilder.Entity<Comment>()
                .HasOne(comment => comment.ParentComment)
                .WithMany(comment => comment.Replies)
                .HasForeignKey(comment => comment.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(comment => comment.Author)
                .WithMany(user => user.Comments)
                .HasForeignKey(comment => comment.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(comment => comment.Movie)
                .WithMany(movie => movie.Comments)
                .HasForeignKey(comment => comment.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            // BattleBet
            modelBuilder.Entity<BattleBet>()
                .HasOne(battleBet => battleBet.User)
                .WithMany(user => user.Bets)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BattleBet>()
                .HasOne(battleBet => battleBet.Battle)
                .WithMany(battle => battle.Bets)
                .OnDelete(DeleteBehavior.Restrict);

            // ReferralLog
            modelBuilder.Entity<ReferralLog>()
                .HasOne(referralLog => referralLog.Ambassador)
                .WithMany()
                .HasForeignKey(referralLog => referralLog.AmbassadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReferralLog>()
                .HasOne(referralLog => referralLog.ReferredUser)
                .WithMany()
                .HasForeignKey(referralLog => referralLog.ReferredUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking
            modelBuilder.Entity<Booking>()
                .HasIndex(booking => new { booking.ScreeningId, booking.Row, booking.Column })
                .IsUnique();

            // UserBadge
            modelBuilder.Entity<UserBadge>()
                .HasOne(userBadge => userBadge.User)
                .WithMany(user => user.UserBadges)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserBadge>()
                .HasOne(userBadge => userBadge.Badge)
                .WithMany(badge => badge.UserBadges)
                .OnDelete(DeleteBehavior.Restrict);

            // UserStats: 1-to-1 with User
            modelBuilder.Entity<UserStats>()
                .HasOne(userStats => userStats.User)
                .WithOne(user => user.UserStats)
                .HasForeignKey<UserStats>(userStats => userStats.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserSpinData
            modelBuilder.Entity<UserSpinData>()
                .HasKey(userSpinData => userSpinData.UserId);

            modelBuilder.Entity<UserSpinData>()
                .Property(userSpinData => userSpinData.UserId)
                .ValueGeneratedNever();

            modelBuilder.Entity<UserSpinData>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<UserSpinData>(usd => usd.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // MarathonProgress
            modelBuilder.Entity<MarathonProgress>()
                .HasOne(marathonProgress => marathonProgress.User)
                .WithMany()
                .HasForeignKey(marathonProgress => marathonProgress.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Decimal Precisions 

            modelBuilder.Entity<User>()
                .Property(user => user.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Movie>()
                .Property(movie => movie.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Movie>()
                .Property(movie => movie.ActiveSaleDiscountPercent)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Equipment>()
                .Property(equipment => equipment.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<MovieEvent>()
                .Property(movieEvent => movieEvent.TicketPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Movie>()
                .Property(movie => movie.Rating)
                .HasPrecision(3, 1);

            modelBuilder.Entity<ActiveSale>()
                .Property(activeSale => activeSale.DiscountPercentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Transaction>()
                .Property(transaction => transaction.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<MusicTrack>()
                .Property(musicTrack => musicTrack.DurationSeconds)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Reel>()
                .Property(reel => reel.FeatureDurationSeconds)
                .HasPrecision(18, 2);

            modelBuilder.Entity<UserMoviePreference>()
                .Property(userMoviePreference => userMoviePreference.Score)
                .HasPrecision(8, 4);

            modelBuilder.Entity<UserProfile>()
                .Property(userProfile => userProfile.AverageWatchTimeSeconds)
                .HasPrecision(18, 2);

            modelBuilder.Entity<UserProfile>()
                .Property(userProfile => userProfile.LikeToViewRatio)
                .HasPrecision(8, 4);

            modelBuilder.Entity<UserReelInteraction>()
                .Property(userReelInteraction => userReelInteraction.WatchDurationSeconds)
                .HasPrecision(18, 2);

            modelBuilder.Entity<UserReelInteraction>()
                .Property(userReelInteraction => userReelInteraction.WatchPercentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<MovieReview>()
                .Property(movieReview => movieReview.StarRating)
                .HasPrecision(3, 1);

            modelBuilder.Entity<Event>()
                .Property(cinemaEvent => cinemaEvent.TicketPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PriceWatcher>()
                .Property(priceWatcher => priceWatcher.TargetPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Review>()
                .Property(review => review.StarRating)
                .HasColumnType("decimal(3,1)");

            modelBuilder.Entity<Reward>()
                .HasKey(r => r.RewardId);

            modelBuilder.Entity<Reward>()
                .Property(r => r.DiscountValue)
                .HasPrecision(5, 2);
        }
    }
}

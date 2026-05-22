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

        // Ambassador & Referrals
        public DbSet<AmbassadorProfile> AmbassadorProfiles { get; set; }
        public DbSet<ReferralLog> ReferralLogs { get; set; }

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
                .HasKey(ap => ap.UserId);

            modelBuilder.Entity<UserSpinData>()
                .HasKey(usd => usd.UserId);

            modelBuilder.Entity<MarathonProgress>()
                .HasKey(mp => new { mp.UserId, mp.MarathonId });

            modelBuilder.Entity<PriceWatcher>()
                .HasKey(pw => pw.EventId);
            modelBuilder.Entity<PriceWatcher>()
                .Property(pw => pw.EventId)
                .ValueGeneratedNever();

            // Cascade Delete

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Buyer)
                .WithMany(u => u.Purchases)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Seller)
                .WithMany(u => u.Sales)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MovieReview>()
                .HasOne(mr => mr.User)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserReelInteraction>()
                .HasOne(uri => uri.User)
                .WithMany(u => u.ReelInteractions)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OwnedMovie>()
                .HasOne(om => om.User)
                .WithMany(u => u.OwnedMovies)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OwnedTicket>()
                .HasOne(ot => ot.User)
                .WithMany(u => u.OwnedTickets)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserMoviePreference>()
                .HasOne(ump => ump.User)
                .WithMany(u => u.MoviePreferences)
                .OnDelete(DeleteBehavior.Restrict);

            // Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Reviews)
                .OnDelete(DeleteBehavior.Restrict);

            // Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Movie)
                .WithMany(m => m.Comments)
                .HasForeignKey(c => c.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            // BattleBet
            modelBuilder.Entity<BattleBet>()
                .HasOne(bb => bb.User)
                .WithMany(u => u.Bets)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BattleBet>()
                .HasOne(bb => bb.Battle)
                .WithMany(b => b.Bets)
                .OnDelete(DeleteBehavior.Restrict);

            // ReferralLog
            modelBuilder.Entity<ReferralLog>()
                .HasOne(rl => rl.Ambassador)
                .WithMany()
                .HasForeignKey(rl => rl.AmbassadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReferralLog>()
                .HasOne(rl => rl.ReferredUser)
                .WithMany()
                .HasForeignKey(rl => rl.ReferredUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking
            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.ScreeningId, b.Row, b.Column })
                .IsUnique();

            // UserBadge
            modelBuilder.Entity<UserBadge>()
                .HasOne(ub => ub.User)
                .WithMany(u => u.UserBadges)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserBadge>()
                .HasOne(ub => ub.Badge)
                .WithMany(b => b.UserBadges)
                .OnDelete(DeleteBehavior.Restrict);

            // UserStats: 1-to-1 with User
            modelBuilder.Entity<UserStats>()
                .HasOne(us => us.User)
                .WithOne(u => u.UserStats)
                .HasForeignKey<UserStats>(us => us.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // MarathonProgress
            modelBuilder.Entity<MarathonProgress>()
                .HasOne(mp => mp.User)
                .WithMany()
                .HasForeignKey(mp => mp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Decimal Precisions 

            modelBuilder.Entity<User>()
                .Property(u => u.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Movie>()
                .Property(m => m.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Movie>()
                .Property(m => m.ActiveSaleDiscountPercent)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Equipment>()
                .Property(e => e.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<MovieEvent>()
                .Property(me => me.TicketPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Movie>()
                .Property(m => m.Rating)
                .HasPrecision(3, 1);

            modelBuilder.Entity<ActiveSale>()
                .Property(a => a.DiscountPercentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<MusicTrack>()
                .Property(mt => mt.DurationSeconds)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Reel>()
                .Property(r => r.FeatureDurationSeconds)
                .HasPrecision(18, 2);

            modelBuilder.Entity<UserMoviePreference>()
                .Property(p => p.Score)
                .HasPrecision(8, 4);

            modelBuilder.Entity<UserProfile>()
                .Property(p => p.AverageWatchTimeSeconds)
                .HasPrecision(18, 2);

            modelBuilder.Entity<UserProfile>()
                .Property(p => p.LikeToViewRatio)
                .HasPrecision(8, 4);

            modelBuilder.Entity<UserReelInteraction>()
                .Property(i => i.WatchDurationSeconds)
                .HasPrecision(18, 2);

            modelBuilder.Entity<UserReelInteraction>()
                .Property(i => i.WatchPercentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<MovieReview>()
                .Property(mr => mr.StarRating)
                .HasPrecision(3, 1);

            modelBuilder.Entity<Event>()
                .Property(ev => ev.TicketPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PriceWatcher>()
                .Property(pw => pw.TargetPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Review>()
                .Property(r => r.StarRating)
                .HasColumnType("decimal(3,1)");
        }
    }
}

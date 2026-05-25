using Microsoft.EntityFrameworkCore;
using MovieApp.DataLayer.Models;

namespace MovieApp.DataLayer.Interfaces
{
    public interface IMovieAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Movie> Movies { get; }
        DbSet<Genre> Genres { get; }
        DbSet<Actor> Actors { get; }
        DbSet<Director> Directors { get; }
        DbSet<Equipment> Equipment { get; }
        DbSet<MovieEvent> MovieEvents { get; }
        DbSet<ActiveSale> ActiveSales { get; }
        DbSet<MovieReview> MovieReviews { get; }
        DbSet<Transaction> Transactions { get; }
        DbSet<OwnedMovie> OwnedMovies { get; }
        DbSet<OwnedTicket> OwnedTickets { get; }
        DbSet<PriceWatcher> PriceWatchers { get; }
        DbSet<Reel> Reels { get; }
        DbSet<MusicTrack> MusicTracks { get; }
        DbSet<ScrapeJob> ScrapeJobs { get; }
        DbSet<ScrapeJobLog> ScrapeJobLogs { get; }
        DbSet<UserMoviePreference> UserMoviePreferences { get; }
        DbSet<UserProfile> UserProfiles { get; }
        DbSet<UserReelInteraction> UserReelInteractions { get; }
        DbSet<Review> Reviews { get; }
        DbSet<Comment> Comments { get; }
        DbSet<Event> Events { get; }
        DbSet<Screening> Screenings { get; }
        DbSet<Booking> Bookings { get; }
        DbSet<Notification> Notifications { get; }
        DbSet<Battle> Battles { get; }
        DbSet<BattleBet> BattleBets { get; }
        DbSet<Badge> Badges { get; }
        DbSet<UserBadge> UserBadges { get; }
        DbSet<UserStats> UserStats { get; }
        DbSet<UserSpinData> UserSpinData { get; }
        DbSet<TriviaQuestion> TriviaQuestions { get; }
        DbSet<TriviaReward> TriviaRewards { get; }
        DbSet<Marathon> Marathons { get; }
        DbSet<MarathonProgress> MarathonProgressions { get; }
        DbSet<FavoriteEvent> FavoriteEvents { get; }
        DbSet<AmbassadorProfile> AmbassadorProfiles { get; }
        DbSet<ReferralLog> ReferralLogs { get; }

        DbSet<Reward> Rewards { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

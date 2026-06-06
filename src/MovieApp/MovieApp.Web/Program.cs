using MovieApp.Logic.Features.Battles;
using MovieApp.Logic.Features.MovieSwipe;
using MovieApp.Logic.Features.MovieTournament;
using MovieApp.Logic.Features.PersonalityMatch;
using MovieApp.Logic.Features.ReelsEditing;
using MovieApp.Logic.Features.ReelsFeed;
using MovieApp.Logic.Features.ReelsUpload;
using MovieApp.Logic.Features.TrailerScraping;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Proxy;
using MovieApp.Proxy.Services;
using MovieApp.Web.Auth;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<MovieApp.Web.Filters.UnauthorizedApiFilter>();
})
    .ConfigureApplicationPartManager(apm =>
    {
        // The MVC project references MovieApp.WebApi for types, but we must NOT let
        // ASP.NET Core discover WebApi controllers in this MVC app — they need
        // different DI registrations and would cause routing conflicts.
        var toRemove = apm.ApplicationParts
            .OfType<Microsoft.AspNetCore.Mvc.ApplicationParts.AssemblyPart>()
            .Where(p => p.Assembly.GetName().Name == "MovieApp.WebApi")
            .ToList();
        foreach (var part in toRemove)
            apm.ApplicationParts.Remove(part);
    });

// HTTP client for auto-login and for ApiClient
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<ApiClient>(httpClient =>
    httpClient.BaseAddress = new Uri(config["WebApi:BaseUrl"]!));

// Per-request token store backed by ISession
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<JwtTokenStore>();
builder.Services.AddScoped<IAuthTokenProvider>(serviceProvider => serviceProvider.GetRequiredService<JwtTokenStore>());
builder.Services.AddScoped<ICurrentUserService>(serviceProvider => serviceProvider.GetRequiredService<JwtTokenStore>());

// Proxy services
builder.Services.AddTransient<IBattleService, BattleProxyService>();
builder.Services.AddTransient<IReelService, ReelProxyService>();
builder.Services.AddTransient<IAudioLibraryService, AudioLibraryProxyService>();
builder.Services.AddTransient<IMovieService, MovieProxyService>();
builder.Services.AddTransient<IEquipmentService, EquipmentProxyService>();
builder.Services.AddTransient<IEventService, EventProxyService>();
builder.Services.AddTransient<IInventoryService, InventoryProxyService>();
builder.Services.AddTransient<IReviewService, ReviewProxyService>();
builder.Services.AddTransient<IExternalReviewService, ExternalReviewProxyService>();
builder.Services.AddTransient<IActiveSalesService, ActiveSalesProxyService>();
builder.Services.AddTransient<IProfileService, ProfileProxyService>();
builder.Services.AddTransient<IPersonalityMatchService, PersonalityMatchProxyService>();
builder.Services.AddTransient<IMovieCardFeedService, MovieCardFeedProxyService>();
builder.Services.AddTransient<ISwipeService, SwipeProxyService>();
builder.Services.AddTransient<IPersonalityMatchingService, PersonalityMatchingProxyService>();
builder.Services.AddTransient<IRecommendationService, RecommendationProxyService>();
builder.Services.AddTransient<IReelInteractionService, ReelInteractionProxyService>();
builder.Services.AddTransient<IVideoProcessingService, VideoProcessingProxyService>();
builder.Services.AddTransient<IVideoStorageService, VideoStorageProxyService>();
builder.Services.AddTransient<IVideoIngestionService, VideoIngestionProxyService>();
builder.Services.AddTransient<ITournamentLogicService, TournamentLogicProxyService>();
builder.Services.AddTransient<IMovieTournamentService, MovieTournamentProxyService>();
builder.Services.AddTransient<INotificationService, NotificationProxyService>();
builder.Services.AddTransient<ISlotMachineService, SlotMachineProxyService>();
builder.Services.AddTransient<IScreeningService, ScreeningProxyService>();
builder.Services.AddTransient<IBookingService, BookingProxyService>();
builder.Services.AddTransient<IReferralLogService, ReferralProxyService>();
builder.Services.AddTransient<IReferralValidator, ReferralProxyService>();
builder.Services.AddTransient<IReferralCodeGenerator, ReferralProxyService>();
builder.Services.AddTransient<ICommentService, CommentProxyService>();
builder.Services.AddTransient<IAmbassadorService, AmbassadorProxyService>();
builder.Services.AddTransient<IMarathonService, MarathonProxyService>();
builder.Services.AddTransient<IUserService, UserProxyService>();
builder.Services.AddTransient<IBadgeService, BadgeProxyService>();
builder.Services.AddTransient<IUserStatsService, UserStatsProxyService>();
builder.Services.AddTransient<ITriviaService, TriviaProxyService>();
builder.Services.AddTransient<ITriviaRepository, TriviaRepositoryProxy>();
builder.Services.AddTransient<ITriviaRewardRepository, TriviaRewardRepositoryProxy>();
builder.Services.AddTransient<IPriceWatcherService, PriceWatcherProxyService>();
builder.Services.AddTransient<IPointService, PointProxyService>();



// Cache and session
builder.Services.AddMemoryCache();
builder.Services.AddSession();

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

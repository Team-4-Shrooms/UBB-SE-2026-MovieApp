using MovieApp.DataLayer.Models;

namespace MovieApp.Logic.Interfaces.Services;

/// <summary>
/// Defines business logic operations for the ambassador referral program.
/// </summary>
public interface IAmbassadorService
{
    /// <summary>
    /// Checks whether a referral code exists.
    /// </summary>
    Task<bool> IsReferralCodeValidAsync(string referralCode, CancellationToken ct = default);

    /// <summary>
    /// Gets all ambassador profiles.
    /// </summary>
    Task<IEnumerable<AmbassadorProfile>> GetAllAmbassadorsAsync(CancellationToken ct = default);

    /// <summary>
    /// Gets an ambassador profile by ID.
    /// </summary>
    Task<AmbassadorProfile?> GetAmbassadorByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Gets the ambassador referral code for a user, or null if not an ambassador.
    /// </summary>
    Task<string?> GetReferralCodeAsync(int userId, CancellationToken ct = default);

    /// <summary>
    /// Enrolls a user as an ambassador with a generated referral code.
    /// </summary>
    Task CreateAmbassadorProfileAsync(int userId, string referralCode, CancellationToken ct = default);

    /// <summary>
    /// Processes a referral interaction when a friend joins an event using a referral code.
    /// </summary>
    Task ProcessReferralAsync(string referralCode, int friendId, int eventId, CancellationToken ct = default);

    /// <summary>
    /// Gets the referral history for a specific ambassador.
    /// </summary>
    Task<IEnumerable<ReferralHistoryItem>> GetReferralHistoryAsync(int ambassadorId, CancellationToken ct = default);

    /// <summary>
    /// Gets the current reward balance for a user.
    /// </summary>
    Task<int> GetRewardBalanceAsync(int userId, CancellationToken ct = default);

    /// <summary>
    /// Redeems one reward from the user's balance.
    /// </summary>
    Task RedeemRewardAsync(int userId, CancellationToken ct = default);
}

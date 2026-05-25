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
    Task<bool> IsReferralCodeValidAsync(string referralCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the ambassador referral code for a user, or null if not an ambassador.
    /// </summary>
    Task<string?> GetReferralCodeAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enrolls a user as an ambassador with a generated referral code.
    /// </summary>
    Task CreateAmbassadorProfileAsync(int userId, string referralCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes a referral interaction when a friend joins an event using a referral code.
    /// </summary>
    Task ProcessReferralAsync(string referralCode, int friendId, int eventId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the referral history for a specific ambassador.
    /// </summary>
    Task<IEnumerable<ReferralHistoryItem>> GetReferralHistoryAsync(int ambassadorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current reward balance for a user.
    /// </summary>
    Task<int> GetRewardBalanceAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Redeems one reward from the user's balance.
    /// </summary>
    Task RedeemRewardAsync(int userId, CancellationToken cancellationToken = default);

    Task<int?> ResolveCodeToUserIdAsync(string referralCode, CancellationToken cancellationToken = default);

    Task<bool> ReferralLogExistsAsync(int ambassadorId, int friendId, int eventId, CancellationToken cancellationToken = default);

    Task LogReferralByAmbassadorIdAsync(int ambassadorId, int friendId, int eventId, CancellationToken cancellationToken = default);

    Task DecrementRewardBalanceAsync(int userId, CancellationToken cancellationToken = default);
}

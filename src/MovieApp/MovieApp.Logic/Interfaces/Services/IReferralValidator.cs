namespace MovieApp.Logic.Interfaces.Services
{
    public interface IReferralValidator
    {
        Task<bool> IsValidReferralAsync(string referralCode, int currentUserIdentifier, CancellationToken cancellationToken = default);

        Task<bool> IsValidReferralForEventAsync(string referralCode, int currentUserIdentifier, int eventIdentifier, CancellationToken cancellationToken = default);
    }
}

namespace MovieApp.Logic.Interfaces.Services
{
    public interface IReferralLogService
    {
        Task LogReferralUsageAsync(string referralCode, int friendIdentifier, int eventIdentifier, CancellationToken cancellationToken = default);
    }
}

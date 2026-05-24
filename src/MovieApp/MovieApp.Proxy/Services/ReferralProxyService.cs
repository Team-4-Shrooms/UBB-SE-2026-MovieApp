using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MovieApp.Logic.Interfaces.Services;

namespace MovieApp.Proxy.Services;

public sealed class ReferralProxyService : IReferralLogService, IReferralValidator, IReferralCodeGenerator
{
    private const int GeneratedCodeLength = 8;
    private static readonly char[] CodeAlphabet =
        "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray();

    private readonly ApiClient _apiClient;

    public ReferralProxyService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task LogReferralUsageAsync(
        string referralCode,
        int friendIdentifier,
        int eventIdentifier,
        CancellationToken cancellationToken = default)
    {
        int? ambassadorId = await _apiClient.GetAsync<int?>(
            $"api/referrals/code/{referralCode}/user",
            cancellationToken);

        if (ambassadorId is null)
        {
            return;
        }

        await _apiClient.PostAsync(
            "api/referrals/log",
            new
            {
                AmbassadorId = ambassadorId.Value,
                FriendId = friendIdentifier,
                EventId = eventIdentifier,
            },
            cancellationToken);
    }

    public async Task<bool> IsValidReferralAsync(
        string referralCode,
        int currentUserIdentifier,
        CancellationToken cancellationToken = default)
    {
        bool isValid = await _apiClient.GetAsync<bool>(
            $"api/referrals/validate?code={Uri.EscapeDataString(referralCode)}&currentUserId={currentUserIdentifier}",
            cancellationToken);
        return isValid;
    }

    public async Task<bool> IsValidReferralForEventAsync(
        string referralCode,
        int currentUserIdentifier,
        int eventIdentifier,
        CancellationToken cancellationToken = default)
    {
        bool baseValid = await IsValidReferralAsync(referralCode, currentUserIdentifier, cancellationToken);
        if (!baseValid)
        {
            return false;
        }

        int? ambassadorId = await _apiClient.GetAsync<int?>(
            $"api/referrals/code/{referralCode}/user",
            cancellationToken);

        if (ambassadorId is null)
        {
            return false;
        }

        bool alreadyLogged = await _apiClient.GetAsync<bool>(
            $"api/referrals/check?ambassadorId={ambassadorId.Value}&friendId={currentUserIdentifier}&eventId={eventIdentifier}",
            cancellationToken);

        return !alreadyLogged;
    }

    public string Generate(string username, int userIdentifier)
    {
        string seed = string.Concat(
            username ?? string.Empty,
            "|",
            userIdentifier.ToString(CultureInfo.InvariantCulture));

        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(seed));

        char[] code = new char[GeneratedCodeLength];
        for (int index = 0; index < GeneratedCodeLength; index++)
        {
            code[index] = CodeAlphabet[hash[index] % CodeAlphabet.Length];
        }

        return new string(code);
    }
}

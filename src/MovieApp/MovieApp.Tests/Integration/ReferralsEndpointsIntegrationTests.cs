using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MovieApp.Tests.Integration.ProxyRepos;

namespace MovieApp.Tests.Integration.Endpoints;

public sealed class ReferralsEndpointsIntegrationTests
    : IClassFixture<MovieAppWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ReferralsEndpointsIntegrationTests(MovieAppWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test", "integration-test");
    }

    [Fact]
    public async Task ValidateReferralCode_OwnCode_ReturnsFalse()
    {
        int userId = 1;
        string ownCode = $"OWN-CODE-{Guid.NewGuid():N}";

        await UpsertAmbassadorProfileAsync(
            userId,
            ownCode,
            rewardBalance: 0);

        HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/referrals/validate?code={ownCode}&currentUserId={userId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        bool result = await response.Content.ReadFromJsonAsync<bool>();

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateReferralCode_ValidOtherUserCode_ReturnsTrue()
    {
        int ambassadorId = 10;
        int currentUserId = 20;

        string referralCode = $"VALID-CODE-{Guid.NewGuid():N}";

        await UpsertAmbassadorProfileAsync(
            ambassadorId,
            referralCode,
            rewardBalance: 0);

        HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/referrals/validate?code={referralCode}&currentUserId={currentUserId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        bool result = await response.Content.ReadFromJsonAsync<bool>();

        Assert.True(result);
    }

    [Fact]
    public async Task LogReferral_ValidReferral_ReturnsOk()
    {
        int ambassadorId = 30;
        int friendId = 40;
        int eventId = 1;

        string ambassadorCode = $"AMB-CODE-{Guid.NewGuid():N}";

        await UpsertAmbassadorProfileAsync(
            ambassadorId,
            ambassadorCode,
            rewardBalance: 0);

        HttpResponseMessage logResponse =
            await _client.PostAsJsonAsync(
                "/api/referrals/log",
                new
                {
                    AmbassadorId = ambassadorId,
                    FriendId = friendId,
                    EventId = eventId
                });

        Assert.Equal(HttpStatusCode.OK, logResponse.StatusCode);
    }

    private async Task UpsertAmbassadorProfileAsync(
        int userId,
        string referralCode,
        int rewardBalance)
    {
        await _client.PostAsJsonAsync(
            "/api/referrals/profile",
            new
            {
                UserId = userId,
                Code = referralCode
            });

        await _client.PostAsJsonAsync(
            "/api/referrals/user/" + userId + "/balance/decrement",
            new { });

        if (rewardBalance > 0)
        {
            for (int index = 0; index < rewardBalance; index++)
            {
                await _client.PostAsJsonAsync(
                    "/api/referrals/log",
                    new
                    {
                        AmbassadorId = userId,
                        FriendId = 1000 + index,
                        EventId = 2000 + index
                    });
            }
        }
    }
}

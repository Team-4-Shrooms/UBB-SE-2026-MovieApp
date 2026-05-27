using System.Net;
using System.Text.Json;

namespace MovieApp.Tests.Integration;

public sealed class AmbassadorEndpointsIntegrationTests
    : IClassFixture<MovieAppWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public AmbassadorEndpointsIntegrationTests(MovieAppWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllAmbassadors_ReturnsOkWithArray()
    {
        HttpResponseMessage response = await _httpClient.GetAsync("/api/ambassadors");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();
        using JsonDocument document = JsonDocument.Parse(content);

        Assert.Equal(JsonValueKind.Array, document.RootElement.ValueKind);
    }
}

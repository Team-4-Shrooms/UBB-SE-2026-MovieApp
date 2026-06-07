using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.Tests.Integration.Endpoints;

public sealed class BattlesIntegrationTests
{
    [Fact]
    public async Task GetBattleById_NonexistentBattle_ReturnsNotFound()
    {
        using WebApplicationFactory<Program> factory =
            CreateFactoryWithFakeBattleService();

        HttpClient client = CreateAuthorizedClient(factory);

        HttpResponseMessage response =
            await client.GetAsync("/api/battles/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static WebApplicationFactory<Program> CreateFactoryWithFakeBattleService()
    {
        return new MovieAppWebApplicationFactory()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IBattleService>();
                    services.AddScoped<IBattleService, FakeBattleService>();
                });
            });
    }

    private static HttpClient CreateAuthorizedClient(
        WebApplicationFactory<Program> factory)
    {
        HttpClient client = factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test", "integration-test");

        return client;
    }

    private sealed class FakeBattleService : IBattleService
    {
        public Task<IEnumerable<Battle>> GetBattlesAsync(
            CancellationToken cancellationToken = default)
        {
            IEnumerable<Battle> battles = new List<Battle>
            {
                new Battle
                {
                    BattleId = 1,
                    Status = "Active",
                    FirstMovie = new Movie { Id = 1, Title = "Movie 1" },
                    SecondMovie = new Movie { Id = 2, Title = "Movie 2" }
                }
            };

            return Task.FromResult(battles);
        }

        public Task<Battle?> GetBattleByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id == 999999)
            {
                return Task.FromResult<Battle?>(null);
            }

            Battle battle = new Battle
            {
                BattleId = id,
                Status = "Active",
                FirstMovie = new Movie { Id = 1, Title = "Movie 1" },
                SecondMovie = new Movie { Id = 2, Title = "Movie 2" }
            };

            return Task.FromResult<Battle?>(battle);
        }

        public Task<BattleBet> PlaceBetAsync(
            int userId,
            int battleId,
            int movieId,
            int amount,
            CancellationToken cancellationToken = default)
        {
            BattleBet bet = new BattleBet
            {
                BattleBetId = 1,
                Amount = amount,
                Battle = new Battle { BattleId = battleId },
                Movie = new Movie { Id = movieId },
                User = new User { Id = userId }
            };

            return Task.FromResult(bet);
        }

        public Task<Battle?> GetActiveBattleAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<Battle?>(null);

        public Task<Battle> CreateBattleAsync(int firstMovieId, int secondMovieId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<BattleBet?> GetBetAsync(int userId, int battleId, CancellationToken cancellationToken = default)
            => Task.FromResult<BattleBet?>(null);

        public Task<int> DetermineWinnerAsync(int battleId, CancellationToken cancellationToken = default)
            => Task.FromResult(1);

        public Task DistributePayoutsAsync(int battleId, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task SettleExpiredBattlesAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<Battle?> GetCurrentBattleForUserAsync(int userId, CancellationToken cancellationToken = default)
            => Task.FromResult<Battle?>(null);

        public Task ForceSettleBattleAsync(int battleId, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task ResetAllBattlesForDemoAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<Battle> CreateDemoBattleAsync(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }
}

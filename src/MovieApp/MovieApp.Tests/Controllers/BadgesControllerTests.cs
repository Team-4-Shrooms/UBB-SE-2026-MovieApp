using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebApi.DTOs;
using MovieApp.WebApi.Endpoints;

namespace MovieApp.Tests.Controllers;

public sealed class BadgesControllerTests
{

    private sealed class FakeBadgeService : IBadgeService
    {
        public IList<UserStats> LeaderboardResult { get; init; } = new List<UserStats>();
        public List<UserBadge> UserBadgesResult { get; init; } = new List<UserBadge>();
        public List<Badge> AllBadgesResult { get; init; } = new List<Badge>();

        public Task<List<UserBadge>> GetUserBadgesAsync(int userId, CancellationToken ct = default)
            => Task.FromResult(UserBadgesResult);

        public Task<List<Badge>> GetAllBadgesAsync(CancellationToken ct = default)
            => Task.FromResult(AllBadgesResult);

        public Task<IList<UserStats>> GetLeaderboardAsync(CancellationToken ct = default)
            => Task.FromResult(LeaderboardResult);

        public Task CheckAndAwardBadgesAsync(int userId, CancellationToken ct = default)
            => Task.CompletedTask;
    }

    [Fact]
    public async Task GetLeaderboard_ServiceReturnsStats_ReturnsOkResult()
    {
        FakeBadgeService service = new()
        {
            LeaderboardResult = new List<UserStats>
            {
                new() { UserId = 1, TotalPoints = 100, WeeklyScore = 20 },
                new() { UserId = 2, TotalPoints = 50,  WeeklyScore = 10 },
            },
        };
        BadgesController controller = new(service);

        IActionResult result = await controller.GetLeaderboard();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetLeaderboard_ServiceReturnsTwoEntries_ResultContainsTwoEntries()
    {
        FakeBadgeService service = new()
        {
            LeaderboardResult = new List<UserStats>
            {
                new() { UserId = 1, TotalPoints = 100 },
                new() { UserId = 2, TotalPoints = 50  },
            },
        };
        BadgesController controller = new(service);

        IActionResult result = await controller.GetLeaderboard();

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        List<UserStatsDto> dtos = Assert.IsType<List<UserStatsDto>>(ok.Value);
        Assert.Equal(2, dtos.Count);
    }

    [Fact]
    public async Task GetLeaderboard_ServiceReturnsDescendingOrder_ControllerPreservesOrder()
    {
        FakeBadgeService service = new()
        {
            LeaderboardResult = new List<UserStats>
            {
                new() { UserId = 1, TotalPoints = 200 },
                new() { UserId = 2, TotalPoints = 150 },
                new() { UserId = 3, TotalPoints = 50  },
            },
        };
        BadgesController controller = new(service);

        IActionResult result = await controller.GetLeaderboard();

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        List<UserStatsDto> dtos = Assert.IsType<List<UserStatsDto>>(ok.Value);
        for (int i = 0; i + 1 < dtos.Count; i++)
        {
            Assert.True(
                dtos[i].TotalPoints >= dtos[i + 1].TotalPoints,
                $"Entry {i} has TotalPoints {dtos[i].TotalPoints} but entry {i + 1} has {dtos[i + 1].TotalPoints}");
        }
    }

    [Fact]
    public async Task GetUserBadges_ServiceReturnsEmpty_ReturnsOkWithEmptyList()
    {
        FakeBadgeService service = new();
        BadgesController controller = new(service);

        IActionResult result = await controller.GetUserBadges(userId: 1);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        List<UserBadgeDto> dtos = Assert.IsType<List<UserBadgeDto>>(ok.Value);
        Assert.Empty(dtos);
    }

    [Fact]
    public async Task GetUserBadges_ServiceReturnsBadge_ReturnsOkWithOneBadge()
    {
        Badge badge = new() { BadgeId = 1, Name = "The Snob", Description = "Write 10+ reviews.", CriteriaValue = 10 };
        FakeBadgeService service = new()
        {
            UserBadgesResult = new List<UserBadge>
            {
                new() { UserBadgeId = 1, Badge = badge, User = new User { Id = 1 } },
            },
        };
        BadgesController controller = new(service);

        IActionResult result = await controller.GetUserBadges(userId: 1);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        List<UserBadgeDto> dtos = Assert.IsType<List<UserBadgeDto>>(ok.Value);
        Assert.Single(dtos);
        Assert.Equal("The Snob", dtos[0].Badge!.Name);
    }


    [Fact]
    public async Task CheckAndAwardBadges_WhenCalled_ReturnsNoContent()
    {
        FakeBadgeService service = new();
        BadgesController controller = new(service);

        IActionResult result = await controller.CheckAndAwardBadges(userId: 1);

        Assert.IsType<NoContentResult>(result);
    }
}

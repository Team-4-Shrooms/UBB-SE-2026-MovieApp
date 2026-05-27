using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebApi.Endpoints;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MovieApp.Tests.Controllers;

public sealed class MarathonsControllerTests
{
    private readonly Mock<IMarathonService> _marathonServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly MarathonsController _controller;

    public MarathonsControllerTests()
    {
        _marathonServiceMock = new Mock<IMarathonService>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _controller = new MarathonsController(
            _marathonServiceMock.Object,
            _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task GetMarathons_ReturnsOk()
    {
        int userId = 1;

        var marathons = new List<Marathon>();

        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _marathonServiceMock
            .Setup(x => x.GetWeeklyMarathonsAsync(userId))
            .ReturnsAsync(marathons);

        IActionResult result = await _controller.GetMarathons();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(marathons, okResult.Value);
    }

    [Fact]
    public async Task GetMarathon_ReturnsOk()
    {
        int marathonId = 1;

        var movies = new List<Movie>();

        _marathonServiceMock
            .Setup(x => x.GetMoviesForMarathonAsync(marathonId))
            .ReturnsAsync(movies);

        IActionResult result = await _controller.GetMarathon(marathonId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(movies, okResult.Value);
    }

    [Fact]
    public async Task GetWeeklyMarathons_ReturnsOk()
    {
        int userId = 1;

        var marathons = new List<Marathon>();

        _marathonServiceMock
            .Setup(x => x.GetWeeklyMarathonsAsync(userId))
            .ReturnsAsync(marathons);

        IActionResult result = await _controller.GetWeeklyMarathons(userId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(marathons, okResult.Value);
    }

    [Fact]
    public async Task StartMarathon_ReturnsOk_True()
    {
        _marathonServiceMock
            .Setup(x => x.StartMarathonAsync(1))
            .ReturnsAsync(true);

        IActionResult result = await _controller.StartMarathon(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    [Fact]
    public async Task StartMarathon_ReturnsOk_False()
    {
        _marathonServiceMock
            .Setup(x => x.StartMarathonAsync(1))
            .ReturnsAsync(false);

        IActionResult result = await _controller.StartMarathon(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.False((bool)okResult.Value!);
    }

    [Fact]
    public async Task GetProgress_ReturnsNotFound_WhenNull()
    {
        _marathonServiceMock
            .Setup(x => x.GetUserProgressAsync(1, 1))
            .ReturnsAsync((MarathonProgress?)null);

        IActionResult result = await _controller.GetProgress(1, 1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetProgress_ReturnsOk_WhenExists()
    {
        var progress = new MarathonProgress
        {
            UserId = 1,
            MarathonId = 1
        };
        _marathonServiceMock
            .Setup(x => x.GetUserProgressAsync(1, 1))
            .ReturnsAsync(progress);

        IActionResult result = await _controller.GetProgress(1, 1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(progress, okResult.Value);
    }

    [Fact]
    public async Task GetLeaderboard_ReturnsOk()
    {
        var leaderboard = new List<LeaderboardEntry>();

        _marathonServiceMock
            .Setup(x => x.GetLeaderboardWithUsernamesAsync(1))
            .ReturnsAsync(leaderboard);

        IActionResult result = await _controller.GetLeaderboard(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(leaderboard, okResult.Value);
    }

    [Fact]
    public async Task UpdateQuizResult_ReturnsOk()
    {
        _marathonServiceMock
            .Setup(x => x.UpdateQuizResultAsync(1, 5))
            .Returns(Task.CompletedTask);

        IActionResult result = await _controller.UpdateQuizResult(1, 5);

        Assert.IsType<OkResult>(result);

        _marathonServiceMock.Verify(
            x => x.UpdateQuizResultAsync(1, 5),
            Times.Once);
    }

    [Fact]
    public async Task LogMovie_ReturnsOk_True()
    {
        _marathonServiceMock
            .Setup(x => x.LogMovieAsync(1, 2, 3))
            .ReturnsAsync(true);

        IActionResult result = await _controller.LogMovie(1, 2, 3);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    [Fact]
    public async Task LogMovie_ReturnsOk_False()
    {
        _marathonServiceMock
            .Setup(x => x.LogMovieAsync(1, 2, 3))
            .ReturnsAsync(false);

        IActionResult result = await _controller.LogMovie(1, 2, 3);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.False((bool)okResult.Value!);
    }

    [Fact]
    public async Task GetParticipantCount_ReturnsOk()
    {
        _marathonServiceMock
            .Setup(x => x.GetParticipantCountAsync(1))
            .ReturnsAsync(10);

        IActionResult result = await _controller.GetParticipantCount(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(10, okResult.Value);
    }

    [Fact]
    public async Task GetMarathonMovieCount_ReturnsOk()
    {
        _marathonServiceMock
            .Setup(x => x.GetMarathonMovieCountAsync(1))
            .ReturnsAsync(5);

        IActionResult result = await _controller.GetMarathonMovieCount(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(5, okResult.Value);
    }

    [Fact]
    public async Task IsPrerequisiteCompleted_ReturnsOk_True()
    {
        _marathonServiceMock
            .Setup(x => x.IsPrerequisiteCompletedAsync(1, 1))
            .ReturnsAsync(true);

        IActionResult result = await _controller.IsPrerequisiteCompleted(1, 1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    [Fact]
    public async Task IsPrerequisiteCompleted_ReturnsOk_False()
    {
        _marathonServiceMock
            .Setup(x => x.IsPrerequisiteCompletedAsync(1, 1))
            .ReturnsAsync(false);

        IActionResult result = await _controller.IsPrerequisiteCompleted(1, 1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.False((bool)okResult.Value!);
    }
}

using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Logic.Models;
using MovieApp.WebApi.DTOs;
using MovieApp.WebApi.Endpoints;
using MovieApp.WebDTOs.DTOs.RequestDTOs;

namespace MovieApp.Tests.Controllers;

public sealed class SlotMachineControllerTests
{
    private readonly Mock<ISlotMachineService> _slotMachineServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly SlotMachineEndpointsController _controller;

    public SlotMachineControllerTests()
    {
        _slotMachineServiceMock = new Mock<ISlotMachineService>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _controller = new SlotMachineEndpointsController(
            _slotMachineServiceMock.Object,
            _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task GetUserSpinState_ReturnsOk()
    {
        // Arrange
        int userId = 1;

        _slotMachineServiceMock
            .Setup(x => x.GetUserSpinStateAsync(userId))
            .ReturnsAsync(new UserSpinData { UserId = userId, DailySpinsRemaining = 5 });

        // Act
        IActionResult result = await _controller.GetUserSpinState(userId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetAvailableSpins_ReturnsOk()
    {
        // Arrange
        int userId = 1;

        _slotMachineServiceMock
            .Setup(x => x.GetAvailableSpinsAsync(userId))
            .ReturnsAsync(5);

        // Act
        IActionResult result = await _controller.GetAvailableSpins(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal(5, okResult.Value);
    }

    [Fact]
    public async Task Spin_ReturnsOk()
    {
        // Arrange
        int userId = 1;

        _slotMachineServiceMock
            .Setup(x => x.SpinAsync(userId))
            .ReturnsAsync(new SlotMachineResult
            {
                Genre = new Genre { Id = 1, Name = "Sci-Fi" },
                Actor = new Actor { Id = 2, Name = "Actor" },
                Director = new Director { Id = 3, Name = "Director" },
                MatchingEvents = new List<MovieEvent>(),
                JackpotEventIds = new HashSet<int>(),
                JackpotDiscountApplied = false,
                DiscountPercentage = 0,
            });

        // Act
        IActionResult result = await _controller.Spin(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        SlotMachineResultDto dto = Assert.IsType<SlotMachineResultDto>(okResult.Value);

        Assert.Equal(1, dto.Genre.Id);
        Assert.Equal(2, dto.Actor.Id);
        Assert.Equal(3, dto.Director.Id);

        _slotMachineServiceMock.Verify(x => x.SpinAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GrantBonusSpin_ReturnsOk_True()
    {
        // Arrange
        int userId = 1;

        _slotMachineServiceMock
            .Setup(x => x.GrantBonusSpinForEventParticipationAsync(userId))
            .ReturnsAsync(true);

        // Act
        IActionResult result = await _controller.GrantBonusSpin(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.True((bool)okResult.Value!);
    }

    [Fact]
    public async Task GrantBonusSpin_ReturnsOk_False()
    {
        // Arrange
        int userId = 1;

        _slotMachineServiceMock
            .Setup(x => x.GrantBonusSpinForEventParticipationAsync(userId))
            .ReturnsAsync(false);

        // Act
        IActionResult result = await _controller.GrantBonusSpin(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.False((bool)okResult.Value!);
    }

    [Fact]
    public async Task RecordLoginStreak_ReturnsOk_True()
    {
        // Arrange
        int userId = 1;

        _slotMachineServiceMock
            .Setup(x => x.RecordLoginAndCheckStreakAsync(userId))
            .ReturnsAsync(true);

        // Act
        IActionResult result = await _controller.RecordLoginStreak(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.True((bool)okResult.Value!);
    }

    [Fact]
    public async Task RecordLoginStreak_ReturnsOk_False()
    {
        // Arrange
        int userId = 1;

        _slotMachineServiceMock
            .Setup(x => x.RecordLoginAndCheckStreakAsync(userId))
            .ReturnsAsync(false);

        // Act
        IActionResult result = await _controller.RecordLoginStreak(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.False((bool)okResult.Value!);
    }

    [Fact]
    public async Task GrantStreakSpin_ReturnsOk_True()
    {
        // Arrange
        int userId = 1;

        _slotMachineServiceMock
            .Setup(x => x.GrantStreakSpinAsync(userId))
            .ReturnsAsync(true);

        // Act
        IActionResult result = await _controller.GrantStreakSpin(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.True((bool)okResult.Value!);
    }

    [Fact]
    public async Task GrantStreakSpin_ReturnsOk_False()
    {
        // Arrange
        int userId = 1;

        _slotMachineServiceMock
            .Setup(x => x.GrantStreakSpinAsync(userId))
            .ReturnsAsync(false);

        // Act
        IActionResult result = await _controller.GrantStreakSpin(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.False((bool)okResult.Value!);
    }

    [Fact]
    public async Task GetGenres_ReturnsOk()
    {
        // Arrange
        _slotMachineServiceMock
            .Setup(x => x.GetGenresAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Genre>());

        // Act
        IActionResult result = await _controller.GetGenres();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetRandomGenre_ReturnsOk()
    {
        // Arrange
        _slotMachineServiceMock
            .Setup(x => x.GetRandomGenreAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Genre());

        // Act
        IActionResult result = await _controller.GetRandomGenre();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetActors_ReturnsOk()
    {
        // Arrange
        _slotMachineServiceMock
            .Setup(x => x.GetActorsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Actor>());

        // Act
        IActionResult result = await _controller.GetActors();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetRandomActor_ReturnsOk()
    {
        // Arrange
        _slotMachineServiceMock
            .Setup(x => x.GetRandomActorAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Actor());

        // Act
        IActionResult result = await _controller.GetRandomActor();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetDirectors_ReturnsOk()
    {
        // Arrange
        _slotMachineServiceMock
            .Setup(x => x.GetDirectorsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Director>());

        // Act
        IActionResult result = await _controller.GetDirectors();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetRandomDirector_ReturnsOk()
    {
        // Arrange
        _slotMachineServiceMock
            .Setup(x => x.GetRandomDirectorAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Director());

        // Act
        IActionResult result = await _controller.GetRandomDirector();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetMatchingEvents_ReturnsOk()
    {
        // Arrange
        _slotMachineServiceMock
            .Setup(x => x.GetMatchingEventsAsync(1, 2, 3))
            .ReturnsAsync(new List<MovieEvent>());

        // Act
        IActionResult result = await _controller.GetMatchingEvents(1, 2, 3);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task FindJackpotMovie_ReturnsOk()
    {
        // Arrange
        _slotMachineServiceMock
            .Setup(x => x.FindJackpotMovieAsync(1, 2, 3))
            .ReturnsAsync(new Movie());

        // Act
        IActionResult result = await _controller.FindJackpotMovie(1, 2, 3);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GrantJackpotDiscount_ReturnsForbid_WhenUserIdsDoNotMatch()
    {
        // Arrange
        _currentUserServiceMock
            .Setup(x => x.UserId)
            .Returns(1);

        var request = new GrantJackpotDiscountRequestBody
        {
            UserId = 2,
            MovieId = 1
        };

        // Act
        IActionResult result =
            await _controller.GrantJackpotDiscount(request);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }


}

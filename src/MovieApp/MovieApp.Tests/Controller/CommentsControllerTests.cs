using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebApi.DTOs;
using MovieApp.WebApi.Endpoints;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using System.Threading;
using Xunit;

namespace MovieApp.Tests.Controllers;

public sealed class CommentsControllerTests
{
    private readonly Mock<ICommentService> _commentServiceMock;
    private readonly CommentsController _controller;

    public CommentsControllerTests()
    {
        _commentServiceMock = new Mock<ICommentService>();

        _controller = new CommentsController(
            _commentServiceMock.Object);
    }



    [Fact]
    public async Task AddComment_ReturnsBadRequest_WhenRequestIsNull()
    {
        // Act
        IActionResult result =
            await _controller.AddComment(1, null!);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task AddComment_ReturnsBadRequest_WhenContentIsEmpty()
    {
        // Arrange
        var request = new AddCommentRequest
        {
            UserId = 1,
            Content = string.Empty
        };

        // Act
        IActionResult result =
            await _controller.AddComment(1, request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task AddComment_ReturnsOk_WhenRequestIsValid()
    {
        // Arrange
        int movieId = 1;

        var request = new AddCommentRequest
        {
            UserId = 1,
            Content = "Great movie"
        };

        var comment = new Comment
        {
            CommentId = 1,
            AuthorId = request.UserId,
            MovieId = movieId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            Author = new User
            {
                Username = "Ale"
            }
        };

        _commentServiceMock
            .Setup(x =>
                x.AddCommentAsync(
                    request.UserId,
                    movieId,
                    request.Content,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        // Act
        IActionResult result =
            await _controller.AddComment(
                movieId,
                request);

        // Assert
        var okResult =
            Assert.IsType<OkObjectResult>(result);

        var response =
            Assert.IsType<CommentResponseDto>(
                okResult.Value);

        Assert.Equal(
            request.Content,
            response.Content);

        Assert.Equal(
            movieId,
            response.MovieId);
    }

    [Fact]
    public async Task ReplyComment_ReturnsBadRequest_WhenRequestIsNull()
    {
        // Act
        IActionResult result =
            await _controller.ReplyComment(1, null!);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ReplyComment_ReturnsBadRequest_WhenContentIsEmpty()
    {
        // Arrange
        var request = new ReplyCommentRequest
        {
            UserId = 1,
            Content = string.Empty
        };

        // Act
        IActionResult result =
            await _controller.ReplyComment(1, request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ReplyComment_ReturnsOk_WhenRequestIsValid()
    {
        // Arrange
        int parentCommentId = 1;

        var request = new ReplyCommentRequest
        {
            UserId = 1,
            Content = "Reply content"
        };

        var reply = new Comment
        {
            CommentId = 2,
            AuthorId = request.UserId,
            MovieId = 1,
            ParentCommentId = parentCommentId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            Author = new User
            {
                Username = "Ale"
            }
        };

        _commentServiceMock
            .Setup(x =>
                x.AddReplyAsync(
                    request.UserId,
                    parentCommentId,
                    request.Content,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(reply);

        // Act
        IActionResult result =
            await _controller.ReplyComment(
                parentCommentId,
                request);

        // Assert
        var okResult =
            Assert.IsType<OkObjectResult>(result);

        var response =
            Assert.IsType<CommentResponseDto>(
                okResult.Value);

        Assert.Equal(
            parentCommentId,
            response.ParentCommentId);
    }
}

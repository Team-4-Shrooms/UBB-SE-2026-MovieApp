using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.DTOs;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api")]
public sealed class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("movies/{id}/comments")]
    public async Task<IActionResult> GetCommentsForMovie(int id)
    {
        List<Comment> rootComments = await _commentService.GetCommentsForMovieAsync(id);

        List<CommentResponseDto> response = rootComments.Select(MapToDto).ToList();
        return Ok(response);
    }

    [HttpPost("movies/{id}/comments")]
    public async Task<IActionResult> AddComment(int id, [FromBody] AddCommentRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest("Content cannot be empty.");
        }

        Comment comment = await _commentService.AddCommentAsync(request.UserId, id, request.Content);
        return Ok(MapToDto(comment));
    }

    [HttpPost("comments/{id}/reply")]
    public async Task<IActionResult> ReplyComment(int id, [FromBody] ReplyCommentRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest("Content cannot be empty.");
        }

        Comment reply = await _commentService.AddReplyAsync(request.UserId, id, request.Content);
        return Ok(MapToDto(reply));
    }

    private static CommentResponseDto MapToDto(Comment comment)
    {
        return new CommentResponseDto
        {
            CommentId = comment.CommentId,
            AuthorId = comment.AuthorId,
            MovieId = comment.MovieId,
            ParentCommentId = comment.ParentCommentId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            AuthorUsername = comment.Author?.Username ?? string.Empty,
            Replies = comment.Replies?.Select(MapToDto).ToList() ?? new List<CommentResponseDto>()
        };
    }
}

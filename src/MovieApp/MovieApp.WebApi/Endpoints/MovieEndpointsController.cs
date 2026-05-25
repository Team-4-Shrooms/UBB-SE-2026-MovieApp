using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.WebDTOs.DTOs.RequestDTOs;
using MovieApp.WebApi.Mappings;
using MovieApp.Logic.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace MovieApp.WebApi.Endpoints;

[Authorize]
[ApiController]
[Route("api/movies")]
public sealed class MovieEndpointsController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IExternalReviewService _externalReviewService;

    public MovieEndpointsController(IMovieService movieService, IExternalReviewService externalReviewService)
    {
        _movieService = movieService;
        _externalReviewService = externalReviewService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMovies()
    {
        var movies = await _movieService.GetAllMoviesAsync();
        return Ok(movies.Select(movie => movie.ToDto()));
    }

    [HttpGet("{movieId:int}")]
    public async Task<IActionResult> GetMovieById(int movieId)
    {
        var movie = await _movieService.GetMovieByIdAsync(movieId);

        if (movie == null)
        {
            return NoContent();
        }

        return Ok(movie.ToDto());
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTop10MoviesAsync([FromQuery] string? partialMovieName)
    {
        var movies = await _movieService.SearchMoviesAsync(partialMovieName ?? string.Empty);
        return Ok(movies.Select(movie => movie.ToDto()));
    }

    [HttpGet("{movieId:int}/owned/{userId:int}")]
    public async Task<IActionResult> UserOwnsMovie(int movieId, int userId)
    {
        return Ok(await _movieService.UserOwnsMovieAsync(userId, movieId));
    }

    [HttpPost("{id:int}/purchase")]
    public async Task<IActionResult> PurchaseMovie(int id, [FromBody] PurchaseMovieRequestBody body)
    {
        await _movieService.PurchaseMovieAsync(body.UserId, id, body.FinalPrice);
        return Ok();
    }

    [HttpGet("{id:int}/external-reviews")]
    public async Task<IActionResult> GetExternalReviews(int id, CancellationToken cancellationToken)
    {
        var movie = await _movieService.GetMovieByIdAsync(id);

        if (movie == null)
        {
            return NotFound("Movie not found.");
        }

        var reviews = await _externalReviewService.GetExternalReviewsAsync(movie.Title, movie.ReleaseYear, cancellationToken);

        return Ok(reviews);
    }
}

using IMDB.Api.Auth;
using IMDB.Api.Mapping;
using IMDB.Application.Models;
using IMDB.Application.Services;
using IMDB.Contracts.Requests;
using IMDB.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.Api.Controllers;

[ApiController]
public class MoviesController(IMovieService _movieService) : ControllerBase
{
    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPost(ApiEndpoints.Movies.Create)]
    [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
    {
        Movie movie = request.MapToMovie();

        await _movieService.CreateAsync(movie, token);

        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie.MapToResponse());
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        Guid? userId = HttpContext.GetUserId();

        Movie? movie = Guid.TryParse(idOrSlug, out Guid id)
            ? await _movieService.GetByIdAsync(id, userId, token)
            : await _movieService.GetBySlugAsync(idOrSlug, userId, token);
        if (movie is null) return NotFound();

        return Ok(movie.MapToResponse());
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken token)
    {
        Guid? userId = HttpContext.GetUserId();
        GetAllMoviesOptions options = request.MapToOptions()
            .WithUser(userId);
        
        IEnumerable<Movie> movies = await _movieService.GetAllAsync(options, token);
        int movieCount = await _movieService.GetCountAsync(options.Title, options.Year, token);
        MoviesResponse moviesResponse = movies.MapToResponse(options.Page, options.PageSize, movieCount);
        
        return Ok(moviesResponse);
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiEndpoints.Movies.Update)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request,
        CancellationToken token)
    {
        Movie movie = request.MapToMovie(id);
        Guid? userId = HttpContext.GetUserId();

        Movie? updatedMovie = await _movieService.UpdateAsync(movie, userId, token);
        if (updatedMovie is null) return NotFound();

        return Ok(movie.MapToResponse());
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        bool deleted = await _movieService.DeleteByIdAsync(id, token);
        if (!deleted) return NotFound();

        return Ok();
    }
}
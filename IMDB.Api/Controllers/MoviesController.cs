using IMDB.Api.Mapping;
using IMDB.Application.Models;
using IMDB.Application.Services;
using IMDB.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.Api.Controllers;

[Authorize]
[ApiController]
public class MoviesController(IMovieService _movieService) : ControllerBase
{
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
    {
        Movie movie = request.MapToMovie();

        await _movieService.CreateAsync(movie, token);

        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie.MapToResponse());
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        Movie? movie = Guid.TryParse(idOrSlug, out Guid id)
            ? await _movieService.GetByIdAsync(id, token)
            : await _movieService.GetBySlugAsync(idOrSlug, token);

        if (movie is null) return NotFound();

        return Ok(movie.MapToResponse());
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        IEnumerable<Movie> movies = await _movieService.GetAllAsync(token);

        return Ok(movies.MapToResponse());
    }

    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request,
        CancellationToken token)
    {
        Movie movie = request.MapToMovie(id);

        Movie? updatedMovie = await _movieService.UpdateAsync(movie, token);

        if (updatedMovie is null) return NotFound();

        return Ok(movie.MapToResponse());
    }

    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        bool deleted = await _movieService.DeleteByIdAsync(id, token);

        if (!deleted) return NotFound();

        return Ok();
    }
}
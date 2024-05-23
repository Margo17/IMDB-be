using IMDB.Api.Mapping;
using IMDB.Application.Models;
using IMDB.Application.Repositories;
using IMDB.Application.Services;
using IMDB.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.Api.Controllers;

[ApiController]
public class MoviesController(IMovieService _movieService) : ControllerBase
{
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        Movie movie = request.MapToMovie();

        await _movieService.CreateAsync(movie);

        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie.MapToResponse());
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug)
    {
        Movie? movie = Guid.TryParse(idOrSlug, out Guid id)
            ? await _movieService.GetByIdAsync(id)
            : await _movieService.GetBySlugAsync(idOrSlug);

        if (movie is null) return NotFound();

        return Ok(movie.MapToResponse());
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        IEnumerable<Movie> movies = await _movieService.GetAllAsync();

        return Ok(movies.MapToResponse());
    }

    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request)
    {
        Movie movie = request.MapToMovie(id);

        Movie? updatedMovie = await _movieService.UpdateAsync(movie);

        if (updatedMovie is null) return NotFound();

        return Ok(movie.MapToResponse());
    }

    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        bool deleted = await _movieService.DeleteByIdAsync(id);

        if (!deleted) return NotFound();

        return Ok();
    }
}
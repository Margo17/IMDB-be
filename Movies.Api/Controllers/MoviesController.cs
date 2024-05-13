using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController(IMovieRepository _movieRepository) : ControllerBase
{
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody]CreateMovieRequest request)
    {
        Movie movie = request.MapToMovie();
        
        await _movieRepository.CreateAsync(movie);

        return Created($"{ApiEndpoints.Movies.Create}/{movie.Id}", movie.MapToResponse());
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> GetById([FromRoute]Guid id)
    {
        Movie? movie = await _movieRepository.GetByIdAsync(id);

        if (movie is null)
        {
            return NotFound();
        }
        
        return Ok(movie.MapToResponse());
    }
    
    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        IEnumerable<Movie> movies = await _movieRepository.GetAllAsync();
        
        return Ok(movies.MapToResponse());
    }
}
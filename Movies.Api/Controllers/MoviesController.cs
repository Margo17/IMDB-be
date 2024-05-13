using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
[Route("api")]
public class MoviesController(IMovieRepository _movieRepository) : ControllerBase
{
    [HttpPost("movies")]
    public async Task<IActionResult> Create([FromBody]CreateMovieRequest request)
    {
        Movie movie = request.MapToMovie();
        
        await _movieRepository.CreateAsync(movie);

        return Created($"/api/movies/{movie.Id}", movie);
    }
}
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Repositories;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController(IMovieRepository _movieRepository) : ControllerBase
{
}
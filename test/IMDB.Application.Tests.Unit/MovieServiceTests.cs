using FluentValidation;
using IMDB.Application.Models;
using IMDB.Application.Repositories;
using IMDB.Application.Services;
using NSubstitute;

namespace IMDB.Application.Tests.Unit;

public class MovieServiceTests
{
    private readonly MovieService _sut;
    private readonly IRatingRepository _ratingRepository = Substitute.For<IRatingRepository>();
    private readonly IMovieRepository _movieRepository = Substitute.For<IMovieRepository>();
    private readonly IValidator<Movie> _movieValidator = Substitute.For<IValidator<Movie>>();

    private readonly IValidator<GetAllMoviesOptions> _optionsValidator =
        Substitute.For<IValidator<GetAllMoviesOptions>>();

    public MovieServiceTests()
    {
        _sut = new MovieService(_movieRepository, _ratingRepository, _movieValidator, _optionsValidator);
    }
}
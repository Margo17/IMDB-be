using FluentAssertions;
using FluentAssertions.Specialized;
using FluentValidation;
using FluentValidation.Results;
using IMDB.Application.Models;
using IMDB.Application.Repositories;
using IMDB.Application.Services;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace IMDB.Application.Tests.Unit;

public class MovieServiceTests
{
    private readonly MovieService _sut;
    private readonly IRatingRepository _ratingRepository = Substitute.For<IRatingRepository>();
    private readonly IMovieRepository _movieRepository = Substitute.For<IMovieRepository>();
    private readonly IValidator<Movie> _movieValidator = Substitute.For<IValidator<Movie>>();
    private readonly IValidator<GetAllMoviesOptions> _optionsValidator = Substitute.For<IValidator<GetAllMoviesOptions>>();

    public MovieServiceTests()
    {
        _sut = new MovieService(_movieRepository, _ratingRepository, _movieValidator, _optionsValidator);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenMovieDoesNotPassValidation()
    {
        // Arrange
        Movie invalidMovie = new()
        {
            Id = Guid.Empty,
            Title = "",
            Rating = 1,
            UserRating = 1,
            Year = DateTime.UtcNow.AddYears(1).Year,
            Genres = []
        };
        List<ValidationFailure> validationFailures =
        [
            new("Id", "Id must not be empty"),
            new("Title", "Title must not be empty"),
            new("Year", $"Year must be less than or equal to {DateTime.UtcNow.Year}"),
            new("Genres", "Genres must not be empty")
        ];
        ValidationException validationException = new(validationFailures);
        _movieValidator.ValidateAndThrowAsync(Arg.Any<Movie>(), Arg.Any<CancellationToken>())
            .Throws(validationException); // Todo: Need to mock validator properly

        // Act
        Func<Task<bool>> requestAction = async () => await _sut.CreateAsync(invalidMovie);

        // Assert
        ExceptionAssertions<ValidationException>? exception = await requestAction.Should().ThrowAsync<ValidationException>();
        exception.And.Errors.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(validationFailures);
    }
}
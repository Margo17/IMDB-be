using FluentAssertions;
using FluentAssertions.Specialized;
using FluentValidation;
using FluentValidation.Results;
using IMDB.Application.Repositories;
using IMDB.Application.Services;
using NSubstitute;

namespace IMDB.Application.Tests.Unit;

public class RatingServiceTests
{
    private readonly RatingService _sut;
    private readonly IRatingRepository _ratingRepository = Substitute.For<IRatingRepository>();
    private readonly IMovieRepository _movieRepository = Substitute.For<IMovieRepository>();

    public RatingServiceTests()
    {
        _sut = new RatingService(_ratingRepository, _movieRepository);
    }

    [Fact]
    public async Task RateMovieAsync_ShouldThrowValidationExceptionWithCorrectDetails_WhenRatingIsInvalid()
    {
        // Arrange
        const int invalidRating = -1;
        const string expectedPropertyName = "rating";
        const string expectedErrorMessage = "The rating must be between 1 and 5";

        // Act
        Func<Task> result = async () => await _sut.RateMovieAsync(Arg.Any<Guid>(), invalidRating, Arg.Any<Guid>());

        // Assert
        ExceptionAssertions<ValidationException>? exception = await result.Should().ThrowAsync<ValidationException>();
        exception.And.Errors.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new ValidationFailure
            {
                PropertyName = expectedPropertyName,
                ErrorMessage = expectedErrorMessage
            });
    }

    [Fact]
    public async Task RateMovieAsync_ShouldRateMovie_WhenMovieExists()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid movieId = Guid.NewGuid();
        const int movieRating = 5;
        _movieRepository.ExistsByIdAsync(movieId).Returns(true);
        _ratingRepository.RateMovieAsync(movieId, movieRating, userId).Returns(true);

        // Act
        bool result = await _sut.RateMovieAsync(movieId, movieRating, userId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task RateMovieAsync_ShouldNotRateMovie_WhenNoMovieExistsOrIdIsInvalid()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid movieId = Guid.NewGuid();
        const int movieRating = 5;
        _movieRepository.ExistsByIdAsync(movieId).Returns(false);
        _ratingRepository.RateMovieAsync(movieId, movieRating, userId).Returns(true);

        // Act
        bool result = await _sut.RateMovieAsync(movieId, movieRating, userId);

        // Assert
        result.Should().BeFalse();
    }
}
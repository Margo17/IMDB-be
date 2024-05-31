using FluentValidation;
using FluentValidation.Results;
using IMDB.Application.Repositories;

namespace IMDB.Application.Services;

public class RatingService(IRatingRepository _ratingRepository, IMovieRepository _movieRepository) : IRatingService
{
    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default)
    {
        if (rating is <= 0 or > 5)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure
                {
                    PropertyName = "rating",
                    ErrorMessage = "The rating must be between 1 and 5"
                }
            });
        }

        bool movieExists = await _movieRepository.ExistsByIdAsync(movieId, token);
        if (!movieExists)
        {
            return false;
        }

        return await _ratingRepository.RateMovieAsync(movieId, rating, userId, token);
    }

    public Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        return _ratingRepository.DeleteRatingAsync(movieId, userId, token);
    }
}
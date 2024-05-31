using FluentValidation;
using IMDB.Application.Models;
using IMDB.Application.Repositories;

namespace IMDB.Application.Services;

public class MoveService(
    IMovieRepository _movieRepository,
    IRatingRepository _ratingRepository,
    IValidator<Movie> _movieValidator) : IMovieService
{
    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken: token);
        
        return await _movieRepository.CreateAsync(movie, token);
    }

    public Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
    {
        return _movieRepository.GetByIdAsync(id, userId, token);
    }

    public Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
    {
        return _movieRepository.GetBySlugAsync(slug, userId, token);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(Guid? userId = default, CancellationToken token = default)
    {
        return _movieRepository.GetAllAsync(userId, token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken token = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken: token);
        
        bool movieExists = await _movieRepository.ExistsByIdAsync(movie.Id, token);
        if (!movieExists) return null;

        await _movieRepository.UpdateAsync(movie, token);

        if (!userId.HasValue)
        {
            float? rating = await _ratingRepository.GetRatingAsync(movie.Id, token);
            movie.Rating = rating;
            
            return movie;
        }

        (float? movieRating, int? userRating) = await _ratingRepository.GetRatingAsync(movie.Id, userId.Value, token);
        movie.Rating = movieRating;
        movie.UserRating = userRating;
        
        return movie;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        return _movieRepository.DeleteByIdAsync(id, token);
    }
}
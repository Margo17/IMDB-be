namespace IMDB.Application.Services;

public interface IRatingService
{
    Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default);

    Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default);
}
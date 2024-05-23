using IMDB.Application.Models;

namespace IMDB.Application.Services;

public interface IMovieService
{
    Task<bool> CreateAsync(Movie movie);

    Task<Movie?> GetByIdAsync(Guid id);

    Task<Movie?> GetBySlugAsync(string slug);

    Task<IEnumerable<Movie>> GetAllAsync();

    Task<Movie?> UpdateAsync(Movie movie);

    Task<bool> DeleteByIdAsync(Guid id);
}
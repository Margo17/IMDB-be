using IMDB.Contracts.Responses;
using Refit;

namespace IMDB.Api.Sdk;

public interface IMoviesApi
{
    [Get(ApiEndpoints.Movies.Get)]
    Task<MovieResponse> GetMovieAsync(string idOrSlug);
}
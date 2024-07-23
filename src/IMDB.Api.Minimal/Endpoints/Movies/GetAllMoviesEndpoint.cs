using IMDB.Api.Minimal.Auth;
using IMDB.Api.Minimal.Mapping;
using IMDB.Application.Models;
using IMDB.Application.Services;
using IMDB.Contracts.Minimal.Requests;
using IMDB.Contracts.Minimal.Responses;

namespace IMDB.Api.Minimal.Endpoints.Movies;

public static class GetAllMoviesEndpoint
{
    public const string Name = "GetMovies";

    public static IEndpointRouteBuilder MapGetAllMovies(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Movies.GetAll,
                async ([AsParameters] GetAllMoviesRequest request, IMovieService movieService, HttpContext context,
                    CancellationToken token) =>
                {
                    Guid? userId = context.GetUserId();
                    GetAllMoviesOptions options = request.MapToOptions()
                        .WithUser(userId);

                    IEnumerable<Movie> movies = await movieService.GetAllAsync(options, token);
                    int movieCount = await movieService.GetCountAsync(options.Title, options.Year, token);
                    MoviesResponse moviesResponse = movies.MapToResponse(
                        request.Page.GetValueOrDefault(PagedRequest.DefaultPage),
                        request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize),
                        movieCount);

                    return TypedResults.Ok(moviesResponse);
                })
            .WithName(Name)
            .Produces<MoviesResponse>(StatusCodes.Status200OK);

        return app;
    }
}
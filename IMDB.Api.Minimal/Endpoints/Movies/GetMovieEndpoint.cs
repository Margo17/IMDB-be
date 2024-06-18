using IMDB.Api.Minimal.Auth;
using IMDB.Api.Minimal.Mapping;
using IMDB.Application.Models;
using IMDB.Application.Services;
using IMDB.Contracts.Minimal.Responses;

namespace IMDB.Api.Minimal.Endpoints.Movies;

public static class GetMovieEndpoint
{
    public const string Name = "GetMovie";

    public static IEndpointRouteBuilder MapGetMovie(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Movies.Get,
                async (string idOrSlug, IMovieService movieService, HttpContext context, CancellationToken token) =>
                {
                    Guid? userId = context.GetUserId();

                    Movie? movie = Guid.TryParse(idOrSlug, out Guid id)
                        ? await movieService.GetByIdAsync(id, userId, token)
                        : await movieService.GetBySlugAsync(idOrSlug, userId, token);

                    return movie is null ? Results.NotFound() : TypedResults.Ok(movie.MapToResponse());
                })
            .WithName(Name)
            .Produces<MovieResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
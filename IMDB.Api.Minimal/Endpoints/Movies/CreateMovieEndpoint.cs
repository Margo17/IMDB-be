using IMDB.Api.Minimal.Mapping;
using IMDB.Application.Models;
using IMDB.Application.Services;
using IMDB.Contracts.Requests;

namespace IMDB.Api.Minimal.Endpoints.Movies;

public static class CreateMovieEndpoint
{
    public const string Name = "CreateMovie";

    public static IEndpointRouteBuilder MapCreateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Movies.Create,
            async (IMovieService movieService, CreateMovieRequest request, CancellationToken token) =>
            {
                Movie movie = request.MapToMovie();

                await movieService.CreateAsync(movie, token);

                return TypedResults.CreatedAtRoute(movie.MapToResponse(), GetMovieEndpoint.Name,
                    new { idOrSlug = movie.Id });
            })
            .WithName(Name);

        return app;
    }
}
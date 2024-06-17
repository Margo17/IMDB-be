using IMDB.Application.Services;

namespace IMDB.Api.Minimal.Endpoints.Movies;

public static class DeleteMovieEndpoint
{
    public const string Name = "DeleteMovie";

    public static IEndpointRouteBuilder MapDeleteMovie(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Movies.Delete,
                async (Guid id, IMovieService movieService, CancellationToken token) =>
                {
                    bool deleted = await movieService.DeleteByIdAsync(id, token);
                    if (!deleted) return Results.NotFound();

                    return TypedResults.Ok();
                })
            .WithName(Name);

        return app;
    }
}
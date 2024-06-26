using IMDB.Api.Minimal.Auth;
using IMDB.Api.Minimal.Mapping;
using IMDB.Application.Models;
using IMDB.Application.Services;
using IMDB.Contracts.Minimal.Requests;
using IMDB.Contracts.Minimal.Responses;

namespace IMDB.Api.Minimal.Endpoints.Movies;

public static class UpdateMovieEndpoint
{
    public const string Name = "UpdateMovie";

    public static IEndpointRouteBuilder MapUpdateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Movies.Update,
                async (Guid id, UpdateMovieRequest request, IMovieService movieService, HttpContext context,
                    CancellationToken token) =>
                {
                    Movie movie = request.MapToMovie(id);
                    Guid? userId = context.GetUserId();

                    Movie? updatedMovie = await movieService.UpdateAsync(movie, userId, token);
                    if (updatedMovie is null) return Results.NotFound();

                    return TypedResults.Ok(movie.MapToResponse());
                })
            .WithName(Name)
            .Produces<MovieResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);

        return app;
    }
}
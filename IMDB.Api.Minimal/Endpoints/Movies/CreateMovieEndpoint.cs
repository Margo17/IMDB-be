using IMDB.Api.Minimal.Auth;
using IMDB.Api.Minimal.Mapping;
using IMDB.Application.Models;
using IMDB.Application.Services;
using IMDB.Contracts.Minimal.Requests;
using IMDB.Contracts.Minimal.Responses;

namespace IMDB.Api.Minimal.Endpoints.Movies;

public static class CreateMovieEndpoint
{
    public const string Name = "CreateMovie";

    public static IEndpointRouteBuilder MapCreateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Movies.Create,
            async (CreateMovieRequest request, IMovieService movieService, CancellationToken token) =>
            {
                Movie movie = request.MapToMovie();

                await movieService.CreateAsync(movie, token);

                return TypedResults.CreatedAtRoute(movie.MapToResponse(), GetMovieEndpoint.Name,
                    new { idOrSlug = movie.Id });
            })
            .WithName(Name)
            .Produces<MovieResponse>(StatusCodes.Status201Created)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);

        return app;
    }
}
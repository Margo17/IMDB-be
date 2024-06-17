using IMDB.Api.Minimal.Auth;
using IMDB.Api.Minimal.Mapping;
using IMDB.Application.Models;
using IMDB.Application.Services;

namespace IMDB.Api.Minimal.Endpoints.Ratings;

public static class GetUserRatingsEndpoint
{
    public const string Name = "GetUserRatings";

    public static IEndpointRouteBuilder MapGetUserRatings(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Ratings.GetUserRatings,
                async (HttpContext context, IRatingService ratingService,
                    CancellationToken token) =>
                {
                    Guid? userId = context.GetUserId();

                    IEnumerable<MovieRating> result = await ratingService.GetRatingsForUserAsync(userId!.Value, token);

                    return TypedResults.Ok(result.MapToResponse());
                })
            .WithName(Name);

        return app;
    }
}
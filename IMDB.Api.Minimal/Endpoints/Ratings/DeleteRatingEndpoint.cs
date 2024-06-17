using IMDB.Api.Minimal.Auth;
using IMDB.Application.Services;

namespace IMDB.Api.Minimal.Endpoints.Ratings;

public static class DeleteRatingEndpoint
{
    public const string Name = "DeleteRating";

    public static IEndpointRouteBuilder MapDeleteRating(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Movies.DeleteRating,
                async (Guid id, HttpContext context, IRatingService ratingService,
                    CancellationToken token) =>
                {
                    Guid? userId = context.GetUserId();

                    bool result = await ratingService.DeleteRatingAsync(id, userId!.Value, token);

                    return result ? Results.Ok() : Results.NotFound();
                })
            .WithName(Name)
            .RequireAuthorization();

        return app;
    }
}
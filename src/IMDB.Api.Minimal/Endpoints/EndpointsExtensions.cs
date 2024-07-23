using IMDB.Api.Minimal.Endpoints.Movies;
using IMDB.Api.Minimal.Endpoints.Ratings;

namespace IMDB.Api.Minimal.Endpoints;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapMovieEndpoints();
        app.MapRatingEndpoints();

        return app;
    }
}
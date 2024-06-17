namespace IMDB.Api.Minimal.Endpoints.Movies;

public static class MovieEndpointExtensions
{
    public static IEndpointRouteBuilder MapMovieEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetMovie();
        app.MapCreateMovie();
        /*
        app.MapGetAllMovie();
        app.MapUpdateMovie();
        app.MapDeleteMovie();
        */

        return app;
    }
}
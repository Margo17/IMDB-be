using System.Text.Json;
using IMDB.Api.Sdk;
using IMDB.Api.Sdk.Consumer;
using IMDB.Contracts.Requests;
using IMDB.Contracts.Responses;
using Microsoft.Extensions.DependencyInjection;
using Refit;

// This allows for a basic Refit implementation without HttpClientFactory and DI
// IMoviesApi moviesApi = RestService.For<IMoviesApi>("https://localhost:5001");

/* A better way is to move DI registration to an extension method
   to provide simpler interaction for SDK consumer */
ServiceCollection services = [];
services
    .AddHttpClient()
    .AddSingleton<AuthTokenProvider>()
    .AddRefitClient<IMoviesApi>(sa => new RefitSettings
    {
        AuthorizationHeaderValueGetter = async (_, _) => await sa.GetRequiredService<AuthTokenProvider>().GetTokenAsync()
    })
    .ConfigureHttpClient(cc => cc.BaseAddress = new Uri("https://localhost:5001"));

ServiceProvider provider = services.BuildServiceProvider();
IMoviesApi moviesApi = provider.GetRequiredService<IMoviesApi>();

MovieResponse movie = await moviesApi.GetMovieAsync("kazkoks-filmas-2000");
MovieResponse newMovie = await moviesApi.CreateMovieAsync(new CreateMovieRequest
{
    Title = "2000: A Space Odyssey",
    Year = 1967,
    Genres = ["Adventure", "Sci-Fi"]
});
await moviesApi.UpdateMovieAsync(newMovie.Id, new UpdateMovieRequest
{
    Title = "2001: A Space Odyssey",
    Year = 1968,
    Genres = ["Adventure", "Sci-Fi"]
});
await moviesApi.RateMovieAsync(newMovie.Id, new RateMovieRequest
{
    Rating = 5
});
await moviesApi.DeleteRatingAsync(newMovie.Id);
await moviesApi.DeleteMovieAsync(newMovie.Id);
MoviesResponse movies = await moviesApi.GetMoviesAsync(new GetAllMoviesRequest
{
    Title = null,
    Year = null,
    SortBy = null,
    Page = 1,
    PageSize = 5
});

Console.WriteLine("GetMovie response:");
Console.WriteLine(JsonSerializer.Serialize(movie));

Console.WriteLine("GetMovies response:");
foreach (MovieResponse movieEntry in movies.Items)
{
    Console.WriteLine(JsonSerializer.Serialize(movieEntry));
}

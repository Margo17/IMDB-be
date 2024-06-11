using System.Text.Json;
using IMDB.Api.Sdk;
using IMDB.Api.Sdk.Consumer;
using IMDB.Contracts.Requests;
using IMDB.Contracts.Responses;
using Microsoft.Extensions.DependencyInjection;
using Refit;

// This allows for a simple Refit implementation without HttpClientFactory and DI
// IMoviesApi moviesApi = RestService.For<IMoviesApi>("https://localhost:5001");

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

GetAllMoviesRequest moviesRequest = new()
{
    Title = null,
    Year = null,
    SortBy = null,
    Page = 1,
    PageSize = 3
};
MoviesResponse movies = await moviesApi.GetMoviesAsync(moviesRequest);

Console.WriteLine("GetMovie response:");
Console.WriteLine(JsonSerializer.Serialize(movie));

Console.WriteLine("GetMovies response:");
foreach (MovieResponse movieEntry in movies.Items)
{
    Console.WriteLine(JsonSerializer.Serialize(movieEntry));
}

using System.Text.Json;
using IMDB.Api.Sdk;
using IMDB.Contracts.Requests;
using IMDB.Contracts.Responses;
using Refit;

IMoviesApi moviesApi = RestService.For<IMoviesApi>("https://localhost:5001");

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

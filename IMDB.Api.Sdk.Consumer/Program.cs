using System.Text.Json;
using IMDB.Api.Sdk;
using IMDB.Contracts.Responses;
using Refit;

IMoviesApi moviesApi = RestService.For<IMoviesApi>("https://localhost:5001");

MovieResponse movie = await moviesApi.GetMovieAsync("martyno-filmas-2000");

Console.WriteLine(JsonSerializer.Serialize(movie));
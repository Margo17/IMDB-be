using IMDB.Application.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace IMDB.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IMovieRepository, MovieRepository>();

        return services;
    }
}
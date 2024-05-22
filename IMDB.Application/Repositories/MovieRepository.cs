using System.Data;
using Dapper;
using IMDB.Application.Database;
using IMDB.Application.Models;

namespace IMDB.Application.Repositories;

public class MovieRepository(IDbConnectionFactory _dbConnectionFactory) : IMovieRepository
{
    public async Task<bool> CreateAsync(Movie movie)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync();
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         insert into movies (id, slug, title, yearofrelease)
                                                                         values (@Id, @Slug, @Title, @YearOfRelease)
                                                                         """, movie));

        if (result is 0) return false;
        
        foreach (string genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                                                                insert into genres (movieId, name)
                                                                values (@MovieId, @Name)
                                                                """, new { MovieId = movie.Id, Name = genre }));
        }
        
        transaction.Commit();

        return true;
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
    
    public Task<Movie?> GetBySlugAsync(string slug)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Movie movie)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
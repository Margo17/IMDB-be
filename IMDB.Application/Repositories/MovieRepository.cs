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

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync();

        Movie? movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            select * from movies where id = @id
            """, new { id }));

        if (movie is null) return null;

        IEnumerable<string> genres = await connection.QueryAsync<string>(new CommandDefinition("""
            select name from genres where movieid = @id 
            """, new { id }));

        foreach (string genre in genres) movie.Genres.Add(genre);

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync();

        Movie? movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            select * from movies where slug = @slug
            """, new { slug }));

        if (movie is null) return null;

        IEnumerable<string> genres = await connection.QueryAsync<string>(new CommandDefinition("""
            select name from genres where movieid = @id 
            """, new { id = movie.Id }));

        foreach (string genre in genres) movie.Genres.Add(genre);

        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync();

        IEnumerable<dynamic> result = await connection.QueryAsync(new CommandDefinition("""
            select m.*, string_agg(g.name, ',') as genres 
            from movies m left join genres g on m.id = g.movieid
            group by id 
            """));

        return result.Select(m => new Movie
        {
            Id = m.id,
            Title = m.title,
            YearOfRelease = m.yearofrelease,
            Genres = Enumerable.ToList(m.genres.Split(','))
        });
    }

    public async Task<bool> UpdateAsync(Movie movie)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync();
        using IDbTransaction transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            delete from genres where movieid = @id
            """, new { id = movie.Id }));

        foreach (string genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                insert into genres (movieId, name)
                values (@MovieId, @Name)
                """, new { MovieId = movie.Id, Name = genre }));
        }
        
        int result = await connection.ExecuteAsync(new CommandDefinition("""
            update movies set slug = @Slug, title = @Title, yearofrelease = @YearOfRelease 
            where id = @Id
            """, movie));
        
        transaction.Commit();
        
        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync();
        using IDbTransaction transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            delete from genres where movieid = @id
            """, new { id }));

        int result = await connection.ExecuteAsync(new CommandDefinition("""
            delete from movies where id = @id
            """, new { id }));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync();

        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
            select count(1) from movies where id = @id
            """, new { id }));
    }
}
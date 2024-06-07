using System.Data;
using Dapper;
using IMDB.Application.Database;
using IMDB.Application.Models;

namespace IMDB.Application.Repositories;

public class MovieRepository(IDbConnectionFactory _dbConnectionFactory) : IMovieRepository
{
    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        int result = await connection.ExecuteAsync(new CommandDefinition("""
            insert into movies (id, slug, title, year)
            values (@Id, @Slug, @Title, @Year)
            """, movie, cancellationToken: token));

        if (result is 0) return false;

        foreach (string genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                insert into genres (movieid, name)
                values (@MovieId, @Name)
                """, new { MovieId = movie.Id, Name = genre }, cancellationToken: token));
        }

        transaction.Commit();

        return true;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        Movie? movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating
            from movies m
            left join ratings r on m.id = r.movieid
            left join ratings myr on m.id = myr.movieid
                and myr.userid = @userId
            where id = @id
            group by id, userrating
            """, new { id, userId }, cancellationToken: token));

        if (movie is null) return null;

        IEnumerable<string> genres = await connection.QueryAsync<string>(new CommandDefinition("""
            select name from genres where movieid = @id 
            """, new { id }, cancellationToken: token));

        foreach (string genre in genres) movie.Genres.Add(genre);

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        Movie? movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition("""
            select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating
            from movies m
            left join ratings r on m.id = r.movieid
            left join ratings myr on m.id = myr.movieid
                and myr.userid = @userId
            where slug = @slug
            group by id, userrating
            """, new { slug, userId }, cancellationToken: token));

        if (movie is null) return null;

        IEnumerable<string> genres = await connection.QueryAsync<string>(new CommandDefinition("""
            select name from genres where movieid = @id 
            """, new { id = movie.Id }, cancellationToken: token));

        foreach (string genre in genres) movie.Genres.Add(genre);

        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        string orderClause = string.Empty;
        if (options.SortField is not null)
        {
            orderClause = $"""
                , m.{options.SortField}
                order by m.{options.SortField} {(options.SortOrder == SortOrder.Ascending ? "asc" : "desc")}
                """;
        }

        IEnumerable<dynamic> result = await connection.QueryAsync(new CommandDefinition($"""
                select m.*,
                       string_agg(distinct g.name, ',') as genres,
                       round(avg(r.rating), 1) as rating,
                       myr.rating as userrating
                from movies m
                left join genres g on m.id = g.movieid
                left join ratings r on m.id = r.movieid
                left join ratings myr on m.id = myr.movieid
                    and myr.userid = @userid
                where (@title is null or m.title like ('%' || @title || '%'))
                and (@year is null or m.year = @year)
                group by id, userrating {orderClause}
                limit @pageSize
                offset @pageOffset
                """, new
            {
                userId = options.UserId,
                title = options.Title,
                year = options.Year,
                pageSize = options.PageSize,
                pageOffset = (options.Page - 1) * options.PageSize
            },
            cancellationToken: token));

        return result.Select(m => new Movie
        {
            Id = m.id,
            Title = m.title,
            Year = m.year,
            Rating = (float?)m.rating,
            UserRating = (int?)m.userrating,
            Genres = Enumerable.ToList(m.genres.Split(','))
        });
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            delete from genres where movieid = @id
            """, new { id = movie.Id }, cancellationToken: token));

        foreach (string genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                insert into genres (movieid, name)
                values (@MovieId, @Name)
                """, new { MovieId = movie.Id, Name = genre }, cancellationToken: token));
        }
        
        int result = await connection.ExecuteAsync(new CommandDefinition("""
            update movies set slug = @Slug, title = @Title, year = @Year 
            where id = @Id
            """, movie, cancellationToken: token));
        
        transaction.Commit();
        
        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using IDbTransaction transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            delete from genres where movieid = @id
            """, new { id }, cancellationToken: token));

        int result = await connection.ExecuteAsync(new CommandDefinition("""
            delete from movies where id = @id
            """, new { id }, cancellationToken: token));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
            select count(1) from movies where id = @id
            """, new { id }, cancellationToken: token));
    }

    public async Task<int> GetCountAsync(string? title, int? year, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        return await connection.QuerySingleAsync(new CommandDefinition("""
            select count(*)
            from movies
            where (@title is null or title like ('%' || @title || '%'))
            and (@year is null or @year = year) 
            """, new { title, year }, cancellationToken: token));
    }
}
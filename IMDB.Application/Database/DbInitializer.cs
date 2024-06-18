using System.Data;
using Dapper;

namespace IMDB.Application.Database;

public class DbInitializer(IDbConnectionFactory _dbConnectionFactory)
{
    public async Task InitializeAsync()
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync();

        await connection.ExecuteAsync("""
                                      create table if not exists movies (
                                          id UUID primary key,
                                          slug TEXT not null,
                                          title TEXT not null,
                                          year INTEGER not null);
                                      """);

        await connection.ExecuteAsync("""
                                      create unique index concurrently if not exists movies_slug_idx
                                      on movies
                                      using btree(slug)
                                      """);

        await connection.ExecuteAsync("""
                                      create table if not exists genres (
                                          movieid UUID references movies (id),
                                          name TEXT not null);
                                      """);

        await connection.ExecuteAsync("""
                                      create table if not exists ratings (
                                          userid UUID,
                                          movieid UUID references movies (id),
                                          rating INTEGER not null,
                                          primary key (userid, movieid));
                                      """);
    }
}
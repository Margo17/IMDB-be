using System.Data;
using Dapper;
using IMDB.Application.Database;
using IMDB.Application.Models;

namespace IMDB.Application.Repositories;

public class RatingRepository(IDbConnectionFactory _dbConnectionFactory) : IRatingRepository
{
    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        int result = await connection.ExecuteAsync(new CommandDefinition("""
            insert into ratings(userid, movieid, rating)
            values (@userId, @movieId, @rating)
            on conflict (userid, movieid) do update
            set rating = @rating
            """, new { userId, movieId, rating }, cancellationToken: token));

        return result > 0;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
            select round(avg(r.rating), 1) from ratings r
            where movieid = @movieId
            """, new { movieId }, cancellationToken: token));
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId,
        CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
            select round(avg(rating), 1),
                (select rating
                 from ratings
                 where movieid = @movieId
                    and userid = @userId
                 limit 1)
            from ratings
            where movieid = @movieId
            """, new { movieId, userId }, cancellationToken: token));
    }

    public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        return await connection.QueryAsync<MovieRating>(new CommandDefinition("""
            select r.movieid, r.rating, m.slug
            from ratings r
            inner join movies m on r.movieid = m.id
            where userid = @userId
            """, new { userId }, cancellationToken: token));
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        int result = await connection.ExecuteAsync(new CommandDefinition("""
            delete from ratings
            where movieid = @movieId
            and userid = @userId
            """, new { movieId, userId }, cancellationToken: token));

        return result > 0;
    }
}
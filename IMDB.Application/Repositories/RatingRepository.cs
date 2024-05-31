using System.Data;
using Dapper;
using IMDB.Application.Database;

namespace IMDB.Application.Repositories;

public class RatingRepository(IDbConnectionFactory _dbConnectionFactory) : IRatingRepository
{
    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
            select round(avg(r.rating), 1) from ratings r
            where movieid = @movieId
            """, new { movieId }, cancellationToken: token));
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
            select round(avg(r.rating), 1),
                (select rating
                 from ratings
                 where movieid = @movieId
                    and userid = @userId
                 limit 1)
            from ratings r
            where movieid = @movieId
            """, new { movieId, userId }, cancellationToken: token));
    }
}
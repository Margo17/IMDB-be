using IMDB.Application.Database;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace IMDB.Api.Health;

public class DatabaseHealthCheck(IDbConnectionFactory _connectionFactory, ILogger<DatabaseHealthCheck> _logger)
    : IHealthCheck
{
    public const string Name = "Database";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken token = new())
    {
        try
        {
            _ = await _connectionFactory.CreateConnectionAsync(token);

            return HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            const string errorMessage = "Database is unhealthy";
            _logger.LogError(errorMessage, e);

            return HealthCheckResult.Unhealthy(errorMessage, e);
        }
    }
}
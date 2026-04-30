using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Domain.Entities;

namespace ApiTier7Provision.Infrastructure.Persistence.Repositories;

public sealed class HealthCheckLogRepository(AppDbContext dbContext) : IHealthCheckLogRepository
{
    public async Task<int> AddAsync(DateTime loggedAtUtc, CancellationToken cancellationToken)
    {
        var healthCheckLog = new HealthCheckLog
        {
            LoggedAtUtc = loggedAtUtc
        };

        await dbContext.HealthCheckLogs.AddAsync(healthCheckLog, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return healthCheckLog.Id;
    }
}

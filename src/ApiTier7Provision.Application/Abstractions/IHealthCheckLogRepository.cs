namespace ApiTier7Provision.Application.Abstractions;

public interface IHealthCheckLogRepository
{
    Task<int> AddAsync(DateTime loggedAtUtc, CancellationToken cancellationToken);
}

using ApiTier7Provision.Application.Abstractions;
using MediatR;

namespace ApiTier7Provision.Application.HealthChecks;

public sealed class LogHealthCheckCommandHandler(
    IHealthCheckLogRepository healthCheckLogRepository,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<LogHealthCheckCommand, LogHealthCheckResult>
{
    public async Task<LogHealthCheckResult> Handle(LogHealthCheckCommand request, CancellationToken cancellationToken)
    {
        var loggedAtUtc = dateTimeProvider.UtcNow;
        var id = await healthCheckLogRepository.AddAsync(loggedAtUtc, cancellationToken);

        return new LogHealthCheckResult(id, loggedAtUtc);
    }
}

using MediatR;

namespace ApiTier7Provision.Application.HealthChecks;

public sealed record LogHealthCheckCommand : IRequest<LogHealthCheckResult>;

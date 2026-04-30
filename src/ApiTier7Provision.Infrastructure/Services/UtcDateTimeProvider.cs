using ApiTier7Provision.Application.Abstractions;

namespace ApiTier7Provision.Infrastructure.Services;

public sealed class UtcDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

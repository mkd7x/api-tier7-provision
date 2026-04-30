namespace ApiTier7Provision.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

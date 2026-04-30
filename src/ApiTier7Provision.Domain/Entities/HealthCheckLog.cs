namespace ApiTier7Provision.Domain.Entities;

public sealed class HealthCheckLog
{
    public int Id { get; set; }

    public DateTime LoggedAtUtc { get; set; }
}

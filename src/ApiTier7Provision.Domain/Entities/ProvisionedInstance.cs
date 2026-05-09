namespace ApiTier7Provision.Domain.Entities;

public sealed class ProvisionedInstance
{
    public int Id { get; set; }

    public int ModelTemplateId { get; set; }

    public ModelTemplate ModelTemplate { get; set; } = null!;

    public long VastInstanceId { get; set; }

    public required string Gpu { get; set; }

    public decimal TotalCostPerHour { get; set; }

    public decimal IngressCost { get; set; }

    public decimal EgressCost { get; set; }

    public DateTime ProvisionedAtUtc { get; set; }
}
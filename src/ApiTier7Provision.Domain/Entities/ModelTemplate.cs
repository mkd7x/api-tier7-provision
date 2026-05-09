namespace ApiTier7Provision.Domain.Entities;

public sealed class ModelTemplate
{
    public int Id { get; set; }

    public required string Model { get; set; }

    public required string[] SupportedGpus { get; set; }

    public required string ProvisioningTemplateUuid { get; set; }

    public ICollection<ProvisionedInstance> Instances { get; set; } = [];
}
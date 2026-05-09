namespace ApiTier7Provision.Infrastructure.Persistence;

public sealed class ModelTemplateCatalogOptions
{
    public const string SectionName = "ModelTemplates";

    public List<ModelTemplateCatalogTemplateOptions> Templates { get; init; } = [];
}

public sealed class ModelTemplateCatalogTemplateOptions
{
    public required string Model { get; init; }

    public string[] SupportedGpus { get; init; } = [];

    public required string ProvisioningTemplateUuid { get; init; }
}
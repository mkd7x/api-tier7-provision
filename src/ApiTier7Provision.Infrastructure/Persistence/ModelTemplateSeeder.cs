using ApiTier7Provision.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApiTier7Provision.Infrastructure.Persistence;

public sealed class ModelTemplateSeeder(
    AppDbContext dbContext,
    IOptions<ModelTemplateCatalogOptions> options)
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var templates = options.Value.Templates
            .Select(Normalize)
            .Where(x => x is not null)
            .Cast<ModelTemplateCatalogTemplateOptions>()
            .ToArray();

        if (templates.Length == 0)
        {
            return;
        }

        var existingTemplates = await dbContext.ModelTemplates.ToListAsync(cancellationToken);
        var existingByModel = existingTemplates.ToDictionary(x => x.Model, StringComparer.OrdinalIgnoreCase);

        foreach (var template in templates)
        {
            if (existingByModel.TryGetValue(template.Model, out var existing))
            {
                existing.SupportedGpus = template.SupportedGpus;
                existing.ProvisioningTemplateUuid = template.ProvisioningTemplateUuid;
                continue;
            }

            dbContext.ModelTemplates.Add(new ModelTemplate
            {
                Model = template.Model,
                SupportedGpus = template.SupportedGpus,
                ProvisioningTemplateUuid = template.ProvisioningTemplateUuid
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static ModelTemplateCatalogTemplateOptions? Normalize(ModelTemplateCatalogTemplateOptions template)
    {
        if (string.IsNullOrWhiteSpace(template.Model)
            || string.IsNullOrWhiteSpace(template.ProvisioningTemplateUuid))
        {
            return null;
        }

        var supportedGpus = template.SupportedGpus
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (supportedGpus.Length == 0)
        {
            return null;
        }

        return new ModelTemplateCatalogTemplateOptions
        {
            Model = template.Model.Trim(),
            SupportedGpus = supportedGpus,
            ProvisioningTemplateUuid = template.ProvisioningTemplateUuid.Trim()
        };
    }
}
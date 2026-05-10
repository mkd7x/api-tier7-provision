using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiTier7Provision.Infrastructure.Persistence.Repositories;

public sealed class ModelTemplateRepository(AppDbContext dbContext) : IModelTemplateRepository
{
    public Task<ModelTemplate?> GetByModelAsync(string model, CancellationToken cancellationToken) =>
        dbContext.ModelTemplates
            .AsNoTracking()
            .SingleOrDefaultAsync(x => EF.Functions.ILike(x.Model, model), cancellationToken);

    public Task<List<ModelTemplate>> GetAllAsync(CancellationToken cancellationToken) =>
        dbContext.ModelTemplates
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task AddAsync(ModelTemplate modelTemplate, CancellationToken cancellationToken)
    {
        dbContext.ModelTemplates.FirstOrDefaultAsync(x => EF.Functions.ILike(x.Model, modelTemplate.Model), cancellationToken)
            .ContinueWith(existingTemplateTask =>
            {
                if (existingTemplateTask.Result is not null)
                {
                    throw new ArgumentException($"A model template with the model '{modelTemplate.Model}' already exists.", nameof(modelTemplate));
                }
            }, cancellationToken).Wait(cancellationToken);

        dbContext.ModelTemplates.Add(modelTemplate);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ModelTemplateDeleteStatus> DeleteByModelAsync(string model, CancellationToken cancellationToken)
    {
        var modelTemplate = await dbContext.ModelTemplates
            .SingleOrDefaultAsync(x => EF.Functions.ILike(x.Model, model), cancellationToken);

        if (modelTemplate is null)
        {
            return ModelTemplateDeleteStatus.NotFound;
        }

        var isInUse = await dbContext.ProvisionedInstances
            .AnyAsync(x => x.ModelTemplateId == modelTemplate.Id, cancellationToken);

        if (isInUse)
        {
            return ModelTemplateDeleteStatus.InUse;
        }

        dbContext.ModelTemplates.Remove(modelTemplate);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ModelTemplateDeleteStatus.Deleted;
    }
}
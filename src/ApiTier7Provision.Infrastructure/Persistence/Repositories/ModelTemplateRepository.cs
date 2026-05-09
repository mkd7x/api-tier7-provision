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
}
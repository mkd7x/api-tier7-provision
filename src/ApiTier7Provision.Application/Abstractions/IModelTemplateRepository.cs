using ApiTier7Provision.Domain.Entities;

namespace ApiTier7Provision.Application.Abstractions;

public interface IModelTemplateRepository
{
    Task<ModelTemplate?> GetByModelAsync(string model, CancellationToken cancellationToken);
}
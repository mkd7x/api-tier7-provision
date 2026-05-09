using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Domain.Entities;

namespace ApiTier7Provision.Infrastructure.Persistence.Repositories;

public sealed class ProvisionedInstanceRepository(AppDbContext dbContext) : IProvisionedInstanceRepository
{
    public async Task<int> AddAsync(ProvisionedInstance provisionedInstance, CancellationToken cancellationToken)
    {
        await dbContext.ProvisionedInstances.AddAsync(provisionedInstance, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return provisionedInstance.Id;
    }
}
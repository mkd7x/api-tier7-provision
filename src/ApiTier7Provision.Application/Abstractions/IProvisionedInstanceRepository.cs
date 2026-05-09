using ApiTier7Provision.Domain.Entities;

namespace ApiTier7Provision.Application.Abstractions;

public interface IProvisionedInstanceRepository
{
    Task<int> AddAsync(ProvisionedInstance provisionedInstance, CancellationToken cancellationToken);
}
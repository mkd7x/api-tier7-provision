using ApiTier7Provision.Application.Queries.Instances;

namespace ApiTier7Provision.Application.Abstractions;

public interface IVastAiInstancesGateway
{
    Task<GetInstancesResult> GetInstancesAsync(VastAiInstancesRequest request, CancellationToken cancellationToken);
}

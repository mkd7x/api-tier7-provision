using ApiTier7Provision.Application.Abstractions;
using MediatR;

namespace ApiTier7Provision.Application.Queries.Instances;

public sealed class GetInstancesQueryHandler(IVastAiInstancesGateway vastAiInstancesGateway)
    : IRequestHandler<GetInstancesQuery, GetInstancesResult>
{
    public Task<GetInstancesResult> Handle(GetInstancesQuery request, CancellationToken cancellationToken) =>
        vastAiInstancesGateway.GetInstancesAsync(request.ToRequest(), cancellationToken);
}

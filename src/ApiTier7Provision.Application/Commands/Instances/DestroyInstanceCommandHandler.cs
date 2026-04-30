using ApiTier7Provision.Application.Abstractions;
using MediatR;

namespace ApiTier7Provision.Application.Commands.Instances;

public sealed class DestroyInstanceCommandHandler(IVastAiInstancesGateway vastAiInstancesGateway)
    : IRequestHandler<DestroyInstanceCommand, DestroyInstanceResult>
{
    public Task<DestroyInstanceResult> Handle(DestroyInstanceCommand request, CancellationToken cancellationToken) =>
        vastAiInstancesGateway.DestroyInstanceAsync(request.ToRequest(), cancellationToken);
}

using ApiTier7Provision.Application.Abstractions;
using MediatR;

namespace ApiTier7Provision.Application.Commands.Instances;

public sealed class ManageInstanceCommandHandler(IVastAiInstancesGateway vastAiInstancesGateway)
    : IRequestHandler<ManageInstanceCommand, ManageInstanceResult>
{
    public Task<ManageInstanceResult> Handle(ManageInstanceCommand request, CancellationToken cancellationToken) =>
        vastAiInstancesGateway.ManageInstanceAsync(request.ToRequest(), cancellationToken);
}

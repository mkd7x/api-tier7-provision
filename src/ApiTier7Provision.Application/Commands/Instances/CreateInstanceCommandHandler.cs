using ApiTier7Provision.Application.Abstractions;
using MediatR;

namespace ApiTier7Provision.Application.Commands.Instances;

public sealed class CreateInstanceCommandHandler(IVastAiInstancesGateway vastAiInstancesGateway)
    : IRequestHandler<CreateInstanceCommand, CreateInstanceResult>
{
    public Task<CreateInstanceResult> Handle(CreateInstanceCommand request, CancellationToken cancellationToken) =>
        vastAiInstancesGateway.CreateInstanceAsync(request.ToRequest(), cancellationToken);
}

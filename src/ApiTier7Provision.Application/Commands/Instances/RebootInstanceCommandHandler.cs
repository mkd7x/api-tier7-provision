using ApiTier7Provision.Application.Abstractions;
using MediatR;

namespace ApiTier7Provision.Application.Commands.Instances;

public sealed class RebootInstanceCommandHandler(IVastAiInstancesGateway vastAiInstancesGateway)
    : IRequestHandler<RebootInstanceCommand, RebootInstanceResult>
{
    public Task<RebootInstanceResult> Handle(RebootInstanceCommand request, CancellationToken cancellationToken) =>
        vastAiInstancesGateway.RebootInstanceAsync(request.ToRequest(), cancellationToken);
}

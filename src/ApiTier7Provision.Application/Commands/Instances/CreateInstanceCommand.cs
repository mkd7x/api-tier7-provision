using MediatR;

namespace ApiTier7Provision.Application.Commands.Instances;

public sealed record CreateInstanceCommand(int AskId, string Payload) : IRequest<CreateInstanceResult>
{
    public CreateInstanceRequest ToRequest() => new(AskId, Payload);
}

public sealed record CreateInstanceRequest(int AskId, string Payload);

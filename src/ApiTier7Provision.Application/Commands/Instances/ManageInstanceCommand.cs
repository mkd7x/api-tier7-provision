using MediatR;

namespace ApiTier7Provision.Application.Commands.Instances;

public sealed record ManageInstanceCommand(int InstanceId, string Payload) : IRequest<ManageInstanceResult>
{
    public ManageInstanceRequest ToRequest() => new(InstanceId, Payload);
}

public sealed record ManageInstanceRequest(int InstanceId, string Payload);

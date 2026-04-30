using MediatR;

namespace ApiTier7Provision.Application.Commands.Instances;

public sealed record DestroyInstanceCommand(int InstanceId) : IRequest<DestroyInstanceResult>
{
    public DestroyInstanceRequest ToRequest() => new(InstanceId);
}

public sealed record DestroyInstanceRequest(int InstanceId);

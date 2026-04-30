using MediatR;

namespace ApiTier7Provision.Application.Commands.Instances;

public sealed record RebootInstanceCommand(int InstanceId) : IRequest<RebootInstanceResult>
{
    public RebootInstanceRequest ToRequest() => new(InstanceId);
}

public sealed record RebootInstanceRequest(int InstanceId);

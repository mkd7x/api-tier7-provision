using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Application.Commands.Instances;
using ApiTier7Provision.Application.Queries.Instances;

namespace ApiTier7Provision.Application.Tests;

public sealed class RebootInstanceCommandHandlerTests
{
    [Fact]
    public async Task Handle_UsesGatewayAndReturnsPayload()
    {
        var gateway = new FakeVastAiInstancesGateway
        {
            Result = new RebootInstanceResult(200, """{"success":true}""")
        };

        var handler = new RebootInstanceCommandHandler(gateway);
        var command = new RebootInstanceCommand(1234);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(200, result.StatusCode);
        Assert.Contains(@"""success"":true", result.Payload);
        Assert.NotNull(gateway.LastRequest);
        Assert.Equal(1234, gateway.LastRequest!.InstanceId);
    }

    private sealed class FakeVastAiInstancesGateway : IVastAiInstancesGateway
    {
        public RebootInstanceRequest? LastRequest { get; private set; }

        public RebootInstanceResult Result { get; init; } = new RebootInstanceResult(200, "{}");

        public Task<GetInstancesResult> GetInstancesAsync(VastAiInstancesRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(new GetInstancesResult(200, "{}"));

        public Task<SearchOffersResult> SearchOffersAsync(SearchOffersRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(new SearchOffersResult(200, "{}"));

        public Task<CreateInstanceResult> CreateInstanceAsync(CreateInstanceRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(new CreateInstanceResult(200, "{}"));

        public Task<ManageInstanceResult> ManageInstanceAsync(ManageInstanceRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(new ManageInstanceResult(200, "{}"));

        public Task<DestroyInstanceResult> DestroyInstanceAsync(DestroyInstanceRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(new DestroyInstanceResult(200, "{}"));

        public Task<RebootInstanceResult> RebootInstanceAsync(RebootInstanceRequest request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(Result);
        }
    }
}

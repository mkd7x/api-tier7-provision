using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Application.Commands.Instances;
using ApiTier7Provision.Application.Queries.Instances;

namespace ApiTier7Provision.Application.Tests;

public sealed class ManageInstanceCommandHandlerTests
{
    [Fact]
    public async Task Handle_UsesGatewayAndReturnsPayload()
    {
        var gateway = new FakeVastAiInstancesGateway
        {
            Result = new ManageInstanceResult(200, """{"success":true}""")
        };

        var handler = new ManageInstanceCommandHandler(gateway);
        var command = new ManageInstanceCommand(1234, """{"state":"stopped"}""");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(200, result.StatusCode);
        Assert.Contains(@"""success"":true", result.Payload);
        Assert.NotNull(gateway.LastRequest);
        Assert.Equal(1234, gateway.LastRequest!.InstanceId);
        Assert.Contains(@"""state"":""stopped""", gateway.LastRequest.Payload);
    }

    private sealed class FakeVastAiInstancesGateway : IVastAiInstancesGateway
    {
        public ManageInstanceRequest? LastRequest { get; private set; }

        public ManageInstanceResult Result { get; init; } = new ManageInstanceResult(200, "{}");

        public Task<GetInstancesResult> GetInstancesAsync(VastAiInstancesRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(new GetInstancesResult(200, "{}"));

        public Task<SearchOffersResult> SearchOffersAsync(SearchOffersRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(new SearchOffersResult(200, "{}"));

        public Task<CreateInstanceResult> CreateInstanceAsync(CreateInstanceRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(new CreateInstanceResult(200, "{}"));

        public Task<ManageInstanceResult> ManageInstanceAsync(ManageInstanceRequest request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(Result);
        }

        public Task<DestroyInstanceResult> DestroyInstanceAsync(DestroyInstanceRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(new DestroyInstanceResult(200, "{}"));

        public Task<RebootInstanceResult> RebootInstanceAsync(RebootInstanceRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(new RebootInstanceResult(200, "{}"));
    }
}

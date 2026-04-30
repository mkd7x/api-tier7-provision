using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Application.Commands.Instances;
using ApiTier7Provision.Application.Queries.Instances;

namespace ApiTier7Provision.Application.Tests;

public sealed class CreateInstanceCommandHandlerTests
{
    [Fact]
    public async Task Handle_UsesGatewayAndReturnsPayload()
    {
        var gateway = new FakeVastAiInstancesGateway
        {
            Result = new CreateInstanceResult(200, """{"success":true,"new_contract":1234568}""")
        };

        var handler = new CreateInstanceCommandHandler(gateway);
        var command = new CreateInstanceCommand(1234567, """{"image":"vastai/base-image:@vastai-automatic-tag"}""");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(200, result.StatusCode);
        Assert.Contains(@"""new_contract"":1234568", result.Payload);
        Assert.NotNull(gateway.LastRequest);
        Assert.Equal(1234567, gateway.LastRequest!.AskId);
        Assert.Contains(@"""image"":""vastai/base-image:@vastai-automatic-tag""", gateway.LastRequest.Payload);
    }

    private sealed class FakeVastAiInstancesGateway : IVastAiInstancesGateway
    {
        public CreateInstanceRequest? LastRequest { get; private set; }

        public CreateInstanceResult Result { get; init; } = new CreateInstanceResult(200, "{}");

        public Task<GetInstancesResult> GetInstancesAsync(
            VastAiInstancesRequest request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new GetInstancesResult(200, "{}"));

        public Task<SearchOffersResult> SearchOffersAsync(
            SearchOffersRequest request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new SearchOffersResult(200, "{}"));

        public Task<CreateInstanceResult> CreateInstanceAsync(
            CreateInstanceRequest request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(Result);
        }
    }
}

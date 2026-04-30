using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Application.Commands.Instances;
using ApiTier7Provision.Application.Queries.Instances;

namespace ApiTier7Provision.Application.Tests;

public sealed class SearchOffersQueryHandlerTests
{
    [Fact]
    public async Task Handle_UsesGatewayAndReturnsPayload()
    {
        var gateway = new FakeVastAiInstancesGateway
        {
            Result = new SearchOffersResult(200, """{"offers":[{"id":123}]}""")
        };

        var handler = new SearchOffersQueryHandler(gateway);
        var query = new SearchOffersQuery("""{"limit":100,"verified":{"eq":true}}""");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(200, result.StatusCode);
        Assert.Contains(@"""offers""", result.Payload);
        Assert.NotNull(gateway.LastRequest);
        Assert.Contains(@"""limit"":100", gateway.LastRequest!.Payload);
    }

    private sealed class FakeVastAiInstancesGateway : IVastAiInstancesGateway
    {
        public SearchOffersRequest? LastRequest { get; private set; }

        public SearchOffersResult Result { get; init; } = new SearchOffersResult(200, "{}");

        public Task<GetInstancesResult> GetInstancesAsync(
            VastAiInstancesRequest request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new GetInstancesResult(200, "{}"));

        public Task<SearchOffersResult> SearchOffersAsync(
            SearchOffersRequest request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(Result);
        }

        public Task<CreateInstanceResult> CreateInstanceAsync(
            CreateInstanceRequest request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new CreateInstanceResult(200, "{}"));

        public Task<ManageInstanceResult> ManageInstanceAsync(
            ManageInstanceRequest request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new ManageInstanceResult(200, "{}"));

        public Task<DestroyInstanceResult> DestroyInstanceAsync(
            DestroyInstanceRequest request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new DestroyInstanceResult(200, "{}"));

        public Task<RebootInstanceResult> RebootInstanceAsync(
            RebootInstanceRequest request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new RebootInstanceResult(200, "{}"));
    }
}

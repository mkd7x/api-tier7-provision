using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Application.Commands.Instances;
using ApiTier7Provision.Application.Queries.Instances;

namespace ApiTier7Provision.Application.Tests;

public sealed class GetInstancesQueryHandlerTests
{
    [Fact]
    public async Task Handle_UsesGatewayAndReturnsPayload()
    {
        var gateway = new FakeVastAiInstancesGateway
        {
            Result = new GetInstancesResult(200, """{"success":true,"instances_found":1}""")
        };

        var handler = new GetInstancesQueryHandler(gateway);
        var query = new GetInstancesQuery(
            100,
            "abc",
            """[{"col":"id","dir":"asc"}]""",
            """["id","label"]""",
            """{"actual_status":{"eq":"running"}}""");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(200, result.StatusCode);
        Assert.Contains(@"""instances_found"":1", result.Payload);
        Assert.NotNull(gateway.LastRequest);
        Assert.Equal(25, gateway.LastRequest!.Limit);
        Assert.Equal("abc", gateway.LastRequest.AfterToken);
    }

    [Fact]
    public void ToRequest_NormalizesLimitWhenLessThanOrEqualToZero()
    {
        var query = new GetInstancesQuery(
            0,
            null,
            null,
            null,
            null);

        var request = query.ToRequest();

        Assert.Equal(5, request.Limit);
    }

    private sealed class FakeVastAiInstancesGateway : IVastAiInstancesGateway
    {
        public VastAiInstancesRequest? LastRequest { get; private set; }

        public GetInstancesResult Result { get; init; } = new GetInstancesResult(200, "{}");

        public Task<GetInstancesResult> GetInstancesAsync(VastAiInstancesRequest request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(Result);
        }

        public Task<SearchOffersResult> SearchOffersAsync(SearchOffersRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(new SearchOffersResult(200, "{}"));

        public Task<CreateInstanceResult> CreateInstanceAsync(
            CreateInstanceRequest request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new CreateInstanceResult(200, "{}"));
    }
}

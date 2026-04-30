using MediatR;

namespace ApiTier7Provision.Application.Queries.Instances;

public sealed record SearchOffersQuery(string Payload) : IRequest<SearchOffersResult>
{
    public SearchOffersRequest ToRequest() => new(Payload);
}

public sealed record SearchOffersRequest(string Payload);

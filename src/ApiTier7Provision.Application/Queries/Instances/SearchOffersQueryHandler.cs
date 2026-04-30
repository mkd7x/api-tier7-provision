using ApiTier7Provision.Application.Abstractions;
using MediatR;

namespace ApiTier7Provision.Application.Queries.Instances;

public sealed class SearchOffersQueryHandler(IVastAiInstancesGateway vastAiInstancesGateway)
    : IRequestHandler<SearchOffersQuery, SearchOffersResult>
{
    public Task<SearchOffersResult> Handle(SearchOffersQuery request, CancellationToken cancellationToken) =>
        vastAiInstancesGateway.SearchOffersAsync(request.ToRequest(), cancellationToken);
}

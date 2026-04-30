using MediatR;

namespace ApiTier7Provision.Application.Queries.Instances;

public sealed record GetInstancesQuery(
    int? Limit,
    string? AfterToken,
    string? OrderBy,
    string? SelectCols,
    string? SelectFilters) : IRequest<GetInstancesResult>
{
    public VastAiInstancesRequest ToRequest()
    {
        var normalizedLimit = Limit switch
        {
            <= 0 => 5,
            > 25 => 25,
            _ => Limit
        };

        return new VastAiInstancesRequest(
            normalizedLimit,
            AfterToken,
            OrderBy,
            SelectCols,
            SelectFilters);
    }
}

public sealed record VastAiInstancesRequest(
    int? Limit,
    string? AfterToken,
    string? OrderBy,
    string? SelectCols,
    string? SelectFilters);

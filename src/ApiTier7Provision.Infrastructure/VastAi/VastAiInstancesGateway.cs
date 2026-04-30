using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Application.Queries.Instances;

namespace ApiTier7Provision.Infrastructure.VastAi;

public sealed class VastAiInstancesGateway(HttpClient httpClient)
    : VastAiHttpClientBase(httpClient), IVastAiInstancesGateway
{
    public async Task<GetInstancesResult> GetInstancesAsync(
        VastAiInstancesRequest request,
        CancellationToken cancellationToken)
    {
        var queryString = BuildQueryString(new Dictionary<string, string?>
        {
            ["limit"] = request.Limit?.ToString(),
            ["after_token"] = request.AfterToken,
            ["order_by"] = request.OrderBy,
            ["select_cols"] = request.SelectCols,
            ["select_filters"] = request.SelectFilters
        });

        var requestUri = "/api/v1/instances/";
        if (!string.IsNullOrWhiteSpace(queryString))
        {
            requestUri = $"{requestUri}?{queryString}";
        }

        using var response = await HttpClient.GetAsync(requestUri, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        return new GetInstancesResult((int)response.StatusCode, payload);
    }
}

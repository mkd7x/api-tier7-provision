using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Application.Queries.Instances;
using System.Text;

namespace ApiTier7Provision.Infrastructure.ApiClients.VastAi;

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

    public async Task<SearchOffersResult> SearchOffersAsync(
        SearchOffersRequest request,
        CancellationToken cancellationToken)
    {
        using var content = new StringContent(request.Payload, Encoding.UTF8, "application/json");
        using var response = await HttpClient.PostAsync("/api/v0/bundles/", content, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        return new SearchOffersResult((int)response.StatusCode, payload);
    }
}

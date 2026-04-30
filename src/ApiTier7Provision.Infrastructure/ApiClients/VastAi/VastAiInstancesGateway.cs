using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Application.Commands.Instances;
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

    public async Task<CreateInstanceResult> CreateInstanceAsync(
        CreateInstanceRequest request,
        CancellationToken cancellationToken)
    {
        using var content = new StringContent(request.Payload, Encoding.UTF8, "application/json");
        using var response = await HttpClient.PutAsync($"/api/v0/asks/{request.AskId}/", content, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        return new CreateInstanceResult((int)response.StatusCode, payload);
    }

    public async Task<ManageInstanceResult> ManageInstanceAsync(
        ManageInstanceRequest request,
        CancellationToken cancellationToken)
    {
        using var content = new StringContent(request.Payload, Encoding.UTF8, "application/json");
        using var response = await HttpClient.PutAsync($"/api/v0/instances/{request.InstanceId}/", content, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        return new ManageInstanceResult((int)response.StatusCode, payload);
    }

    public async Task<DestroyInstanceResult> DestroyInstanceAsync(
        DestroyInstanceRequest request,
        CancellationToken cancellationToken)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/v0/instances/{request.InstanceId}/");
        using var response = await HttpClient.SendAsync(requestMessage, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        return new DestroyInstanceResult((int)response.StatusCode, payload);
    }

    public async Task<RebootInstanceResult> RebootInstanceAsync(
        RebootInstanceRequest request,
        CancellationToken cancellationToken)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/api/v0/instances/reboot/{request.InstanceId}/");
        using var response = await HttpClient.SendAsync(requestMessage, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        return new RebootInstanceResult((int)response.StatusCode, payload);
    }
}

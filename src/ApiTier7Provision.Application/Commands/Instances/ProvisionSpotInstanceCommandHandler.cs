using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Application.Models;
using ApiTier7Provision.Application.Queries.Instances;
using ApiTier7Provision.Domain.Entities;
using MediatR;
using System.Text.Json;

namespace ApiTier7Provision.Application.Commands.Instances;

public sealed class ProvisionSpotInstanceCommandHandler(
    IModelTemplateRepository modelTemplateRepository,
    IProvisionedInstanceRepository provisionedInstanceRepository,
    IVastAiInstancesGateway vastAiInstancesGateway,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<ProvisionSpotInstanceCommand, InstanceSpotOrderResponse>
{
    public async Task<InstanceSpotOrderResponse> Handle(
        ProvisionSpotInstanceCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedModel = request.Model.Trim();
        var modelTemplate = await modelTemplateRepository.GetByModelAsync(normalizedModel, cancellationToken);

        if (modelTemplate is null)
        {
            throw new ModelTemplateNotFoundException(normalizedModel);
        }

        if (modelTemplate.SupportedGpus.Length == 0)
        {
            throw new NoQualifyingSpotOfferException(normalizedModel);
        }

        var searchPayload = BuildSearchPayload(modelTemplate.SupportedGpus, request.MaxDPH);
        var searchResult = await vastAiInstancesGateway.SearchOffersAsync(
            new SearchOffersRequest(searchPayload),
            cancellationToken);

        EnsureSuccess(searchResult.StatusCode, searchResult.Payload, "search offers");

        var offer = SelectBestOffer(
            searchResult.Payload,
            request.MaxDPH,
            request.MaxIngressCost,
            request.MaxEgressCost,
            normalizedModel);
        var createPayload = JsonSerializer.Serialize(new { template_hash_id = modelTemplate.ProvisioningTemplateUuid });

        var createResult = await vastAiInstancesGateway.CreateInstanceAsync(
            new CreateInstanceRequest((int)offer.AskId, createPayload),
            cancellationToken);

        EnsureSuccess(createResult.StatusCode, createResult.Payload, "create instance");

        var instanceId = ParseProvisionedInstanceId(createResult.Payload);
        var provisionedInstance = new ProvisionedInstance
        {
            ModelTemplateId = modelTemplate.Id,
            VastInstanceId = instanceId,
            Gpu = offer.Gpu,
            TotalCostPerHour = offer.TotalCostPerHour,
            IngressCost = offer.IngressCost,
            EgressCost = offer.EgressCost,
            ProvisionedAtUtc = dateTimeProvider.UtcNow
        };

        await provisionedInstanceRepository.AddAsync(provisionedInstance, cancellationToken);

        return new InstanceSpotOrderResponse
        {
            InstanceId = instanceId,
            Gpu = offer.Gpu,
            TotalCostPerHour = offer.TotalCostPerHour,
            IngressCost = offer.IngressCost,
            EgressCost = offer.EgressCost
        };
    }

    private static string BuildSearchPayload(IReadOnlyCollection<string> supportedGpus, decimal? maxDph)
    {
        var payload = new Dictionary<string, object?>
        {
            ["limit"] = 100,
            ["type"] = "bid",
            ["verified"] = new Dictionary<string, object?> { ["eq"] = true },
            ["rentable"] = new Dictionary<string, object?> { ["eq"] = true },
            ["rented"] = new Dictionary<string, object?> { ["eq"] = false },
            ["gpu_name"] = new Dictionary<string, object?> { ["in"] = supportedGpus.ToArray() },
            ["order"] = new object[] { new object[] { "dph_total", "asc" } }
        };

        if (maxDph is not null)
        {
            payload["dph_total"] = new Dictionary<string, object?> { ["lte"] = maxDph.Value };
        }

        return JsonSerializer.Serialize(payload);
    }

    private static SpotOffer SelectBestOffer(
        string payload,
        decimal? maxDph,
        decimal? maxIngressCost,
        decimal? maxEgressCost,
        string model)
    {
        var offers = ParseOffers(payload);

        var bestOffer = offers
            .Where(x => maxDph is null || x.TotalCostPerHour <= maxDph.Value)
            .Where(x => maxIngressCost is null || x.IngressCost <= maxIngressCost.Value)
            .Where(x => maxEgressCost is null || x.EgressCost <= maxEgressCost.Value)
            .OrderBy(x => x.TotalCostPerHour)
            .FirstOrDefault();

        return bestOffer ?? throw new NoQualifyingSpotOfferException(model);
    }

    private static IReadOnlyList<SpotOffer> ParseOffers(string payload)
    {
        try
        {
            using var document = JsonDocument.Parse(payload);
            if (!document.RootElement.TryGetProperty("offers", out var offersElement))
            {
                throw new VastAiMalformedResponseException("search offers");
            }

            return offersElement.ValueKind switch
            {
                JsonValueKind.Array => offersElement.EnumerateArray().Select(ParseOffer).ToArray(),
                JsonValueKind.Object => [ParseOffer(offersElement)],
                _ => throw new VastAiMalformedResponseException("search offers")
            };
        }
        catch (JsonException)
        {
            throw new VastAiMalformedResponseException("search offers");
        }
    }

    private static SpotOffer ParseOffer(JsonElement offerElement)
    {
        var askId = GetRequiredInt64(offerElement, "id", "search offers");
        var gpu = GetRequiredString(offerElement, "gpu_name", "search offers");
        var totalCostPerHour = GetRequiredDecimal(offerElement, "dph_total", "search offers");
        var ingressCost = GetRequiredDecimal(offerElement, "inet_down_cost", "search offers");
        var egressCost = GetRequiredDecimal(offerElement, "inet_up_cost", "search offers");

        return new SpotOffer(askId, gpu, totalCostPerHour, ingressCost, egressCost);
    }

    private static long ParseProvisionedInstanceId(string payload)
    {
        try
        {
            using var document = JsonDocument.Parse(payload);
            return GetRequiredInt64(document.RootElement, "new_contract", "create instance");
        }
        catch (JsonException)
        {
            throw new VastAiMalformedResponseException("create instance");
        }
    }

    private static void EnsureSuccess(int statusCode, string payload, string operation)
    {
        if (statusCode is < 200 or >= 300)
        {
            throw new VastAiProvisioningException(operation, statusCode, payload);
        }
    }

    private static long GetRequiredInt64(JsonElement element, string propertyName, string operation)
    {
        if (element.TryGetProperty(propertyName, out var property)
            && property.ValueKind == JsonValueKind.Number
            && property.TryGetInt64(out var value))
        {
            return value;
        }

        throw new VastAiMalformedResponseException(operation);
    }

    private static decimal GetRequiredDecimal(JsonElement element, string propertyName, string operation)
    {
        if (element.TryGetProperty(propertyName, out var property)
            && property.ValueKind == JsonValueKind.Number
            && property.TryGetDecimal(out var value))
        {
            return value;
        }

        throw new VastAiMalformedResponseException(operation);
    }

    private static string GetRequiredString(JsonElement element, string propertyName, string operation)
    {
        if (element.TryGetProperty(propertyName, out var property)
            && property.ValueKind == JsonValueKind.String
            && property.GetString() is { Length: > 0 } value)
        {
            return value;
        }

        throw new VastAiMalformedResponseException(operation);
    }

    private sealed record SpotOffer(
        long AskId,
        string Gpu,
        decimal TotalCostPerHour,
        decimal IngressCost,
        decimal EgressCost);
}
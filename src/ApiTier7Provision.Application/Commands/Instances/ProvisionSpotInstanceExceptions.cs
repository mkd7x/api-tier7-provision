namespace ApiTier7Provision.Application.Commands.Instances;

public sealed class ModelTemplateNotFoundException(string model)
    : Exception($"No model template was found for model '{model}'.");

public sealed class NoQualifyingSpotOfferException(string model)
    : Exception($"No qualifying spot offer was found for model '{model}'.");

public sealed class VastAiProvisioningException(string operation, int statusCode, string payload)
    : Exception($"Vast.AI {operation} failed with status code {statusCode}.")
{
    public string Operation { get; } = operation;

    public int StatusCode { get; } = statusCode;

    public string Payload { get; } = payload;
}

public sealed class VastAiMalformedResponseException(string operation)
    : Exception($"Vast.AI {operation} returned a malformed response.");
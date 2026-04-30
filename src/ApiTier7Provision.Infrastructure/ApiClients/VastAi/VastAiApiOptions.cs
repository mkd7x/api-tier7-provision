namespace ApiTier7Provision.Infrastructure.ApiClients.VastAi;

public sealed class VastAiApiOptions
{
    public const string SectionName = "VastAi";

    public string BaseUrl { get; set; } = "https://console.vast.ai";

    public string ApiKey { get; set; } = string.Empty;
}

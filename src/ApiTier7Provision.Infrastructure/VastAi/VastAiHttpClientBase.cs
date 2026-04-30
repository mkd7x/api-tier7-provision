namespace ApiTier7Provision.Infrastructure.VastAi;

public abstract class VastAiHttpClientBase(HttpClient httpClient)
{
    protected HttpClient HttpClient { get; } = httpClient;

    protected static string BuildQueryString(IReadOnlyDictionary<string, string?> queryParameters)
    {
        var pairs = queryParameters
            .Where(static x => !string.IsNullOrWhiteSpace(x.Value))
            .Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value!)}");

        return string.Join("&", pairs);
    }
}

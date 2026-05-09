namespace ApiTier7Provision.Application.Models;
public record ModelTemplateResponse(
    string Model,
    string[] SupportedGpus
);
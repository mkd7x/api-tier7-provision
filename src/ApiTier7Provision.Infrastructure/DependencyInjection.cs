using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Infrastructure.Persistence;
using ApiTier7Provision.Infrastructure.Persistence.Repositories;
using ApiTier7Provision.Infrastructure.Services;
using ApiTier7Provision.Infrastructure.ApiClients.VastAi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace ApiTier7Provision.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IHealthCheckLogRepository, HealthCheckLogRepository>();
        services.AddScoped<IModelTemplateRepository, ModelTemplateRepository>();
        services.AddScoped<ModelTemplateSeeder>();
        services.AddScoped<IProvisionedInstanceRepository, ProvisionedInstanceRepository>();
        services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();
        services.AddOptions<ModelTemplateCatalogOptions>()
            .Bind(configuration.GetSection(ModelTemplateCatalogOptions.SectionName));
        services.AddOptions<VastAiApiOptions>()
            .Bind(configuration.GetSection(VastAiApiOptions.SectionName));

        services.AddHttpClient<IVastAiInstancesGateway, VastAiInstancesGateway>((serviceProvider, httpClient) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<VastAiApiOptions>>().Value;
            if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUri))
            {
                throw new InvalidOperationException(
                    $"Configuration section '{VastAiApiOptions.SectionName}:BaseUrl' must be a valid absolute URI.");
            }

            httpClient.BaseAddress = baseUri;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(options.ApiKey))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
            }
        });

        return services;
    }
}

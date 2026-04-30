using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Infrastructure.Persistence;
using ApiTier7Provision.Infrastructure.Persistence.Repositories;
using ApiTier7Provision.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTier7Provision.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IHealthCheckLogRepository, HealthCheckLogRepository>();
        services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();

        return services;
    }
}

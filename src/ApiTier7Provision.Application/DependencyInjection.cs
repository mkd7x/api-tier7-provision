using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ApiTier7Provision.Application.Models;

namespace ApiTier7Provision.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssemblyContaining<InstanceSpotOrderRequestValidator>();

        return services;
    }
}

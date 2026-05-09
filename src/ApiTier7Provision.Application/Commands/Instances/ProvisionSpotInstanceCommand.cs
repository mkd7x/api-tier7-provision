using ApiTier7Provision.Application.Models;
using MediatR;

namespace ApiTier7Provision.Application.Commands.Instances;

public sealed record ProvisionSpotInstanceCommand(
    string Model,
    decimal? MaxDPH,
    decimal? MaxIngressCost,
    decimal? MaxEgressCost) : IRequest<InstanceSpotOrderResponse>;
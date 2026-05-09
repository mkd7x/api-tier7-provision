namespace ApiTier7Provision.Application.Models;

public sealed class InstanceSpotOrderResponse
{
	public required long InstanceId { get; init; }

	public required string Gpu { get; init; }

	public required decimal TotalCostPerHour { get; init; }

	public required decimal IngressCost { get; init; }

	public required decimal EgressCost { get; init; }
}

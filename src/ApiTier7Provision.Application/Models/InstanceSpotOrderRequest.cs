namespace ApiTier7Provision.Application.Models;

public class InstanceSpotOrderRequest
{
    public required string Model { get; set; }

    public decimal? MaxDPH { get; set; }

    public decimal? MaxIngressCost { get; set; }

    public decimal? MaxEgressCost { get; set; }

}
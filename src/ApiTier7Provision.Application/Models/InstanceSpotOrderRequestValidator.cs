using FluentValidation;

namespace ApiTier7Provision.Application.Models;

public sealed class InstanceSpotOrderRequestValidator : AbstractValidator<InstanceSpotOrderRequest>
{
    public InstanceSpotOrderRequestValidator()
    {
        RuleFor(x => x.Model)
            .NotEmpty();

        RuleFor(x => x.MaxDPH)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxDPH.HasValue);

        RuleFor(x => x.MaxIngressCost)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxIngressCost.HasValue);

        RuleFor(x => x.MaxEgressCost)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxEgressCost.HasValue);
    }
}
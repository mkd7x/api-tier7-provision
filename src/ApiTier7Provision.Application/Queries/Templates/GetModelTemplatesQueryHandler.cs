using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Application.Models;
using MediatR;

public sealed class GetModelTemplatesQueryHandler(IModelTemplateRepository modelTemplateRepository) : IRequestHandler<GetModelTemplatesQuery, ModelTemplateResponse>
{
    public async Task<ModelTemplateResponse> Handle(GetModelTemplatesQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Model templates are currently unavailable. This endpoint will be implemented in a future release.");
    }
}
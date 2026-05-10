using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Application.Commands.Instances;
using ApiTier7Provision.Application.Models;
using MediatR;

public sealed class GetModelTemplatesQueryHandler(IModelTemplateRepository modelTemplateRepository) : IRequestHandler<GetModelTemplatesQuery, ModelTemplateResponse>
{
    public async Task<ModelTemplateResponse> Handle(GetModelTemplatesQuery query, CancellationToken cancellationToken)
    {
        var normalizedModel = query.Model?.Trim();

        if (string.IsNullOrWhiteSpace(normalizedModel))
        {
            throw new ArgumentException("A model query parameter is required.", nameof(query));
        }

        var modelTemplate = await modelTemplateRepository.GetByModelAsync(normalizedModel, cancellationToken);

        if (modelTemplate is null)
        {
            throw new ModelTemplateNotFoundException(normalizedModel);
        }

        return new ModelTemplateResponse(modelTemplate.Model, modelTemplate.SupportedGpus);
    }
}

using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Domain.Entities;
using MediatR;

public class CreateModelTemplateCommandHandler : IRequestHandler<CreateModelTemplateCommand, Unit>
{
    private readonly IModelTemplateRepository _repository;

    public CreateModelTemplateCommandHandler(IModelTemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(CreateModelTemplateCommand request, CancellationToken cancellationToken)
    {
        var modelTemplate = new ModelTemplate
        {
            Model = request.Model,
            SupportedGpus = request.SupportedGpus,
            ProvisioningTemplateUuid = request.TemplateUuid
        };
        await _repository.AddAsync(modelTemplate, cancellationToken);
        return Unit.Value;
    }
}
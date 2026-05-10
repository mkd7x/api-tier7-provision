using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Application.Commands.Instances;
using MediatR;

public sealed record DeleteModelTemplateCommand(string? Model) : IRequest<Unit>;

public sealed class DeleteModelTemplateCommandHandler(IModelTemplateRepository modelTemplateRepository)
    : IRequestHandler<DeleteModelTemplateCommand, Unit>
{
    public async Task<Unit> Handle(DeleteModelTemplateCommand request, CancellationToken cancellationToken)
    {
        var normalizedModel = request.Model?.Trim();

        if (string.IsNullOrWhiteSpace(normalizedModel))
        {
            throw new ArgumentException("A model query parameter is required.", nameof(request.Model));
        }

        var deleteStatus = await modelTemplateRepository.DeleteByModelAsync(normalizedModel, cancellationToken);

        return deleteStatus switch
        {
            ModelTemplateDeleteStatus.Deleted => Unit.Value,
            ModelTemplateDeleteStatus.NotFound => throw new ModelTemplateNotFoundException(normalizedModel),
            ModelTemplateDeleteStatus.InUse => throw new ModelTemplateInUseException(normalizedModel),
            _ => throw new InvalidOperationException($"Unsupported delete status '{deleteStatus}'.")
        };
    }
}
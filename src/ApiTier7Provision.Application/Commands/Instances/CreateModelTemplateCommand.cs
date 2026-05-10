using MediatR;

public record CreateModelTemplateCommand(string Model, string[] SupportedGpus, string TemplateUuid) : IRequest<Unit>;
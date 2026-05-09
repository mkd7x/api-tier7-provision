using MediatR;
using ApiTier7Provision.Application.Models;

public sealed record GetModelTemplatesQuery(
    string? Model
) : IRequest<ModelTemplateResponse>;
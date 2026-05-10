using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Application.Commands.Instances;
using ApiTier7Provision.Domain.Entities;
using MediatR;

namespace ApiTier7Provision.Application.Tests;

public sealed class DeleteModelTemplateCommandHandlerTests
{
    [Fact]
    public async Task Handle_DeletesTemplate_WhenModelExists()
    {
        var repository = new FakeModelTemplateRepository(ModelTemplateDeleteStatus.Deleted);
        var handler = new DeleteModelTemplateCommandHandler(repository);

        var result = await handler.Handle(new DeleteModelTemplateCommand("  llama-3  "), CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        Assert.Equal("llama-3", repository.LastDeletedModel);
    }

    [Fact]
    public async Task Handle_ThrowsWhenTemplateDoesNotExist()
    {
        var repository = new FakeModelTemplateRepository(ModelTemplateDeleteStatus.NotFound);
        var handler = new DeleteModelTemplateCommandHandler(repository);

        await Assert.ThrowsAsync<ModelTemplateNotFoundException>(() =>
            handler.Handle(new DeleteModelTemplateCommand("missing-model"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ThrowsWhenTemplateIsInUse()
    {
        var repository = new FakeModelTemplateRepository(ModelTemplateDeleteStatus.InUse);
        var handler = new DeleteModelTemplateCommandHandler(repository);

        await Assert.ThrowsAsync<ModelTemplateInUseException>(() =>
            handler.Handle(new DeleteModelTemplateCommand("llama-3"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ThrowsWhenModelIsMissing()
    {
        var repository = new FakeModelTemplateRepository(ModelTemplateDeleteStatus.Deleted);
        var handler = new DeleteModelTemplateCommandHandler(repository);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.Handle(new DeleteModelTemplateCommand("   "), CancellationToken.None));
    }

    private sealed class FakeModelTemplateRepository(ModelTemplateDeleteStatus deleteStatus) : IModelTemplateRepository
    {
        public string? LastDeletedModel { get; private set; }

        public Task<ModelTemplate?> GetByModelAsync(string model, CancellationToken cancellationToken) =>
            Task.FromResult<ModelTemplate?>(null);

        public Task<List<ModelTemplate>> GetAllAsync(CancellationToken cancellationToken) =>
            Task.FromResult(new List<ModelTemplate>());

        public Task AddAsync(ModelTemplate modelTemplate, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task<ModelTemplateDeleteStatus> DeleteByModelAsync(string model, CancellationToken cancellationToken)
        {
            LastDeletedModel = model;
            return Task.FromResult(deleteStatus);
        }
    }
}
using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Application.Commands.Instances;
using ApiTier7Provision.Domain.Entities;

namespace ApiTier7Provision.Application.Tests;

public sealed class GetModelTemplatesQueryHandlerTests
{
  [Fact]
  public async Task Handle_ReturnsMappedTemplate_WhenModelExists()
  {
    var repository = new FakeModelTemplateRepository(
      new ModelTemplate
      {
        Id = 7,
        Model = "llama-3",
        SupportedGpus = ["RTX 4090", "A100"],
        ProvisioningTemplateUuid = "template-hash-123"
      });

    var handler = new GetModelTemplatesQueryHandler(repository);

    var result = await handler.Handle(new GetModelTemplatesQuery("  llama-3  "), CancellationToken.None);

    Assert.Equal("llama-3", result.Model);
    Assert.Equal(["RTX 4090", "A100"], result.SupportedGpus);
  }

  [Fact]
  public async Task Handle_ThrowsWhenTemplateDoesNotExist()
  {
    var repository = new FakeModelTemplateRepository(null);
    var handler = new GetModelTemplatesQueryHandler(repository);

    await Assert.ThrowsAsync<ModelTemplateNotFoundException>(() =>
      handler.Handle(new GetModelTemplatesQuery("missing-model"), CancellationToken.None));
  }

  [Fact]
  public async Task Handle_ThrowsWhenModelIsMissing()
  {
    var repository = new FakeModelTemplateRepository(null);
    var handler = new GetModelTemplatesQueryHandler(repository);

    await Assert.ThrowsAsync<ArgumentException>(() =>
      handler.Handle(new GetModelTemplatesQuery("   "), CancellationToken.None));
  }

  private sealed class FakeModelTemplateRepository(ModelTemplate? modelTemplate) : IModelTemplateRepository
  {
    public Task<ModelTemplate?> GetByModelAsync(string model, CancellationToken cancellationToken) =>
      Task.FromResult(modelTemplate is not null && string.Equals(modelTemplate.Model, model, StringComparison.OrdinalIgnoreCase)
        ? modelTemplate
        : null);

    public Task<List<ModelTemplate>> GetAllAsync(CancellationToken cancellationToken) =>
      Task.FromResult(modelTemplate is null ? new List<ModelTemplate>() : new List<ModelTemplate> { modelTemplate });

    public Task AddAsync(ModelTemplate modelTemplateToAdd, CancellationToken cancellationToken) =>
      Task.CompletedTask;

    public Task<ModelTemplateDeleteStatus> DeleteByModelAsync(string model, CancellationToken cancellationToken) =>
      Task.FromResult(ModelTemplateDeleteStatus.NotFound);
  }
}
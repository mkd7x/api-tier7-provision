using ApiTier7Provision.Application.Abstractions;
using ApiTier7Provision.Application.Commands.Instances;
using ApiTier7Provision.Application.Queries.Instances;
using ApiTier7Provision.Domain.Entities;

namespace ApiTier7Provision.Application.Tests;

public sealed class ProvisionSpotInstanceCommandHandlerTests
{
  [Fact]
  public async Task Handle_SelectsCheapestQualifyingOffer_CreatesInstance_AndPersistsRecord()
  {
    var modelTemplateRepository = new FakeModelTemplateRepository(
      new ModelTemplate
      {
        Id = 42,
        Model = "llama-3",
        SupportedGpus = ["RTX 4090", "A100"],
        ProvisioningTemplateUuid = "template-hash-123"
      });

    var provisionedInstanceRepository = new FakeProvisionedInstanceRepository();
    var gateway = new FakeVastAiInstancesGateway
    {
      SearchResult = new SearchOffersResult(
        200,
        """
        {
          "offers": [
          {
            "id": 1001,
            "gpu_name": "A100",
            "dph_total": 0.42,
            "inet_down_cost": 0.02,
            "inet_up_cost": 0.03
          },
          {
            "id": 1002,
            "gpu_name": "RTX 4090",
            "dph_total": 0.30,
            "inet_down_cost": 0.04,
            "inet_up_cost": 0.05
          }
          ]
        }
        """),
      CreateResult = new CreateInstanceResult(200, """{"success":true,"new_contract":777001}""")
    };

    var handler = CreateHandler(modelTemplateRepository, provisionedInstanceRepository, gateway);

    var result = await handler.Handle(
      new ProvisionSpotInstanceCommand("llama-3", 0.50m, 0.10m, 0.10m),
      CancellationToken.None);

    Assert.Equal(777001, result.InstanceId);
    Assert.Equal("RTX 4090", result.Gpu);
    Assert.Equal(0.30m, result.TotalCostPerHour);
    Assert.Equal(0.04m, result.IngressCost);
    Assert.Equal(0.05m, result.EgressCost);

    Assert.NotNull(gateway.LastSearchRequest);
    Assert.Contains("\"type\":\"bid\"", gateway.LastSearchRequest!.Payload);
    Assert.Contains("\"RTX 4090\"", gateway.LastSearchRequest.Payload);
    Assert.Contains("\"A100\"", gateway.LastSearchRequest.Payload);
    Assert.Contains("\"lte\":0.50", gateway.LastSearchRequest.Payload);

    Assert.NotNull(gateway.LastCreateRequest);
    Assert.Equal(1002, gateway.LastCreateRequest!.AskId);
    Assert.Contains("\"template_hash_id\":\"template-hash-123\"", gateway.LastCreateRequest.Payload);

    var persistedInstance = Assert.Single(provisionedInstanceRepository.AddedInstances);
    Assert.Equal(42, persistedInstance.ModelTemplateId);
    Assert.Equal(777001, persistedInstance.VastInstanceId);
    Assert.Equal("RTX 4090", persistedInstance.Gpu);
    Assert.Equal(0.04m, persistedInstance.IngressCost);
    Assert.Equal(0.05m, persistedInstance.EgressCost);
    Assert.Equal(new DateTime(2026, 5, 9, 12, 0, 0, DateTimeKind.Utc), persistedInstance.ProvisionedAtUtc);
  }

  [Fact]
  public async Task Handle_ThrowsWhenTemplateDoesNotExist()
  {
    var modelTemplateRepository = new FakeModelTemplateRepository(null);
    var provisionedInstanceRepository = new FakeProvisionedInstanceRepository();
    var gateway = new FakeVastAiInstancesGateway();
    var handler = CreateHandler(modelTemplateRepository, provisionedInstanceRepository, gateway);

    await Assert.ThrowsAsync<ModelTemplateNotFoundException>(() =>
      handler.Handle(new ProvisionSpotInstanceCommand("missing-model", null, null, null), CancellationToken.None));

    Assert.Null(gateway.LastSearchRequest);
    Assert.Empty(provisionedInstanceRepository.AddedInstances);
  }

  [Fact]
  public async Task Handle_ThrowsWhenNoOffersMatchIngressLimit()
  {
    var modelTemplateRepository = new FakeModelTemplateRepository(
      new ModelTemplate
      {
        Id = 10,
        Model = "llama-3",
        SupportedGpus = ["RTX 4090"],
        ProvisioningTemplateUuid = "template-hash-123"
      });

    var provisionedInstanceRepository = new FakeProvisionedInstanceRepository();
    var gateway = new FakeVastAiInstancesGateway
    {
      SearchResult = new SearchOffersResult(
        200,
        """
        {
          "offers": [
          {
            "id": 1002,
            "gpu_name": "RTX 4090",
            "dph_total": 0.30,
            "inet_down_cost": 0.40,
            "inet_up_cost": 0.35
          }
          ]
        }
        """)
    };

    var handler = CreateHandler(modelTemplateRepository, provisionedInstanceRepository, gateway);

    await Assert.ThrowsAsync<NoQualifyingSpotOfferException>(() =>
      handler.Handle(new ProvisionSpotInstanceCommand("llama-3", null, 0.20m, null), CancellationToken.None));

    Assert.Empty(provisionedInstanceRepository.AddedInstances);
  }

  [Fact]
  public async Task Handle_DoesNotPersistWhenCreateFails()
  {
    var modelTemplateRepository = new FakeModelTemplateRepository(
      new ModelTemplate
      {
        Id = 11,
        Model = "llama-3",
        SupportedGpus = ["RTX 4090"],
        ProvisioningTemplateUuid = "template-hash-123"
      });

    var provisionedInstanceRepository = new FakeProvisionedInstanceRepository();
    var gateway = new FakeVastAiInstancesGateway
    {
      SearchResult = new SearchOffersResult(
        200,
        """
        {
          "offers": [
          {
            "id": 1002,
            "gpu_name": "RTX 4090",
            "dph_total": 0.30,
            "inet_down_cost": 0.04,
            "inet_up_cost": 0.05
          }
          ]
        }
        """),
      CreateResult = new CreateInstanceResult(500, """{"success":false,"msg":"create failed"}""")
    };

    var handler = CreateHandler(modelTemplateRepository, provisionedInstanceRepository, gateway);

    var exception = await Assert.ThrowsAsync<VastAiProvisioningException>(() =>
      handler.Handle(new ProvisionSpotInstanceCommand("llama-3", null, null, null), CancellationToken.None));

    Assert.Equal("create instance", exception.Operation);
    Assert.Empty(provisionedInstanceRepository.AddedInstances);
  }

  private static ProvisionSpotInstanceCommandHandler CreateHandler(
    IModelTemplateRepository modelTemplateRepository,
    FakeProvisionedInstanceRepository provisionedInstanceRepository,
    FakeVastAiInstancesGateway gateway) =>
    new(
      modelTemplateRepository,
      provisionedInstanceRepository,
      gateway,
      new FakeDateTimeProvider(new DateTime(2026, 5, 9, 12, 0, 0, DateTimeKind.Utc)));

  private sealed class FakeModelTemplateRepository(ModelTemplate? modelTemplate) : IModelTemplateRepository
  {
    public Task<ModelTemplate?> GetByModelAsync(string model, CancellationToken cancellationToken) =>
      Task.FromResult(modelTemplate is not null && string.Equals(modelTemplate.Model, model, StringComparison.OrdinalIgnoreCase)
        ? modelTemplate
        : null);
  }

  private sealed class FakeProvisionedInstanceRepository : IProvisionedInstanceRepository
  {
    public List<ProvisionedInstance> AddedInstances { get; } = [];

    public Task<int> AddAsync(ProvisionedInstance provisionedInstance, CancellationToken cancellationToken)
    {
      provisionedInstance.Id = AddedInstances.Count + 1;
      AddedInstances.Add(provisionedInstance);
      return Task.FromResult(provisionedInstance.Id);
    }
  }

  private sealed class FakeDateTimeProvider(DateTime utcNow) : IDateTimeProvider
  {
    public DateTime UtcNow { get; } = utcNow;
  }

  private sealed class FakeVastAiInstancesGateway : IVastAiInstancesGateway
  {
    public SearchOffersRequest? LastSearchRequest { get; private set; }

    public CreateInstanceRequest? LastCreateRequest { get; private set; }

    public SearchOffersResult SearchResult { get; init; } = new SearchOffersResult(200, "{}");

    public CreateInstanceResult CreateResult { get; init; } = new CreateInstanceResult(200, "{}");

    public Task<GetInstancesResult> GetInstancesAsync(
      VastAiInstancesRequest request,
      CancellationToken cancellationToken) =>
      Task.FromResult(new GetInstancesResult(200, "{}"));

    public Task<SearchOffersResult> SearchOffersAsync(
      SearchOffersRequest request,
      CancellationToken cancellationToken)
    {
      LastSearchRequest = request;
      return Task.FromResult(SearchResult);
    }

    public Task<CreateInstanceResult> CreateInstanceAsync(
      CreateInstanceRequest request,
      CancellationToken cancellationToken)
    {
      LastCreateRequest = request;
      return Task.FromResult(CreateResult);
    }

    public Task<ManageInstanceResult> ManageInstanceAsync(
      ManageInstanceRequest request,
      CancellationToken cancellationToken) =>
      Task.FromResult(new ManageInstanceResult(200, "{}"));

    public Task<DestroyInstanceResult> DestroyInstanceAsync(
      DestroyInstanceRequest request,
      CancellationToken cancellationToken) =>
      Task.FromResult(new DestroyInstanceResult(200, "{}"));

    public Task<RebootInstanceResult> RebootInstanceAsync(
      RebootInstanceRequest request,
      CancellationToken cancellationToken) =>
      Task.FromResult(new RebootInstanceResult(200, "{}"));
  }
}
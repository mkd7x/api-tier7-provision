# Copilot Agent Instructions: Initialize a Similar Project Template

## Goal
Create a **new .NET 10 Clean Architecture + CQRS template** matching this repository’s structure and tooling, but **without feature-specific implementations** (no Vast.ai endpoints, no health-check feature logic, no feature entities/migrations/handlers/controllers beyond scaffolding).

## Use This Existing Project as Structural Reference

### Solution layout
- `src/ApiTier7Provision.Domain`
- `src/ApiTier7Provision.Application`
- `src/ApiTier7Provision.Infrastructure`
- `src/ApiTier7Provision.Api`
- `tests/ApiTier7Provision.Application.Tests`
- solution file at repo root (`*.slnx`)

### Architectural boundaries
- **Domain**: entities/value objects only (template should start mostly empty)
- **Application**: CQRS contracts/handlers + abstractions (template should provide only generic scaffolding)
- **Infrastructure**: EF Core + external implementations of Application abstractions
- **Api**: controller host, DI wiring, request routing
- **Tests**: application-layer unit tests for template examples only

### Baseline infrastructure patterns to preserve
- `AddApplication()` extension method in Application for MediatR registration
- `AddInfrastructure(IConfiguration)` extension method in Infrastructure for data + client wiring
- API startup in `Program.cs`:
  - `AddOpenApi()`
  - `AddApplication()`
  - `AddInfrastructure()`
  - `AddControllers()`
  - `MapControllers()`
  - automatic DB migration at startup (keep as template option)

### Tooling + runtime assets to include
- `Dockerfile` (multi-stage SDK -> ASP.NET runtime)
- `docker-compose.yml` (API + Postgres + persistent volume)
- `.dockerignore`
- `.gitignore`
- `.env` + `example.env` pattern (no secrets in git)
- `README.md` with setup/run instructions

---

## Required Initialization Steps

1. Create solution and projects:
   - Class libraries: Domain, Application, Infrastructure
   - Web API: Api
   - Test project: Application.Tests
2. Add project references:
   - Application -> Domain
   - Infrastructure -> Domain, Application
   - Api -> Application, Infrastructure
   - Tests -> Application, Domain
3. Add NuGet dependencies (same versions/purpose as current project unless updated intentionally):
   - Application: `MediatR`, DI abstractions
   - Infrastructure: EF Core, EF Design, Npgsql provider, HTTP client/options packages
   - Api: OpenAPI + EF Design for migrations tooling
   - Tests: xUnit + test SDK
4. Add clean template scaffolding files:
   - `DependencyInjection.cs` in Application and Infrastructure
   - `Program.cs` in Api with DI and controller mapping
   - minimal `AppDbContext` in Infrastructure (no feature tables/entities)
   - placeholder abstractions/interfaces only where necessary
5. Add container/dev config:
   - Dockerfile and compose stack
   - Postgres service healthcheck + volume
   - API env wiring through `.env`
6. Add documentation:
   - README with local run commands (`dotnet` + `docker compose`)
   - describe architecture layers and extension points for adding features

---

## Explicitly Exclude From Template
- Any feature-specific controllers/endpoints (e.g., Vast.ai proxy endpoints)
- Any feature CQRS handlers/commands/queries tied to business use-cases
- Any feature entity/table names (e.g., health logs)
- Any API keys/secrets, sample real credentials, or hardcoded tenant-specific values
- Any migration based on feature tables (only keep migration scaffolding if generic baseline DB is required)

---

## Suggested Command Sequence

```powershell
dotnet new sln -n <solution-name>
dotnet new classlib -n <DomainProject> -f net10.0 -o src\<DomainProject>
dotnet new classlib -n <ApplicationProject> -f net10.0 -o src\<ApplicationProject>
dotnet new classlib -n <InfrastructureProject> -f net10.0 -o src\<InfrastructureProject>
dotnet new webapi -n <ApiProject> -f net10.0 --no-https -o src\<ApiProject>
dotnet new xunit -n <ApplicationTestsProject> -f net10.0 -o tests\<ApplicationTestsProject>

dotnet sln <solution>.slnx add <all projects>
dotnet add <reference commands...>
dotnet add <package commands...>
```

---

## Acceptance Criteria for the Template
- Solution builds and tests pass with no feature code present.
- API starts and exposes OpenAPI + controller routing.
- Docker compose starts API + Postgres successfully.
- Architecture boundaries are clean and ready for feature implementation.
- README clearly explains how to add the first feature using CQRS in this structure.

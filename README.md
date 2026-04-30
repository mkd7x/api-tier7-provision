# api-tier7-provision

Clean Architecture .NET 10 Web API with CQRS (MediatR), PostgreSQL persistence, and Docker-based local development.

## Structure

- `src/ApiTier7Provision.Domain`: domain entities
- `src/ApiTier7Provision.Application`: CQRS commands/handlers and abstractions
- `src/ApiTier7Provision.Infrastructure`: EF Core + PostgreSQL implementations
- `src/ApiTier7Provision.Api`: API host and endpoints
- `tests/ApiTier7Provision.Application.Tests`: application-layer unit tests

## Run with Docker Compose

```bash
docker compose up --build
```

API: `http://localhost:8080`  
PostgreSQL: `localhost:5432` (`postgres` / `postgres`)

## Health endpoint

```bash
curl http://localhost:8080/health
```

Each call inserts a new row into `health_check_logs` with the current UTC timestamp.

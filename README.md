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

If you want `/api/instances` to call Vast.ai, set your API key in `.env` before starting compose:

```bash
VastAi__ApiKey=your_vast_ai_api_key
```

## Health endpoint

```bash
curl http://localhost:8080/api/health
```

Each call inserts a new row into `health_check_logs` with the current UTC timestamp.

## Instances endpoint (Vast.ai passthrough via CQRS)

```bash
curl "http://localhost:8080/api/instances?limit=25&select_filters={\"actual_status\":{\"eq\":\"running\"}}"
```

Supported query parameters are passed to Vast.ai `GET /api/v1/instances/`:
- `limit`
- `after_token`
- `order_by`
- `select_cols`
- `select_filters`

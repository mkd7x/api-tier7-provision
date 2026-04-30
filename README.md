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

## Search offers endpoint (Vast.ai passthrough via CQRS)

```bash
curl -X POST "http://localhost:8080/api/instances/search-offers" \
  -H "Content-Type: application/json" \
  -d "{\"limit\":100,\"verified\":{\"eq\":true},\"num_gpus\":{\"gte\":4},\"order\":[[\"dph_total\",\"asc\"]]}"
```

This endpoint forwards the JSON body to Vast.ai `POST /api/v0/bundles/` and returns upstream status/body.

Filter fields use operator objects (for example: `eq`, `neq`, `gt`, `lt`, `gte`, `lte`, `in`, `notin`).

## Create instance endpoint (Vast.ai passthrough via CQRS)

```bash
curl -X PUT "http://localhost:8080/api/instances/asks/1234567" \
  -H "Content-Type: application/json" \
  -d "{\"image\":\"vastai/base-image:@vastai-automatic-tag\",\"label\":\"my-instance\"}"
```

This endpoint forwards the request body to Vast.ai `PUT /api/v0/asks/{id}/` where `{id}` is the offer (ask) ID.

`template_hash_id` can be provided in the body, and request values override or merge with template defaults according to Vast.ai template precedence rules.

## Manage instance endpoint (Vast.ai passthrough via CQRS)

```bash
curl -X PUT "http://localhost:8080/api/instances/1234" \
  -H "Content-Type: application/json" \
  -d "{\"state\":\"stopped\"}"
```

This endpoint forwards to Vast.ai `PUT /api/v0/instances/{id}/` for state/label changes.

## Destroy instance endpoint (Vast.ai passthrough via CQRS)

```bash
curl -X DELETE "http://localhost:8080/api/instances/1234"
```

This endpoint forwards to Vast.ai `DELETE /api/v0/instances/{id}/`.

## Reboot instance endpoint (Vast.ai passthrough via CQRS)

```bash
curl -X PUT "http://localhost:8080/api/instances/reboot/1234"
```

This endpoint forwards to Vast.ai `PUT /api/v0/instances/reboot/{id}/`.

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

To enable spot provisioning by model template, also configure one or more model template rows through configuration. These values are upserted into the `model_templates` table on API startup after EF Core migrations run.

```bash
ModelTemplates__Templates__0__Model=your-model-name
ModelTemplates__Templates__0__SupportedGpus__0=RTX 4090
ModelTemplates__Templates__0__SupportedGpus__1=A100
ModelTemplates__Templates__0__ProvisioningTemplateUuid=your_vast_template_hash_id
```

## Insomnia collection

Import `insomnia\api-tier7-provision.insomnia.json` into Insomnia to get requests for every API endpoint.

The base environment includes:
- `base_url`
- `ask_id`
- `instance_id`

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

## Provision spot instance endpoint

```bash
curl -X POST "http://localhost:8080/api/instances/provision-spot" \
  -H "Content-Type: application/json" \
  -d "{\"model\":\"your-model-name\",\"maxDPH\":0.60,\"maxIngressCost\":0.10,\"maxEgressCost\":0.10}"
```

This endpoint:
- looks up the requested model in the `model_templates` table
- searches Vast.ai bid offers using the template's supported GPU list
- selects the cheapest qualifying offer by `dph_total`
- creates the instance with the configured `template_hash_id`
- persists the successful provision into the `instances` table

The response includes:
- `instanceId`
- `gpu`
- `totalCostPerHour`
- `ingressCost`
- `egressCost`

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

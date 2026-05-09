using ApiTier7Provision.Application.Commands.Instances;
using ApiTier7Provision.Application.Models;
using ApiTier7Provision.Application.Queries.Instances;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiTier7Provision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class InstancesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetInstances(
        [FromQuery] int? limit,
        [FromQuery(Name = "after_token")] string? afterToken,
        [FromQuery(Name = "order_by")] string? orderBy,
        [FromQuery(Name = "select_cols")] string? selectCols,
        [FromQuery(Name = "select_filters")] string? selectFilters,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetInstancesQuery(
                limit,
                afterToken,
                orderBy,
                selectCols,
                selectFilters);

            var result = await sender.Send(query, cancellationToken);
            return new ContentResult
            {
                StatusCode = result.StatusCode,
                ContentType = "application/json",
                Content = result.Payload
            };
        }
        catch (HttpRequestException exception)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                error = "vast_ai_unavailable",
                msg = exception.Message
            });
        }
    }

    [HttpPost("offers")]
    [HttpPost("search-offers")]
    public async Task<IActionResult> SearchOffers([FromBody] JsonElement payload, CancellationToken cancellationToken)
    {
        try
        {
            var query = new SearchOffersQuery(payload.GetRawText());
            var result = await sender.Send(query, cancellationToken);

            return new ContentResult
            {
                StatusCode = result.StatusCode,
                ContentType = "application/json",
                Content = result.Payload
            };
        }
        catch (HttpRequestException exception)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                error = "vast_ai_unavailable",
                msg = exception.Message
            });
        }
    }

    [HttpPut("asks/{id:int}")]
    public async Task<IActionResult> CreateInstance(int id, [FromBody] JsonElement payload, CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateInstanceCommand(id, payload.GetRawText());
            var result = await sender.Send(command, cancellationToken);

            return new ContentResult
            {
                StatusCode = result.StatusCode,
                ContentType = "application/json",
                Content = result.Payload
            };
        }
        catch (HttpRequestException exception)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                error = "vast_ai_unavailable",
                msg = exception.Message
            });
        }
    }

    [HttpPost("provision-spot")]
    public async Task<IActionResult> ProvisionSpotInstance(
        [FromBody] InstanceSpotOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new ProvisionSpotInstanceCommand(
                request.Model,
                request.MaxDPH,
                request.MaxIngressCost,
                request.MaxEgressCost);
            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }
        catch (ModelTemplateNotFoundException exception)
        {
            return NotFound(new
            {
                error = "model_template_not_found",
                msg = exception.Message
            });
        }
        catch (NoQualifyingSpotOfferException exception)
        {
            return NotFound(new
            {
                error = "no_qualifying_offers",
                msg = exception.Message
            });
        }
        catch (VastAiMalformedResponseException exception)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                error = "vast_ai_malformed_response",
                msg = exception.Message
            });
        }
        catch (VastAiProvisioningException exception)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                error = "vast_ai_provisioning_failed",
                operation = exception.Operation,
                upstream_status_code = exception.StatusCode,
                msg = exception.Message,
                details = exception.Payload
            });
        }
        catch (HttpRequestException exception)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                error = "vast_ai_unavailable",
                msg = exception.Message
            });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> ManageInstance(int id, [FromBody] JsonElement payload, CancellationToken cancellationToken)
    {
        if (payload.ValueKind != JsonValueKind.Object
            || (!payload.TryGetProperty("state", out _) && !payload.TryGetProperty("label", out _)))
        {
            return BadRequest(new
            {
                success = false,
                error = "invalid_args",
                msg = "At least one of 'state' or 'label' must be provided."
            });
        }

        try
        {
            var command = new ManageInstanceCommand(id, payload.GetRawText());
            var result = await sender.Send(command, cancellationToken);

            return new ContentResult
            {
                StatusCode = result.StatusCode,
                ContentType = "application/json",
                Content = result.Payload
            };
        }
        catch (HttpRequestException exception)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                error = "vast_ai_unavailable",
                msg = exception.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DestroyInstance(int id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new DestroyInstanceCommand(id);
            var result = await sender.Send(command, cancellationToken);

            return new ContentResult
            {
                StatusCode = result.StatusCode,
                ContentType = "application/json",
                Content = result.Payload
            };
        }
        catch (HttpRequestException exception)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                error = "vast_ai_unavailable",
                msg = exception.Message
            });
        }
    }

    [HttpPut("reboot/{id:int}")]
    public async Task<IActionResult> RebootInstance(int id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new RebootInstanceCommand(id);
            var result = await sender.Send(command, cancellationToken);

            return new ContentResult
            {
                StatusCode = result.StatusCode,
                ContentType = "application/json",
                Content = result.Payload
            };
        }
        catch (HttpRequestException exception)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                error = "vast_ai_unavailable",
                msg = exception.Message
            });
        }
    }
}

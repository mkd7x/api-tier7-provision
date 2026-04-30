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
}

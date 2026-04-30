using ApiTier7Provision.Application.HealthChecks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApiTier7Provision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Health(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new LogHealthCheckCommand(), cancellationToken);

        return Ok(new
        {
            Status = "Healthy",
            result.Id,
            result.LoggedAtUtc
        });
    }
}
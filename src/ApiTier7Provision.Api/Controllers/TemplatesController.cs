using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TemplatesController(ISender sender) : ControllerBase
{
   [HttpGet] 
   public async Task<IActionResult> GetTemplates(
    [FromQuery] int? limit,
    [FromQuery(Name = "model")] string model,
    CancellationToken cancellationToken)
    {
        var res = await sender.Send(new GetModelTemplatesQuery(model), cancellationToken);

        return new ContentResult
        {
            StatusCode = 200,
            ContentType = "application/json",
            Content = JsonSerializer.Serialize(res)
        };
    }
}
using ApiTier7Provision.Application.Commands.Instances;
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
    [FromQuery(Name = "model")] string? model,
    CancellationToken cancellationToken)
    {
        try
        {
            var res = await sender.Send(new GetModelTemplatesQuery(model), cancellationToken);

            return new ContentResult
            {
                StatusCode = 200,
                ContentType = "application/json",
                Content = JsonSerializer.Serialize(res)
            };
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                error = "invalid_model",
                msg = exception.Message
            });
        }
        catch (ModelTemplateNotFoundException exception)
        {
            return NotFound(new
            {
                error = "model_template_not_found",
                msg = exception.Message
            });
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTemplate(
        [FromQuery(Name = "model")] string? model,
        CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new DeleteModelTemplateCommand(model), cancellationToken);
            return NoContent();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                error = "invalid_model",
                msg = exception.Message
            });
        }
        catch (ModelTemplateNotFoundException exception)
        {
            return NotFound(new
            {
                error = "model_template_not_found",
                msg = exception.Message
            });
        }
        catch (ModelTemplateInUseException exception)
        {
            return Conflict(new
            {
                error = "model_template_in_use",
                msg = exception.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTemplate(
        [FromBody] CreateModelTemplateRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new CreateModelTemplateCommand(request.Model, request.SupportedGpus, request.TemplateUuid), cancellationToken);
        return Ok();
    }
}
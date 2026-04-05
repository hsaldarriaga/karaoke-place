using Microsoft.AspNetCore.Mvc;
using karaoke_place.Modules.Diagnostic;

namespace karaoke_place.Api.Diagnostic;

[ApiController]
[Route("[controller]")]
public class DiagnosticController(DiagnosticService diagnosticService) : ControllerBase
{
    private readonly DiagnosticService _diagnosticService = diagnosticService;

    [HttpGet("health")]
    public ActionResult<HealthCheck> GetHealth()
    {
        var health = _diagnosticService.GetHealth();
        return Ok(health);
    }
}

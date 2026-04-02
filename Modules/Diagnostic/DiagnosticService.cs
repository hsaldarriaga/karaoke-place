using System.Reflection;

namespace karaoke_place.Modules.Diagnostic;

public class DiagnosticService
{
    public HealthCheck GetHealth()
    {
        var version = Assembly.GetExecutingAssembly()
            .GetName()
            .Version?.ToString() ?? "1.0.0";

        return new HealthCheck
        {
            Version = version,
            Timestamp = DateTime.UtcNow
        };
    }
}

namespace karaoke_place.Modules.Diagnostic;

public class HealthCheck
{
    public string Version { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

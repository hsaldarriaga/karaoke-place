namespace karaoke_place.Modules.Diagnostic;

public class HealthCheck
{
    public string Version { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

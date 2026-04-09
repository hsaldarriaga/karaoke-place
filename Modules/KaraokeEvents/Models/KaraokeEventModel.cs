using System;

namespace karaoke_place.Modules.KaraokeEvents.Models;

public class KaraokeEvent
{
    public required int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public required string Location { get; set; } = string.Empty;
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public required int CreatedByUserId { get; set; }
    public required bool IsActive { get; set; }
    public required DateTime CreatedAt { get; set; }
}

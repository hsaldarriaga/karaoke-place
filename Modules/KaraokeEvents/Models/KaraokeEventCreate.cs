using System;

namespace karaoke_place.Modules.KaraokeEvents.Models;

public class KaraokeEventCreate
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int CreatedByUserId { get; set; }
}

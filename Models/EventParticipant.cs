using System.ComponentModel.DataAnnotations.Schema;
using karaoke_place.Modules.KaraokeEvents.Models;

namespace karaoke_place.Models;

[Table("EventParticipants")]
public class EventParticipantDB
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public ParticipantRole Role { get; set; }
    public ParticipantStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public KaraokeEventDB Event { get; set; } = null!;
    public UserDB User { get; set; } = null!;
}

namespace karaoke_place.Modules.KaraokeEvents.Models;

public class EventParticipantModel
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public ParticipantRole Role { get; set; }
    public ParticipantStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

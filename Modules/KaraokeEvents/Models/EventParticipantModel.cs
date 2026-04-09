namespace karaoke_place.Modules.KaraokeEvents.Models;

public class EventParticipantModel
{
    public required int Id { get; set; }
    public required int EventId { get; set; }
    public required int UserId { get; set; }
    public required ParticipantRole Role { get; set; }
    public required ParticipantStatus Status { get; set; }
    public required DateTime CreatedAt { get; set; }
}

namespace karaoke_place.Models;

public enum ParticipantRole
{
    Host,
    Admin,
    Participant
}

public class EventParticipant
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public ParticipantRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public KaraokeEvent Event { get; set; } = null!;
    public User User { get; set; } = null!;
}

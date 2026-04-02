namespace karaoke_place.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<KaraokeEvent> CreatedEvents { get; set; } = new List<KaraokeEvent>();
    public ICollection<EventParticipant> EventParticipations { get; set; } = new List<EventParticipant>();
    public ICollection<SongProposal> SongProposals { get; set; } = new List<SongProposal>();
}
